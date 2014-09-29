Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Threading

Namespace fincad
    Module Module1

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

        ''' <summary>
        ''' This test program focus on testing F3 Platform's load balancing capability not performance
        ''' We want to simulate the enviornment where request is sent to the F3 Platform indefinately
        ''' and see if F3 Platform can levelage its workers to handle indefinate requests.
        ''' </summary>
        ''' <remarks>Althought this test is sufficient to test the F3 Platform load balancing feature. 
        ''' If time permitted, it is even better to properly configure F3 Platfrom to use an enterprise ready DBS (SQL Server or Oracle) and
        ''' simulate a even closer scenario where each valuation request is for a unique trade</remarks>
        '''

        Sub Main()

            '0) end point specification & specifiying valuation model,
            '    valuation spec, trade to use
            '**************************************************************

            Dim platform_url As String = "http://localhost:8505/"
            Dim callback_endpoint = "http://127.0.0.1:61095/RestServiceImpl.svc/postJson"

            Dim product_name = "irswap1_c"       'interest swap slug already stored in F3Platfrom
            Dim val_spec = "clean_valspec_c"     'valuation specification slug already stored in F3 Platform
            Dim valuation_model = "simpletestmodel-20140920002745229000" 'valuation model taken from the F3 Platform

            '1) Set the portfolio to a trade.
            '**************************************************************

            Dim portfolio = New List(Of String) From {product_name}

            '2) Create List of Requests
            '**************************************************************
            Dim requests = New List(Of String) From {"ValuationExecutionTime", "CalibrationExecutionTime", "Value"}

            Dim iteration = 1000000
            Dim correlation_id

            'the loop simulate the load for 1 million contineous requests
            'you can change it to an infinity loop to simulate contineous requests
            For i As Integer = 1 To iteration
                Try
                    '3) Send Value Product Call and the result will be sent to the
                    '   callback endpoint for asycronous call
                    '**************************************************************
                    correlation_id = valueProduct(platform_url, valuation_model, portfolio, val_spec, requests, True, callback_endpoint)
                    'Thread.Sleep(1000) 'simulate contineous indefinate reqeusts
                Catch ex As Exception
                    Debug.WriteLine(ex.ToString())
                End Try
            Next
        End Sub
    End Module
End Namespace
