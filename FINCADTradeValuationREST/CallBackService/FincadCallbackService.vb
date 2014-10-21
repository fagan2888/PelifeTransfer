Imports System.IO
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Web
Imports System.Text

<ServiceContract()> _
Public Interface IFincadCallbackService
    <OperationContract()> _
    <WebInvoke(Method:="POST", UriTemplate:="postJson")> _
    Sub Callback(ByVal inputStream As Stream)
End Interface

<ServiceBehavior(InstanceContextMode:=InstanceContextMode.Single)> _
Public Class FincadCallbackService
    Implements IFincadCallbackService

    Public Sub Callback(inputStream As Stream) Implements IFincadCallbackService.Callback

        Dim reader As StreamReader = New StreamReader(inputStream)
        Dim returnValue As String = reader.ReadToEnd()
        Dim queryString As String = HttpUtility.UrlDecode(returnValue)
        Dim queryStringValues As System.Collections.Specialized.NameValueCollection = HttpUtility.ParseQueryString(queryString, Encoding.UTF8)

        RaiseEvent CallBackExecuted(Me, New CallbackEventArgs With {.Results = queryStringValues})

        'Return DirectCast(queryStringValues(queryStringValues.Keys(1)), String)

    End Sub

    Public Event CallBackExecuted As EventHandler(Of CallbackEventArgs)

End Class

Public Class CallbackEventArgs
    Inherits EventArgs

    Public Property Results As System.Collections.Specialized.NameValueCollection

End Class
