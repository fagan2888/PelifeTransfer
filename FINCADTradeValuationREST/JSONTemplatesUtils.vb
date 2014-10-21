Namespace fincad
    Public Class JSONTemplatesUtils

        Public Function tradeValuationConstruct(model As String, _
                                                trades As List(Of String),
                                                valuation_specification As String,
                                                valuation_requests As List(Of String),
                                                expiry_time As String,
                                                as_portfolio As Boolean,
                                                callback_url As String,
                                                ByRef trade_valuation_request As trade_valuation_template) As String
            Dim status As String = "OK"
            Try
                trade_valuation_request.model = model
                trade_valuation_request.trades = trades
                trade_valuation_request.valuation_specification = valuation_specification
                trade_valuation_request.valuation_requests = valuation_requests
                trade_valuation_request.expiry_time = expiry_time
                trade_valuation_request.as_portfolio = as_portfolio
                trade_valuation_request.callback_url = callback_url
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return exception_str
            End Try
            Return status
        End Function

        Public Function commonConstruct(name As String, _
                                        definition As String,
                                        ByRef out_common As common_template,
                                        Optional external_id As String = "",
                                        Optional ByRef tags_list As Object = "") As String
            Dim status As String = "OK"
            Try
                out_common.name = name
                out_common.definition = definition
                out_common.external_id = external_id
                out_common.tags = tags_list
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return exception_str
            End Try
            Return status
        End Function

        Public Function TradeConstruct(trade_name As String, external_id As String, trade_template As String, ByRef tags_list As Object, ByRef input_table As Hashtable, ByRef out_trade As trade) As String
            Dim status As String = "OK"
            Try
                out_trade.name = trade_name
                out_trade.trade_template = trade_template
                out_trade.arguments = New List(Of trade_argument)
                out_trade.tags = tags_list
                out_trade.external_id = external_id
                For Each Item In input_table
                    out_trade.arguments.Add(New trade_argument With {.trade_argument_type = Item.Key(0), .value = Item.Value(1)})
                Next
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return exception_str
            End Try
            Return status
        End Function

        Public Function TradeConstruct(trade_name As String, external_id As String, trade_template As String, ByRef tags_list As Object, ByRef input_table As IDictionary(Of String, Object), ByRef out_trade As trade) As String
            Dim status As String = "OK"
            Try
                out_trade.name = trade_name
                out_trade.trade_template = trade_template
                out_trade.arguments = New List(Of trade_argument)
                out_trade.tags = tags_list
                out_trade.external_id = external_id
                For Each Item In input_table
                    out_trade.arguments.Add(New trade_argument With {.trade_argument_type = Item.Key, .value = Item.Value})
                Next
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return exception_str
            End Try
            Return status
        End Function
    End Class
End Namespace
