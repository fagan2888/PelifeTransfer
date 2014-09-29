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

        Function SelectorConstruct(name As String, _
                                    ByRef filters As List(Of filters_template),
                                    ByRef extractor As extractor_template,
                                    ByRef out_selector As selector_template,
                                    Optional external_id As String = "",
                                    Optional ByRef tags_list As Object = "") As String
            Dim status As String = "OK"
            Try
                out_selector.name = name
                out_selector.include_filters = filters
                out_selector.extractor = extractor
                out_selector.external_id = external_id
                out_selector.tags = tags_list
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return exception_str
            End Try
            Return status
        End Function

        Function IndexFixingsConstruct(name As String, _
                                        ByRef index_fixings_template As index_fixings_template,
                                        index_f3_name As String,
                                        ByRef fixing_quotes As List(Of List(Of Object)),
                                        Optional external_id As String = "",
                                        Optional ByRef tags_list As Object = "") As String
            Dim status As String = "OK"
            Try
                index_fixings_template.name = name
                index_fixings_template.f3name = index_f3_name
                index_fixings_template.fixings_data = fixing_quotes
                index_fixings_template.external_id = external_id
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return exception_str
            End Try
            Return status
        End Function


        Public Function modelRecipeConstruct(name As String, _
                                            ByRef out_model_recipe As model_recipe_template,
                                            ByRef fixing_requirements As List(Of fixing_requirements_template),
                                            ByRef market_data_selectors As List(Of String),
                                            ByRef model_fragments As List(Of String),
                                            Optional external_id As String = "",
                                            Optional ByRef tags_list As Object = "") As String
            Dim status As String = "OK"
            Try
                out_model_recipe.name = name
                out_model_recipe.fixing_requirments = fixing_requirements
                out_model_recipe.market_data_selectors = market_data_selectors
                out_model_recipe.external_id = external_id
                out_model_recipe.model_fragments = model_fragments
                out_model_recipe.tags = tags_list
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return exception_str
            End Try
            Return status
        End Function


        Public Function valuationModelFromRecipeConstruct(valuation_model_name As String, _
                                                          ByRef out_valuation_model_from_recipe_template As valuation_model_from_recipe_template,
                                                          definition As valuation_model_definition_template,
                                                          Optional format As String = "model_recipe",
                                                          Optional external_id As String = "",
                                                          Optional ByRef tags_list As Object = "") As String
            Dim status As String = "OK"
            Try
                out_valuation_model_from_recipe_template.name = valuation_model_name
                out_valuation_model_from_recipe_template.format = format
                out_valuation_model_from_recipe_template.definition = definition
                out_valuation_model_from_recipe_template.external_id = external_id
                out_valuation_model_from_recipe_template.tags = tags_list
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return exception_str
            End Try
            Return status
        End Function
    End Class


End Namespace
