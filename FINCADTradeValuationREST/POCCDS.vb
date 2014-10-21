Imports TradeValuation.fincad
Imports System.Web.Script.Serialization
Imports System.Text
Imports System.ServiceModel


Public Class POCCDS


    Public Shared ReadOnly Property PlatformURL() As String
        Get
            Return "http://RJ-RISC-0044:8505/"
        End Get
    End Property

    Public Shared ReadOnly Property CallbackURL() As String
        Get
            Return "http://rj-risc-0014:49924/postJson"
        End Get
    End Property

    Shared serviceHost As ServiceHost = Nothing

    Private Shared Sub OpenCallbackHandler()

        Dim service As New FincadCallbackService()
        AddHandler service.CallBackExecuted, AddressOf POCCDS_CallBackExecuted

        serviceHost = New ServiceHost(service)

        serviceHost.Open()
        Console.WriteLine("Service running.")

    End Sub

    Private Shared Sub CloseCallbackHandler()

        If Not serviceHost Is Nothing Then
            serviceHost.Close()
            Console.WriteLine("Service stopped.")
        End If

    End Sub

    Public Shared Sub POCCDS_CallBackExecuted(ByVal sender As Object, args As CallbackEventArgs)

        Dim x = args.Results

    End Sub

    Private Shared Function LoadModel() As String

        Dim restAPI As String = "f3platform/api/v1/model/"
        Dim nomeArquivoFuncFile As String = "func_files\modelo_azambuja_cds.func"
        Dim caminhoArquivoFuncFile As String = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeArquivoFuncFile)
        Dim contentBuilder As Text.StringBuilder = Nothing

        'F3 specific
        Dim modelExport As Model = Nothing
        Dim listTags As List(Of tag) = Nothing
        Dim serializer As JavaScriptSerializer
        Dim serializedResult As String = String.Empty
        Dim serializedResultData As Byte() = Nothing
        Dim deserializedResult As Dictionary(Of String, Object) = Nothing

        'network specific
        Dim postResult As String = String.Empty

        If IO.File.Exists(caminhoArquivoFuncFile) Then
            Using reader As IO.StreamReader = IO.File.OpenText(caminhoArquivoFuncFile)
                contentBuilder = New Text.StringBuilder()
                contentBuilder.Append(reader.ReadToEnd())
            End Using
        End If

        serializer = New JavaScriptSerializer
        modelExport = New Model()

        Try

            modelExport.definition = contentBuilder.ToString()
            modelExport.name = "modelo_azambuja_cds"
            modelExport.format = "f3ml"
            modelExport.external_id = "modelo_azamba_cds"

            listTags = New List(Of tag)() From { _
                                                New tag() With {.category = "Azambuja model", .name = "USD"}, _
                                                New tag() With {.category = "CDS model", .name = "USD"} _
                                                }
            modelExport.tags = listTags

            serializedResult = serializer.Serialize(modelExport)
            serializedResultData = Encoding.UTF8.GetBytes(serializedResult)

            ' send REST Request to server
            '**************************************************************
            postResult = f3platformInterface.getInstance(PlatformURL).SendRESTRequest(restAPI, serializedResultData, "application/json", "POST")

            ' deserialize server response (JSON string to object)
            '**************************************************************
            deserializedResult = serializer.Deserialize(Of Dictionary(Of String, Object))(postResult)
            Return Utils.getInstance().GetServerResponse(deserializedResult, Model_Response_Type.slug)

        Catch ex As Exception
            Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
            Debug.WriteLine(exception_str)
            Throw ex
        End Try

        Return "Error"
    End Function

    Private Shared Function CreateCDSTrade(ByVal Entidade As String, _
                                           ByVal CodigoContrato As String, _
                                           ByVal DataInicio As DateTime, _
                                           ByVal Maturity As Object, _
                                           ByVal Notional As Object, _
                                           ByVal Currency As String, _
                                           ByVal Premium As Double, _
                                           ByVal Upfrontfee As Double, _
                                           ByVal PayAccruedInterestUponDefault As Boolean, _
                                           ByVal MarketConvention As String, _
                                           ByVal WeightDefaultUnit As Double, _
                                           ByVal WeightDefaultRecovery As Double, _
                                           ByVal PayRec As String, _
                                           ByVal OffsettingMarketConvention As String) As String

        Dim CDSTrade As StringBuilder = Nothing
        Dim FBuilder As F3XMLFunctionsBuilder = Nothing

        Dim EntidadeCredito As String = String.Format("{0}Credit", Entidade)
        Dim SingleNameCreditContract As String = String.Format("{0}Contract", Entidade)

        CDSTrade = New StringBuilder
        FBuilder = New F3XMLFunctionsBuilder()

        CDSTrade.Append(FBuilder.CreateEntity(Entidade, ""))
        CDSTrade.Append(FBuilder.CreateCreditEntity(EntidadeCredito, Entidade, ""))
        CDSTrade.Append(FBuilder.CreateSingleNameCreditContract(SingleNameCreditContract, EntidadeCredito))
        CDSTrade.Append(FBuilder.CreateSingleEventCreditDefaultSwap(CodigoContrato, _
                                                                    DataInicio.ToOADate, _
                                                                   Maturity, _
                                                                   Notional, _
                                                                   Currency, _
                                                                   SingleNameCreditContract, _
                                                                   Premium, _
                                                                   Upfrontfee, _
                                                                   PayAccruedInterestUponDefault,
                                                                    MarketConvention, _
                                                                    WeightDefaultUnit, _
                                                                    WeightDefaultRecovery, _
                                                                    PayRec, _
                                                                    OffsettingMarketConvention))

        Return CDSTrade.ToString()

    End Function

    Private Shared Function LoadTrade(ByVal productName As String) As String

        Dim newCDSTrade As String = String.Empty

        newCDSTrade = CreateCDSTrade("ItalianGovt", _
                                     "CDS914466", _
                                    New DateTime(2014, 10, 6), _
                                    "5y", _
                                    "60mio", _
                                    "USD", _
                                    0.02, _
                                    0.05, _
                                    True, _
                                    "SwapUSD3m", _
                                    1, _
                                    -1, _
                                    "Rec", _
                                    "SwapUSDAnnual")

        Dim restAPI As String = "f3platform/api/v1/trade/"

        'F3 specific
        Dim tradeExport As trade = Nothing
        Dim listTags As List(Of tag) = Nothing
        Dim serializer As JavaScriptSerializer
        Dim serializedResult As String = String.Empty
        Dim serializedResultData As Byte() = Nothing
        Dim deserializedResult As Dictionary(Of String, Object) = Nothing
        Dim parameters As Hashtable = Nothing
        Dim parser As f3parser
        Dim json_util As JSONTemplatesUtils

        'network specific
        Dim postResult As String = String.Empty


        serializer = New JavaScriptSerializer
        tradeExport = New trade
        parameters = New Hashtable
        parser = New f3parser
        json_util = New JSONTemplatesUtils


        Try

            Dim tags_list As List(Of tag) = _
                  New List(Of tag)() From { _
                                          New tag() With {.category = "Trade", .name = "BTG CDS"}, _
                                          New tag() With {.category = "Portfolio", .name = "TESTE"} _
                                          }



            parser.f3mlToHashTable(parameters, newCDSTrade)

            Dim tradeParameters As IDictionary(Of String, Object) = New Dictionary(Of String, Object)

            tradeParameters("EntityName") = "ItalianGovt"
            tradeParameters("LegalName") = ""
            tradeParameters("CreditEntityName") = "ItalianGovtCredit"
            tradeParameters("Entity") = "ItalianGovt"
            tradeParameters("AgreementType") = ""
            tradeParameters("ContractName") = "ItalianGovtContract"
            tradeParameters("CreditEntity") = "ItalianGovtCredit"
            tradeParameters("ProductName") = productName
            tradeParameters("StartDate") = New DateTime(2014, 10, 14)
            tradeParameters("Maturity") = "5y"
            tradeParameters("Notional") = CType(60000000.0, Double)
            tradeParameters("Currency") = "USD"
            tradeParameters("CreditContract") = "ItalianGovtContract"
            tradeParameters("Premium") = CType(0.02, Double)
            tradeParameters("UpfrontFee") = CType(0.05, Double)
            tradeParameters("PayAccruedInterestUponDefault") = True
            tradeParameters("MarketConvention") = "SwapUSD3m"
            tradeParameters("WeightDefaultUnit") = 1
            tradeParameters("WeightDefaultRecovery") = -1
            tradeParameters("PayRec") = "Rec"
            tradeParameters("OffsettingMarketConvention") = "SwapUSDAnnual"

            tradeParameters = parser.AjustParameters(tradeParameters)

            'json_util.TradeConstruct("irs_d", "fincad_trade", "vanilla-interest-rate-swap", tags_list, table, trade)
            json_util.TradeConstruct(productName, productName + "_EXT", "cds4", tags_list, tradeParameters, tradeExport)

            serializedResult = serializer.Serialize(tradeExport)
            serializedResultData = Encoding.UTF8.GetBytes(serializedResult)

            ' send REST Request to server
            '**************************************************************
            'postResult = f3platformInterface.getInstance(PlatformURL).SendRESTRequest(restAPI, serializedResultData, "application/json", "POST")
            postResult = f3platformInterface.getInstance(PlatformURL).SendRESTRequest(restAPI, serializedResult, "application/json", "POST")

            ' deserialize server response (JSON string to object)
            '**************************************************************
            deserializedResult = serializer.Deserialize(Of Dictionary(Of String, Object))(postResult)

            Return Utils.getInstance().GetServerResponse(deserializedResult, Model_Response_Type.slug)

        Catch ex As Exception
            Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
            Debug.WriteLine(exception_str)
            Throw ex
        End Try

        Return "Error"

    End Function

    Shared Function valueProduct(platform_url As String, model As String, trades As List(Of String), valspec As String, requests As List(Of String), as_portfolio As Boolean, Optional callback_endpoint As String = "") As String
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

            start_date = start_date.Add(duration)
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
            Dim result_post = f3platformInterface.getInstance(platform_url).SendRESTRequest(restAPI, serializedResult, "application/json", "POST")

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

    Shared Function createValuationSpecification(platform_url As String) As String
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

    Public Shared Sub Execute()

        Dim productName As String = String.Empty
        productName = "CDS9144800" '"CDS914470_EXT"

        OpenCallbackHandler()
        Dim model = LoadModel()
        Dim spec = createValuationSpecification(PlatformURL)
        LoadTrade(productName)

        'Dim valuation_model = createModel(platform_url)
        'Dim irs_trade = createTrade(platform_url)
        'Dim val_spec = createValuationSpecification(platform_url)
        Dim requests = New List(Of String) From {"ValuationExecutionTime", "CalibrationExecutionTime", "Value"}
        Dim portfolio = New List(Of String) From {productName.ToLower()}

        valueProduct(PlatformURL, "modelo_azambuja_cds", portfolio, spec, requests, True, CallbackURL)

        Console.WriteLine("Finished POCCDS. Press <Enter> to exit.")
        Console.ReadLine()

        CloseCallbackHandler()
    End Sub



End Class
