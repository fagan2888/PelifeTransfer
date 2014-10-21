Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Web.Script.Serialization

Namespace fincad
    Module Module1
        Function createModel(platform_url As String) As String
            Dim result As String = "Error"
            Try
                Dim export_model As New Model()
                Dim test_f3ml_model As String = "<f><n>CreateEmptyModel</n><a><p><n>ModelName</n><v><r><s>EmptyModel</s></r></v></p><p><n>BaseDate</n><v><r><d>40457</d></r></v></p><p><n>ValuationMethod</n><v><r><s>Default</s></r></v></p></a></f><f><n>FormCashDepoMarketData</n><a><p><n>MarketDataSetName</n><v><r><s>BaseRates</s></r></v></p><p><n>MaturityQuotePairs</n><v><r><s>o/n</s><d>0.0022813</d></r><r><s>t/n</s><d>0.002</d></r><r><s>s/w</s><d>0.0025025</d></r><r><s>1m</s><d>0.0025688</d></r><r><s>2m</s><d>0.0027359</d></r><r><s>3m</s><d>0.0029063</d></r></v></p><p><n>MarketConventions</n><v><r><s>CashUSD</s></r></v></p><p><n>Currency</n><v><r><s>USD</s></r></v></p></a></f><f><n>FormFRAMarketData</n><a><p><n>MarketDataSetName</n><v><r><s>LiborUSD3m:FRAMktData</s></r></v></p><p><n>MaturityQuotePairs</n><v><r><s>3m</s><d>0.003</d></r><r><s>6m</s><d>0.0035</d></r><r><s>9m</s><d>0.004</d></r></v></p><p><n>FloatingIndex</n><v><r><s>LiborUSD3m</s></r></v></p></a></f><f><n>FormVanillaIRSMarketData</n><a><p><n>MarketDataSetName</n><v><r><s>LiborUSD3m:IRSMktData</s></r></v></p><p><n>MaturityQuotePairs</n><v><r><s>3y</s><d>0.00886</d></r><r><s>4y</s><d>0.01199</d></r><r><s>5y</s><d>0.015235</d></r><r><s>7y</s><d>0.020755</d></r><r><s>10y</s><d>0.025895</d></r><r><s>15y</s><d>0.030455</d></r><r><s>20y</s><d>0.0323587</d></r><r><s>25y</s><d>0.03326</d></r></v></p><p><n>FloatingIndex</n><v><r><s>LiborUSD3m</s></r></v></p><p><n>FixedLegMarketConventions</n><v><r><s>SwapUSDAnnual</s></r></v></p><p><n>FloatingLegMarketConventions</n><v><r><s>SwapUSD3mFloating</s></r></v></p></a></f><f><n>AddSimpleBootstrappedDiscountCurveToModel</n><a><p><n>ModelName</n><v><r><s>Curve_USD_Bootstrapped</s></r></v></p><p><n>BaseModel</n><v><r><s>EmptyModel</s></r></v></p><p><n>MarketDataSets</n><v><r><s>BaseRates</s></r><r><s>LiborUSD3m:FRAMktData</s></r><r><s>LiborUSD3m:IRSMktData</s></r></v></p><p><n>Currency</n><v><r><s>USD</s></r></v></p><p><n>FuturesConvexity</n><v><r><m/></r></v></p><p><n>TurnPressureCurve</n><v><r><m/></r></v></p><p><n>InterpolationMethod</n><v><r><s>LogLinear</s></r></v></p></a></f><f><n>ExtendModelWithImpliedRateCurve</n><a><p><n>ModelName</n><v><r><s>SimpleRateModel</s></r></v></p><p><n>BaseModel</n><v><r><s>Curve_USD_Bootstrapped</s></r></v></p><p><n>CurveTag</n><v><r><s>USD LIBOR:3m</s><s>LiborRateCurve</s></r></v></p><p><n>UnderlyingCurveTag</n><v><r><s>USD</s><s>DiscountCurve</s></r></v></p><p><n>RateMarketConventions</n><v><r><s>SwapUSD3mFloating</s></r></v></p></a></f>"
                export_model.definition = test_f3ml_model
                export_model.name = "irs_model_c"
                export_model.format = "f3ml"
                export_model.external_id = "BTG Model"

                'Optional: Tagging the model
                Dim tags_list As List(Of tag) = _
                    New List(Of tag)() From { _
                                            New tag() With {.category = "FINCAD_MODEL", .name = "USD"}, _
                                            New tag() With {.category = "IRS_MODEL", .name = "USD"} _
                                            }
                export_model.tags = tags_list

                ' serialize JASON objects to string
                '**************************************************************
                Dim serializer As New JavaScriptSerializer()

                Dim serializedResult = serializer.Serialize(export_model)
                Dim restAPI = "f3platform/api/v1/model/"
                Dim data As Byte() = Encoding.UTF8.GetBytes(serializedResult)

                ' send REST Request to server
                '**************************************************************
                Dim result_post = f3platformInterface.getInstance(platform_url).SendRESTRequest(restAPI, data, "application/json", "POST")

                ' deserialize server response (JSON string to object)
                '**************************************************************
                Dim deserializedResult As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(result_post)
                Return Utils.getInstance().GetServerResponse(deserializedResult, Model_Response_Type.slug)

            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Debug.WriteLine(exception_str)
                Throw ex
            End Try
            Return result
        End Function

        Function createTrade(platform_url As String) As String
            Dim result As String = "Error"
            Try
                ' end point specification
                '**************************************************************
                Dim restAPI As String = "f3platform/api/v1/trade/"

                ' utilities functions
                '**************************************************************
                Dim fc As New F3XMLFunctionsBuilder()
                Dim json_util = New JSONTemplatesUtils()
                Dim parser = New f3parser()

                ' IRS Trade Details: Can be read from database &
                ' 3rd party systems
                '**************************************************************
                Dim product_name = "IRSWap1_c"
                Dim start_date As Date = New DateTime(2011, 1, 1)
                Dim maturity = "2y"
                Dim notional = "50 mio"
                Dim currency = "USD"
                Dim fixed_coupon = 0.05
                Dim market_convention = "SwapUSD6m"
                Dim floating_rate_index = "LiborUSD3m"
                Dim margin = 0.0
                Dim pay_receive = "Rec"
                Dim floating_leg_market_convention = "SwapUSD3mFloating"

                ' create IRS (f3ml)
                '**************************************************************
                Dim irs As String = fc.CreateInterestRateSwap( _
                                                                product_name,
                                                                start_date.ToOADate(),
                                                                maturity,
                                                                notional,
                                                                currency,
                                                                fixed_coupon,
                                                                market_convention,
                                                                floating_rate_index,
                                                                margin,
                                                                pay_receive,
                                                                floating_leg_market_convention
                                                                )

                ' convert f3ml to JSON
                '**************************************************************

                Dim table As New Hashtable()
                parser.f3mlToHashTable(table, irs)
                Dim trade As New trade()
                'Optional: Tagging the model
                Dim tags_list As List(Of tag) = _
                    New List(Of tag)() From { _
                                            New tag() With {.category = "Trade", .name = "IRS_USD"}, _
                                            New tag() With {.category = "Portfolio", .name = "FINCAD"} _
                                            }
                json_util.TradeConstruct(product_name, "fincad_trade", "vanilla-interest-rate-swap", tags_list, table, trade)


                ' serialize JASON objects to string
                '**************************************************************
                Dim serializer As New JavaScriptSerializer()

                Dim serializedResult = serializer.Serialize(trade)
                Dim data As Byte() = Encoding.UTF8.GetBytes(serializedResult)


                ' delete valspec if exist 
                ' do this only in excercise only of cause
                Try
                    f3platformInterface.getInstance(platform_url).HttpGet(restAPI + product_name.ToLower(), "DELETE")
                Catch ex As Exception
                    'NOP

                End Try
                ' send REST Request to server
                '**************************************************************
                Dim result_post = f3platformInterface.getInstance(platform_url).SendRESTRequest(restAPI, data, "application/json", "POST")

                ' deserialize server response (JSON string to object)
                '**************************************************************
                Dim deserializedResult As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(result_post)

                ' get server response
                '**************************************************************
                Return Utils.getInstance().GetServerResponse(deserializedResult, Model_Response_Type.slug)
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Debug.WriteLine(exception_str)
                Throw ex
            End Try
            Return result
        End Function

        Function valueProduct(platform_url As String, model As String, trades As List(Of String), valspec As String, requests As List(Of String), as_portfolio As Boolean, Optional callback_endpoint As String = "") As String
            Dim result As String = "Error"
            Try
                ' end point specification
                '**************************************************************
                Dim restAPI As String = "f3platform/api/v1/trade_valuation/"

                ' utilities functions
                '**************************************************************
                Dim fc As New F3XMLFunctionsBuilder()
                Dim json_util = New JSONTemplatesUtils()
                Dim parser = New f3parser()

                ' convert f3ml to JSON
                '**************************************************************

                Dim table As New Hashtable()
                Dim trade_valuation_request As New trade_valuation_template

                Dim start_date As Date = Today()
                Dim duration = New System.TimeSpan(36, 0, 0, 0)

                start_date.Add(duration)
                Dim expiry_time = start_date.ToString("yyyy-MM-dd")


                json_util.tradeValuationConstruct(model,
                                                   trades,
                                                   valspec,
                                                   requests,
                                                   expiry_time,
                                                   as_portfolio,
                                                   callback_endpoint,
                                                   trade_valuation_request)


                ' serialize JASON objects to string
                '**************************************************************
                Dim serializer As New JavaScriptSerializer()

                Dim serializedResult = serializer.Serialize(trade_valuation_request)
                Dim data As Byte() = Encoding.UTF8.GetBytes(serializedResult)

                ' send REST Request to server
                '**************************************************************
                Dim result_post = f3platformInterface.getInstance(platform_url).SendRESTRequest(restAPI, data, "application/json", "POST")

                ' deserialize server response (JSON string to object)
                '**************************************************************
                Dim deserializedResult As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(result_post)

                ' get server response
                '**************************************************************
                Return Utils.getInstance().GetServerResponse(deserializedResult, Model_Response_Type.slug)
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Debug.WriteLine(exception_str)
                Throw ex
            End Try
            Return result
        End Function

        Function createValuationSpecification(platform_url As String) As String
            Dim result As String = "Error"
            Try
                ' end point specification
                '**************************************************************
                Dim restAPI As String = "f3platform/api/v1/valuation_specification/"

                ' utilities functions
                '**************************************************************
                Dim fc As New F3XMLFunctionsBuilder()
                Dim json_util = New JSONTemplatesUtils()
                Dim parser = New f3parser()

                'Valuation Specification Details
                '**************************************************************
                Dim ValuationMethod = "Clean_ValSpec_c"
                Dim UnderlyingValuationMethod = "Default"
                Dim CleanValuationMaturity = "0d"
                Dim MaturityCalculator = "NoHolidays"
                Dim AccrualConvention = "act/365f"
                Dim OverrideProductAccrualConvention = False

                'Create the valuation specificaion (f3ml)
                '**************************************************************
                Dim valuation_spec_f3ml As String = _
                    fc.CreateCleanValuationSpecification(ValuationMethod,
                                                        UnderlyingValuationMethod,
                                                        CleanValuationMaturity,
                                                        MaturityCalculator,
                                                        AccrualConvention,
                                                        OverrideProductAccrualConvention)

                ' prepairing reqeust in JSON
                '**************************************************************

                Dim valspectemp As New common_template()
                'optional: external id
                Dim external_id = "fincad_valspec"
                'Optional: Tagging the valuation spec
                Dim tags_list As List(Of tag) = _
                    New List(Of tag)() From { _
                                            New tag() With {.category = "ValuationSpec", .name = "Default"}, _
                                            New tag() With {.category = "FINCAD_ValSpec", .name = "FINCAD"} _
                                            }

                json_util.commonConstruct(ValuationMethod, valuation_spec_f3ml, valspectemp, external_id, tags_list)


                ' serialize JASON objects to string
                '**************************************************************
                Dim serializer As New JavaScriptSerializer()

                Dim serializedResult = serializer.Serialize(valspectemp)
                Dim data As Byte() = Encoding.UTF8.GetBytes(serializedResult)

                ' delete valspec if exist 
                ' do this only in excercise only of cause
                Try
                    f3platformInterface.getInstance(platform_url).HttpGet(restAPI + ValuationMethod.ToLower(), "DELETE")
                Catch ex As Exception
                    'NOP
                End Try


                ' send REST Request to server
                '**************************************************************

                Dim result_post = f3platformInterface.getInstance(platform_url).SendRESTRequest(restAPI, data, "application/json", "POST")

                ' deserialize server response (JSON string to object)
                '**************************************************************
                Dim deserializedResult As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(result_post)
                Return Utils.getInstance().GetServerResponse(deserializedResult, Model_Response_Type.slug)
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Debug.WriteLine(exception_str)
            End Try
            Return result
        End Function

        '        Sub Main()
        '            Try
        '                ' end point specification
        '                '**************************************************************
        '                Dim platform_url As String = "http://rj-risc-0044:8505/"

        '                ' utilities functions
        '                '**************************************************************
        '                Dim json_util = New JSONTemplatesUtils()
        '                Dim parser = New f3parser()

        '                Dim valuation_model = createModel(platform_url)
        '                Dim irs_trade = createTrade(platform_url)
        '                Dim val_spec = createValuationSpecification(platform_url)
        '                Dim requests = New List(Of String) From {"ValuationExecutionTime", "CalibrationExecutionTime", "Value"}
        '                Dim portfolio = New List(Of String) From {irs_trade}

        '#If False Then


        '                ' Send Value Product Call with a callback
        '                '**************************************************************
        '                Dim slug = valueProduct(platform_url, valuation_model, portfolio, val_spec, requests, True, "http://127.0.0.1:61095/RestServiceImpl.svc/postJson")
        '#Else

        '                ' Send Value Product Call with polling resuls
        '                '**************************************************************
        '                Dim slug = valueProduct(platform_url, valuation_model, portfolio, val_spec, requests, True)
        '                While Utils.getInstance().getValuationResult(platform_url, slug) = False
        '                    Thread.Sleep(1)
        '                End While
        '#End If
        '            Catch ex As Exception
        '                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
        '                Debug.WriteLine(exception_str)
        '            End Try
        '        End Sub

        Sub Main()
            POCCDS.Execute()
        End Sub


    End Module
End Namespace
