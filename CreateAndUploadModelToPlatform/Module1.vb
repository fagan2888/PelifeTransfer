Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Web.Script.Serialization

Namespace fincad
    Module Module1
        Private Function CreateIRSJSON(platform_url As String, name As String, api As String) As String
            ' end point specification
            '**************************************************************
            Dim restAPI As String = "f3platform/api/v1/market_data_set/"

            ' utilities functions
            '**************************************************************
            Dim fc As New F3XMLFunctionsBuilder()
            Dim json_util = New JSONTemplatesUtils()
            Dim parser = New f3parser()

            Dim maturities As New List(Of List(Of Object))()
            maturities.Add(New List(Of Object)() From { _
             "1y" _
            })
            maturities.Add(New List(Of Object)() From { _
             "5y" _
            })
            maturities.Add(New List(Of Object)() From { _
             "10y" _
            })

            ' simulatin: add some random fraction to the quotes
            Dim ran As New Random()
            Dim frac As Double = ran.NextDouble() / 100
            Dim quotes As New List(Of List(Of Object))()
            quotes.Add(New List(Of Object)() From { _
             0.02 + frac _
            })
            quotes.Add(New List(Of Object)() From { _
             0.022 + frac _
            })
            quotes.Add(New List(Of Object)() From { _
             0.03 + frac _
            })

            Dim market_data_set_name = name
            ' recreate USD IRS market data object
            Dim irs_md_f3ml = fc.CreateHomogeneousMarketDataElement(market_data_set_name, "USD-IRS-Annual-Libor-3m-Uncollat", maturities, quotes, "IRS:12m:USD LIBOR:3m", "InterestRateSwap")

            ' prepairing reqeust in JSON
            '**************************************************************

            Dim market_data_template As New common_template()
            'optional: external id
            Dim external_id = market_data_set_name + "_ext"
            'Optional: Tagging the valuation spec
            Dim tags_list As List(Of tag) = _
                New List(Of tag)() From { _
                                        New tag() With {.category = "Market_Data", .name = market_data_set_name}, _
                                        New tag() With {.category = "FINCAD_Market_Data", .name = "FINCAD"} _
                                        }

            json_util.commonConstruct(market_data_set_name, irs_md_f3ml, market_data_template, external_id, tags_list)

            ' serialize JASON objects to string
            '**************************************************************
            Dim serializer As New JavaScriptSerializer()

            Dim serializedResult = serializer.Serialize(market_data_template)
            Dim data As Byte() = Encoding.UTF8.GetBytes(serializedResult)
            ' send REST Request to server
            '**************************************************************

            Dim result_post = f3platformInterface.getInstance(platform_url).SendRESTRequest(restAPI, data, "application/json", api)

            ' deserialize server response (JSON string to object)
            '**************************************************************
            Dim deserializedResult As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(result_post)
            Return Utils.getInstance().GetServerResponse(deserializedResult, Common_Response_Type.slug)
        End Function

        Private Function CreateCashDepoJSON(platform_url As String, name As String, api As String) As String
            Dim restAPI As String = "f3platform/api/v1/market_data_set/"

            ' utilities functions
            '**************************************************************
            Dim fc As New F3XMLFunctionsBuilder()
            Dim json_util = New JSONTemplatesUtils()
            Dim parser = New f3parser()

            Dim maturities As New List(Of List(Of Object))()
            maturities.Add(New List(Of Object)() From { _
             "1y" _
            })
            maturities.Add(New List(Of Object)() From { _
             "5y" _
            })
            maturities.Add(New List(Of Object)() From { _
             "10y" _
            })

            ' simulatin: add some random fraction to the quotes
            Dim ran As New Random()
            Dim frac As Double = ran.NextDouble() / 100
            Dim quotes As New List(Of List(Of Object))()
            quotes.Add(New List(Of Object)() From { _
             0.02 + frac _
            })
            quotes.Add(New List(Of Object)() From { _
             0.022 + frac _
            })
            quotes.Add(New List(Of Object)() From { _
             0.03 + frac _
            })
            Dim market_data_set_name = name
            ' recreate USD CASHDEPO market data object
            Dim cashdepo_md_f3ml = fc.CreateHomogeneousMarketDataElement(market_data_set_name, "USD-CashDepo", maturities, quotes, "CashDeposit:USD", "CashDepo")

            ' prepairing reqeust in JSON
            '**************************************************************

            Dim market_data_template As New common_template()
            'optional: external id
            Dim external_id = market_data_set_name + "_ext"
            'Optional: Tagging the valuation spec
            Dim tags_list As List(Of tag) = _
                New List(Of tag)() From { _
                                        New tag() With {.category = "Market_Data", .name = market_data_set_name}, _
                                        New tag() With {.category = "FINCAD_Market_Data", .name = "FINCAD"} _
                                        }

            json_util.commonConstruct(market_data_set_name, cashdepo_md_f3ml, market_data_template, external_id, tags_list)

            ' serialize JASON objects to string
            '**************************************************************
            Dim serializer As New JavaScriptSerializer()

            Dim serializedResult = serializer.Serialize(market_data_template)
            Dim data As Byte() = Encoding.UTF8.GetBytes(serializedResult)
            ' send REST Request to server
            '**************************************************************

            Dim result_post = f3platformInterface.getInstance(platform_url).SendRESTRequest(restAPI, data, "application/json", api)

            ' deserialize server response (JSON string to object)
            '**************************************************************
            Dim deserializedResult As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(result_post)
            Return Utils.getInstance().GetServerResponse(deserializedResult, Common_Response_Type.slug)

        End Function

        Function createSelector(platform_url As String,
                                 selector_name As String,
                                 slug_name As String,
                                 method As String) As String

            Dim restAPI As String = "f3platform/api/v1/selector/"


            ' utilities functions
            '**************************************************************

            Dim json_util = New JSONTemplatesUtils()
            Dim selector_template As New selector_template()

            Dim filter As New filters_template()
            Dim extractor As New extractor_template()
            filter.field = "name"
            filter.operator_ = "="
            filter.value = slug_name.Trim()
            filter.property_type = "object_property"
            Dim filters = New List(Of filters_template) From {filter}
            extractor.field = "modified"
            extractor.operator_ = "highest"
            extractor.count = "1"
            extractor.property_type = "object_property"
            Dim external_id = selector_name + "_ext"

            'Optional: Tagging the valuation spec
            Dim tags_list As List(Of tag) = _
                New List(Of tag)() From { _
                                        New tag() With {.category = "Selector", .name = selector_name}, _
                                        New tag() With {.category = "FINCAD_Selector", .name = "FINCAD"} _
                                        }

            json_util.SelectorConstruct(selector_name.Trim(), filters, extractor, selector_template, external_id, tags_list)


            ' serialize JASON objects to string
            '**************************************************************
            Dim serializer As New JavaScriptSerializer()

            Dim serializedResult = serializer.Serialize(selector_template)

            ' IMPORTANT:
            '       Replace "operator_" with "operator" is necessary only
            '       in vb.net as "operator" is a keyword in VB
            '**************************************************************
            serializedResult = serializedResult.Replace(",""operator_"":", ",""operator"":")
            Dim data As Byte() = Encoding.UTF8.GetBytes(serializedResult)

            ' delete valspec if exist 
            ' do this only in excercise only of cause
            Try
                f3platformInterface.getInstance(platform_url).HttpGet(restAPI + selector_name.ToLower(), "DELETE")
            Catch ex As Exception
                'NOP
            End Try
            ' send REST Request to server
            '**************************************************************
            Dim result_post = f3platformInterface.getInstance(platform_url).SendRESTRequest(restAPI, data, "application/json", method)
            Debug.WriteLine(result_post)
            ' deserialize server response (JSON string to object)
            '**************************************************************
            Dim deserializedResult As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(result_post)
            Return Utils.getInstance().GetServerResponse(deserializedResult, Common_Response_Type.slug)

        End Function

        Function createIndexFixing(platform_url As String,
                                   fixing_name As String,
                                   f3_name As String) As String

            Dim restAPI As String = "f3platform/api/v1/index_fixings/"


            ' utilities functions
            '**************************************************************

            Dim json_util = New JSONTemplatesUtils()

            Dim index_fixings_template = New index_fixings_template()

            Dim external_id = "name " + "ext"

            ' simulation: add some random fraction to the quotes
            Dim ran As New Random()
            Dim frac As Double = ran.NextDouble() / 100
            Dim fixing_quotes As New List(Of List(Of Object))()
            fixing_quotes.Add(New List(Of Object)() From { _
            "2014-03-11", 0.02 + frac _
            })
            fixing_quotes.Add(New List(Of Object)() From { _
            "2014-03-12", 0.02 + frac _
            })
            fixing_quotes.Add(New List(Of Object)() From { _
            "2014-03-13", 0.02 + frac _
            })
            fixing_quotes.Add(New List(Of Object)() From { _
             "2014-03-14", 0.022 + frac _
            })
            fixing_quotes.Add(New List(Of Object)() From { _
             "2014-03-17", 0.03 + frac _
            })
            fixing_quotes.Add(New List(Of Object)() From { _
             "2014-03-18", 0.03 + frac _
            })


            'Optional: Tagging the valuation spec
            Dim tags_list As List(Of tag) = _
                New List(Of tag)() From { _
                                        New tag() With {.category = "Selector", .name = fixing_name}, _
                                        New tag() With {.category = "FINCAD_Selector", .name = "FINCAD"} _
                                        }

            json_util.IndexFixingsConstruct(fixing_name, _
                                            index_fixings_template, _
                                            f3_name, _
                                            fixing_quotes, _
                                            external_id, _
                                            tags_list)


            ' serialize JASON objects to string
            '**************************************************************
            Dim serializer As New JavaScriptSerializer()

            Dim serializedResult = serializer.Serialize(index_fixings_template)

            Dim data As Byte() = Encoding.UTF8.GetBytes(serializedResult)

            ' delete valspec if exist 
            ' do this only in excercise only of cause
            Try
                f3platformInterface.getInstance(platform_url).HttpGet(restAPI + fixing_name.ToLower(), "DELETE")
            Catch ex As Exception
                'NOP
            End Try
            ' send REST Request to server
            '**************************************************************
            Dim result_post = f3platformInterface.getInstance(platform_url).SendRESTRequest(restAPI, data, "application/json", RES_Method_Type.POST)
            ' deserialize server response (JSON string to object)
            '**************************************************************
            Dim deserializedResult As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(result_post)
            Return Utils.getInstance().GetServerResponse(deserializedResult, Common_Response_Type.slug)

        End Function

        Function createModel_Fragment(platform_url As String, ByRef out_model_fregments_list As List(Of String)) As String
            Try

                Dim restAPI = "f3platform/api/v1/model_fragment/"

                ' utilities functions
                '**************************************************************

                Dim json_util = New JSONTemplatesUtils()
                Dim model_fregment_template = New Model()

                ' construct valuation model in JSON format
                '**************************************************************

                Dim model_template As New common_template()

                ' Here we only have one consistant model for the test. 
                ' The modeling instructions can be read from a .func/ f3ml files
                ' exported from F3 Excel Edition. 
                ' In fact, you can break down each modeling instruction into a
                ' model fragment and assamble them according to your flavor when
                ' create the model recipe. This will allow you to create multiple
                ' model receipes
                '**************************************************************

                Dim test_f3ml_model As String = "<f><n>CreateEmptyModel</n><a><p><n>ModelName</n><v><r><s>EmptyModel</s></r></v></p><p><n>BaseDate</n><v><r><d>40457</d></r></v></p><p><n>ValuationMethod</n><v><r><s>Default</s></r></v></p></a></f><f><n>FormCashDepoMarketData</n><a><p><n>MarketDataSetName</n><v><r><s>BaseRates</s></r></v></p><p><n>MaturityQuotePairs</n><v><r><s>o/n</s><d>0.0022813</d></r><r><s>t/n</s><d>0.002</d></r><r><s>s/w</s><d>0.0025025</d></r><r><s>1m</s><d>0.0025688</d></r><r><s>2m</s><d>0.0027359</d></r><r><s>3m</s><d>0.0029063</d></r></v></p><p><n>MarketConventions</n><v><r><s>CashUSD</s></r></v></p><p><n>Currency</n><v><r><s>USD</s></r></v></p></a></f><f><n>FormFRAMarketData</n><a><p><n>MarketDataSetName</n><v><r><s>LiborUSD3m:FRAMktData</s></r></v></p><p><n>MaturityQuotePairs</n><v><r><s>3m</s><d>0.003</d></r><r><s>6m</s><d>0.0035</d></r><r><s>9m</s><d>0.004</d></r></v></p><p><n>FloatingIndex</n><v><r><s>LiborUSD3m</s></r></v></p></a></f><f><n>FormVanillaIRSMarketData</n><a><p><n>MarketDataSetName</n><v><r><s>LiborUSD3m:IRSMktData</s></r></v></p><p><n>MaturityQuotePairs</n><v><r><s>3y</s><d>0.00886</d></r><r><s>4y</s><d>0.01199</d></r><r><s>5y</s><d>0.015235</d></r><r><s>7y</s><d>0.020755</d></r><r><s>10y</s><d>0.025895</d></r><r><s>15y</s><d>0.030455</d></r><r><s>20y</s><d>0.0323587</d></r><r><s>25y</s><d>0.03326</d></r></v></p><p><n>FloatingIndex</n><v><r><s>LiborUSD3m</s></r></v></p><p><n>FixedLegMarketConventions</n><v><r><s>SwapUSDAnnual</s></r></v></p><p><n>FloatingLegMarketConventions</n><v><r><s>SwapUSD3mFloating</s></r></v></p></a></f><f><n>AddSimpleBootstrappedDiscountCurveToModel</n><a><p><n>ModelName</n><v><r><s>Curve_USD_Bootstrapped</s></r></v></p><p><n>BaseModel</n><v><r><s>EmptyModel</s></r></v></p><p><n>MarketDataSets</n><v><r><s>BaseRates</s></r><r><s>LiborUSD3m:FRAMktData</s></r><r><s>LiborUSD3m:IRSMktData</s></r></v></p><p><n>Currency</n><v><r><s>USD</s></r></v></p><p><n>FuturesConvexity</n><v><r><m/></r></v></p><p><n>TurnPressureCurve</n><v><r><m/></r></v></p><p><n>InterpolationMethod</n><v><r><s>LogLinear</s></r></v></p></a></f><f><n>ExtendModelWithImpliedRateCurve</n><a><p><n>ModelName</n><v><r><s>SimpleRateModel</s></r></v></p><p><n>BaseModel</n><v><r><s>Curve_USD_Bootstrapped</s></r></v></p><p><n>CurveTag</n><v><r><s>USD LIBOR:3m</s><s>LiborRateCurve</s></r></v></p><p><n>UnderlyingCurveTag</n><v><r><s>USD</s><s>DiscountCurve</s></r></v></p><p><n>RateMarketConventions</n><v><r><s>SwapUSD3mFloating</s></r></v></p></a></f><f><n>CreateEmptyModel</n><a><p><n>ModelName</n><v><r><s>empty_model</s></r></v></p><p><n>BaseDate</n><v><r><d>41717</d></r></v></p><p><n>ValuationMethod</n><v><r><s>Default</s></r></v></p></a></f><f><n>CreateHomogeneousMarketDataElement</n><a><p><n>MarketDataSetName</n><v><r><s>usd_irs_md</s></r></v></p><p><n>InstrumentType</n><v><r><s>USD-IRS-Annual-Libor-3m-Uncollat</s></r></v></p><p><n>QuoteSpecifications</n><v><r><s>1y</s></r><r><s>5y</s></r><r><s>10y</s></r></v></p><p><n>Quotes</n><v><r><d>0.02</d></r><r><d>0.022</d></r><r><d>0.03</d></r></v></p><p><n>MarketDataName</n><v><r><m/></r></v></p><p><n>MarketDataType</n><v><r><m/></r></v></p></a></f><f><n>CreateHomogeneousMarketDataElement</n><a><p><n>MarketDataSetName</n><v><r><s>usd_cashdepo_md</s></r></v></p><p><n>InstrumentType</n><v><r><s>USD-CashDepo</s></r></v></p><p><n>QuoteSpecifications</n><v><r><s>1y</s></r><r><s>5y</s></r><r><s>10y</s></r></v></p><p><n>Quotes</n><v><r><d>0.02</d></r><r><d>0.022</d></r><r><d>0.03</d></r></v></p><p><n>MarketDataName</n><v><r><m/></r></v></p><p><n>MarketDataType</n><v><r><m/></r></v></p></a></f><f><n>CombineMarketDataSets</n><a><p><n>MarketDataSetName</n><v><r><s>composit_md</s></r></v></p><p><n>InputMarketDataSets</n><v><r><s>usd_irs_md</s></r><r><s>usd_cashdepo_md</s></r></v></p></a></f><f><n>ExtendModelWithMarketData</n><a><p><n>ModelName</n><v><r><s>model_with_md</s></r></v></p><p><n>BaseModel</n><v><r><s>empty_model</s></r></v></p><p><n>MarketData</n><v><r><s>composit_md</s></r></v></p></a></f><f><n>AddSimpleFixingsCurveToModel</n><a><p><n>ModelName</n><v><r><s>model_with_fixing</s></r></v></p><p><n>BaseModel</n><v><r><s>model_with_md</s></r></v></p><p><n>Fixings</n><v><r><d>41716</d><d>0.024</d></r><r><d>41715</d><d>0.023</d></r></v></p><p><n>Index</n><v><r><s>LiborUSD3m</s></r></v></p></a></f><f><n>ExtendModelWithBootstrappedInterpolationCurve</n><a><p><n>ModelName</n><v><r><s>model_with_discount_curve</s></r></v></p><p><n>BaseModel</n><v><r><s>model_with_fixing</s></r></v></p><p><n>CurveTag</n><v><r><s>USD</s><s>DiscountCurve</s></r></v></p><p><n>MarketDataTag</n><v><r><s>CashDeposit:USD</s><s>CashDepo</s></r></v></p><p><n>InterpolationMethod</n><v><r><s>LogLinear</s></r></v></p><p><n>BootstrappingObjective</n><v><r><s>SingleCurrencyValue</s></r></v></p><p><n>AutoSort</n><v><r><b>F</b></r></v></p></a></f><f><n>ExtendModelWithBootstrappedUnanchoredInterpolationCurve</n><a><p><n>ModelName</n><v><r><s>valuation_model</s></r></v></p><p><n>BaseModel</n><v><r><s>model_with_discount_curve</s></r></v></p><p><n>CurveTag</n><v><r><s>USD LIBOR:3m</s><s>LiborRateCurve</s></r></v></p><p><n>MarketDataTag</n><v><r><s>IRS:12m:USD LIBOR:3m</s><s>InterestRateSwap</s></r></v></p><p><n>InterpolationMethod</n><v><r><s>Linear</s></r></v></p><p><n>BootstrappingObjective</n><v><r><s>SingleCurrencyValue</s></r></v></p><p><n>AutoSort</n><v><r><b>F</b></r></v></p></a></f>"
                Dim model_freg_name = "test_irs_model"

                'Optional: Tagging the model
                Dim tags_list As List(Of tag) = _
                    New List(Of tag)() From { _
                                            New tag() With {.category = "MODEL_FREGMENT", .name = "USD"}, _
                                            New tag() With {.category = "IRS_MODEL_FREGMENT", .name = "USD"} _
                                            }

                json_util.commonConstruct(model_freg_name,
                                          test_f3ml_model,
                                          model_template,
                                          "",
                                          tags_list)

                ' serialize JASON objects to string
                '**************************************************************
                Dim serializer As New JavaScriptSerializer()

                Dim serializedResult = serializer.Serialize(model_template)

                Dim data As Byte() = Encoding.UTF8.GetBytes(serializedResult)


                ' delete model fregment if exist 
                ' do this only in excercise only of cause
                Try
                    f3platformInterface.getInstance(platform_url).HttpGet(restAPI + model_freg_name.ToLower(), "DELETE")
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
                Dim model_slug = Utils.getInstance().GetServerResponse(deserializedResult, Model_Response_Type.slug)
                out_model_fregments_list.Add(model_slug)
                Return ("OK")
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Debug.WriteLine(exception_str)
            End Try
            Return ("Error")
        End Function


        Function createMarket_Data_Set(platform_url As String, ByRef market_data_selectors As List(Of String)) As String
            Dim result As String = "OK"
            Try
                ' create IRS market data and send the requests
                '**************************************************************
                Dim IRS_md_name = "usd_irs_swap_md"
                Dim IRS_md_slug = CreateIRSJSON(platform_url, IRS_md_name, "POST")

                ' create a selector IRS market data and
                ' send the requests
                ' idealy for each market data object, you want to create one
                ' selector. this is needed only done once.
                ' with the selector, we can group it with the model object
                '**************************************************************
                Dim irs_md_selector_name = "mds-selector-irs-1"

                Dim selector_irs_slug = createSelector(platform_url,
                                                        irs_md_selector_name,
                                                        IRS_md_name,
                                                        RES_Method_Type.POST)
                Debug.WriteLine(selector_irs_slug)


                ' create cash depo market data and send the requests
                '**************************************************************
                Dim CASH_md_name = "usd_cash_depo_md"
                Dim cash_depo_md_slug = CreateCashDepoJSON(platform_url, CASH_md_name, "POST")


                ' create a selector cash depo and IRS market data and
                ' send the requests
                ' idealy for each market data object, you want to create one
                ' selector. this is needed only done once.
                ' with the selector, we can group it with the model object
                '**************************************************************
                Dim cash_depo_md_selector_name = "mds-selector-cash-c"


                Dim selector_cash_slug = createSelector(platform_url,
                                                        cash_depo_md_selector_name,
                                                        CASH_md_name,
                                                        RES_Method_Type.POST)
                market_data_selectors.Add(selector_irs_slug)
                market_data_selectors.Add(selector_cash_slug)

                Debug.WriteLine(selector_cash_slug)

            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Debug.WriteLine(exception_str)
            End Try
            Return result
        End Function

        Function createModel_Recipe(platform_url As String,
                                    recipe_obj_name As String,
                                    fixing_requirements As List(Of fixing_requirements_template),
                                    model_fragments As List(Of String),
                                    market_data_selectors As List(Of String)) As String
            Try

                Dim restAPI = "f3platform/api/v1/model_recipe/"

                ' utilities functions
                '**************************************************************
                Dim json_util = New JSONTemplatesUtils()

                ' construct valuation model in JSON format
                '**************************************************************
                Dim model_recipe_template = New model_recipe_template()

                'Optional: Tagging the model
                Dim tags_list As List(Of tag) = _
                    New List(Of tag)() From { _
                                            New tag() With {.category = "MODEL_RECIPE", .name = "USD"}, _
                                            New tag() With {.category = "IRS_MODEL_RECIPE", .name = "USD"} _
                                            }

                ' construct a model recipes
                '**************************************************************
                json_util.modelRecipeConstruct(recipe_obj_name, model_recipe_template, fixing_requirements, market_data_selectors, model_fragments, "", tags_list)

                ' serialize JASON objects to string
                '**************************************************************
                Dim serializer As New JavaScriptSerializer()

                Dim serializedResult = serializer.Serialize(model_recipe_template)

                Dim data As Byte() = Encoding.UTF8.GetBytes(serializedResult)

                ' delete model recipe if exist 
                ' do this only in excercise only of cause
                Try
                    f3platformInterface.getInstance(platform_url).HttpGet(restAPI + recipe_obj_name.ToLower(), "DELETE")
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
                Dim model_recipe_slug = Utils.getInstance().GetServerResponse(deserializedResult, Common_Response_Type.slug)
                Return (model_recipe_slug)
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Debug.WriteLine(exception_str)
            End Try
            Return ("Error")
        End Function


        Function createValuationModelFromRecipe(platform_url As String,
                                                valuation_model_name As String,
                                                definition As valuation_model_definition_template,
                                                Optional format As String = "") As String
            Try

                Dim restAPI = "f3platform/api/v1/model/"

                ' utilities functions
                '**************************************************************
                Dim json_util = New JSONTemplatesUtils()

                ' construct valuation model in JSON format
                '**************************************************************
                Dim out_valuation_model_from_recipe_template = New valuation_model_from_recipe_template()

                'Optional: Tagging the model
                Dim tags_list As List(Of tag) = _
                    New List(Of tag)() From { _
                                            New tag() With {.category = "MODEL_RECIPE", .name = "USD"}, _
                                            New tag() With {.category = "IRS_MODEL_RECIPE", .name = "USD"} _
                                            }
                Dim valuaion_model_name = "SimpleTestModel"
                json_util.valuationModelFromRecipeConstruct(valuaion_model_name, out_valuation_model_from_recipe_template, definition, , , tags_list)


                ' serialize JASON objects to string
                '**************************************************************
                Dim serializer As New JavaScriptSerializer()

                Dim serializedResult = serializer.Serialize(out_valuation_model_from_recipe_template)

                Dim data As Byte() = Encoding.UTF8.GetBytes(serializedResult)

                ' send REST Request to server
                '**************************************************************
                Dim result_post = f3platformInterface.getInstance(platform_url).SendRESTRequest(restAPI, data, "application/json", "POST")

                ' deserialize server response (JSON string to object)
                '**************************************************************
                Dim deserializedResult As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(result_post)

                ' get server response
                '**************************************************************
                Dim model_recipe_slug = Utils.getInstance().GetServerResponse(deserializedResult, Common_Response_Type.slug)
                Return (model_recipe_slug)
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Debug.WriteLine(exception_str)
            End Try
            Return ("Error")
        End Function

        Function valueProduct(platform_url As String,
                              model As String,
                              trades As List(Of String),
                              valspec As String,
                              requests As List(Of String),
                              as_portfolio As Boolean,
                              Optional callback_endpoint As String = "") As String
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
                Dim start_date As Date = New DateTime(2014, 3, 21)
                Dim maturity = "2y"
                Dim notional = "1 mio"
                Dim currency = "USD"
                Dim fixed_coupon = 0.03
                Dim market_convention = "SwapUSDAnnual"
                Dim floating_rate_index = "LiborUSD3m"
                Dim margin = 0.0
                Dim pay_receive = "pay"
                Dim floating_leg_market_convention = "SwapUSD3m"

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

        Sub Main()
            Try
                '0) end point specification
                '**************************************************************
                Dim platform_url As String = "http://localhost:8505/"
                Dim market_selectors = New List(Of String)

                '1) create market data
                '**************************************************************
                createMarket_Data_Set(platform_url, market_selectors)

                '2) create fixing
                '**************************************************************
                Dim index_fixing_name = "LiborUSD3m_c"
                Dim f3_name = "LiborUSD3m"
                Dim index_fixing_slug = createIndexFixing(platform_url, index_fixing_name, f3_name)
                Dim fixing_req = New List(Of fixing_requirements_template)
                fixing_req.Add(New fixing_requirements_template With {.index = index_fixing_slug, .required_days = 3})


                '3) Create a IRS Model Fragment
                '   In this case we have one big fragment that include all the 
                '   modeling instructions to price a swap.
                '   In fact, you can break down the modeling instructions into
                '   a single instruction per modeling fragment; hence, allowing
                '   even more granularity whereby you can create multiple recipes
                '   by groping a set of model fragments
                '**************************************************************
                Dim model_fragments = New List(Of String)
                createModel_Fragment(platform_url, model_fragments)

                '4) IRS Model Recipe that group market data, fixing and 
                '   Model fragment togather
                '**************************************************************
                Dim model_recipe_name = "irs_recipe"

                Dim model_recipe_slug = createModel_Recipe(platform_url, model_recipe_name, fixing_req, model_fragments, market_selectors)

                '5) Create Valuation Model using the model recipe
                '**************************************************************
                Dim valuation_model_name = "SimpleTestModel"
                Dim definition = New valuation_model_definition_template()
                definition.model_recipe = model_recipe_name
                definition.base_model = "2014-03-19"
                definition.valuation_date = "2014-03-19"
                Dim valuation_model_slug = createValuationModelFromRecipe(platform_url, valuation_model_name, definition)
                Dim valuation_model = valuation_model_slug

                '6) Create IRS Trade
                '**************************************************************

                Dim irs_trade = createTrade(platform_url)
                Dim portfolio = New List(Of String) From {irs_trade}

                '7) Create Valuation Specification
                '**************************************************************
                Dim val_spec = createValuationSpecification(platform_url)

                '8) Create List of Requests
                '**************************************************************
                Dim requests = New List(Of String) From {"ValuationExecutionTime", "CalibrationExecutionTime", "Value"}

                '9) Start Price IRS Trade
                '**************************************************************
#If True Then 'Asyncronous call
                '10) For Asyncronous call with a callback.
                '   Purpose:
                '   Call the value product once so that the model is placed in 
                '   in the external cache and ready for use by all workers in the
                '   F3 Platform. 
                '   This step is important and will speedup performance.
                '**************************************************************
                Dim slug = valueProduct(platform_url, valuation_model, portfolio, val_spec, requests, True, "http://127.0.0.1:61095/RestServiceImpl.svc/postJson")
#Else 'Syncronous call
                ' For syncronous call
                ' Send Value Product Call with polling resuls
                '**************************************************************
                Dim slug = valueProduct(platform_url, valuation_model, portfolio, val_spec, requests, True)
                While Utils.getInstance().getValuationResult(platform_url, slug) = False
                    Thread.Sleep(1)
                End While
#End If
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Debug.WriteLine(exception_str)
            End Try
        End Sub

    End Module

End Namespace