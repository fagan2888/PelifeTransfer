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
End Namespace
