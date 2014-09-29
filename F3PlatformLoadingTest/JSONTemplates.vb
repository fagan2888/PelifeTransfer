Namespace fincad
    Public Class common_template
        Public Property name As String
        Public Property format As String = "f3ml"
        Public Property definition As String
        Public Property external_id As String = ""
        Public Property tags As List(Of tag)
    End Class
    Public Class tag
        Public Property category As String
        Public Property name As String
    End Class
    Public Class trade_argument
        Public Property trade_argument_type As String
        Public Property value As String
    End Class
    Public Class Model
        Public Property name As String
        Public Property format As String
        Public Property definition As String
        Public Property external_id As String
        Public Property tags As List(Of tag)
    End Class
    Public Class trade
        Public Property name As String
        Public Property trade_template As Object
        Public Property arguments As List(Of trade_argument)
        Public Property external_id As String
        Public Property tags As List(Of tag)
    End Class
    Public Class trade_valuation_template
        Public Property model As String
        Public Property trades As List(Of String)
        Public Property valuation_specification As String
        Public Property valuation_requests As List(Of String)
        Public Property expiry_time As String = ""
        Public Property callback_url As String = ""
        Public Property as_portfolio As Boolean = True
    End Class
    Public Class filters_template
        Public Property field As String
        Public Property operator_ As String
        Public Property value As String
        Public Property property_type As String
    End Class
    Public Class extractor_template
        Public Property field As String
        Public Property operator_ As String
        Public Property count As String
        Public Property property_type As String
    End Class
    Public Class selector_template
        Public Property name As String
        Public Property include_filters As List(Of filters_template)
        Public Property extractor As extractor_template
        Public Property external_id As String
        Public Property tags As List(Of tag)
    End Class
    Public Class index_fixings_template
        Public Property name As String
        Public Property f3name As String
        Public Property fixings_data As List(Of List(Of Object))
        Public Property external_id As String
        Public Property tags As List(Of tag)
    End Class
    Public Class fixing_requirements_template
        Public Property index As String
        Public Property required_days As Integer
    End Class
    Public Class model_recipe_template
        Public Property name As String
        Public Property external_id As String
        Public Property fixing_requirments As List(Of fixing_requirements_template)
        Public Property model_fragments As List(Of String)
        Public Property market_data_selectors As List(Of String)
        Public Property tags As List(Of tag)
    End Class
    Public Class valuation_model_definition_template
        Public Property model_recipe As String
        Public Property base_model As String
        Public Property valuation_date As String
    End Class
    Public Class valuation_model_from_recipe_template
        Public Property name As String
        Public Property format As String = "model_recipe"
        Public Property definition As valuation_model_definition_template
        Public Property external_id As String = ""
        Public Property tags As List(Of tag)
    End Class

End Namespace
