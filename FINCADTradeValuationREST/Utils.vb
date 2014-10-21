Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization

Namespace fincad
    Public Structure Model_Response_Type
        Shared Definition As String = "definition"
        Shared name As String = "name"
        Shared created As String = "created"
        Shared validation_results As String = "validation_results"
        Shared format As String = "format"
        Shared modified As String = "modified"
        Shared valuation_date As String = "valuation_date"
        Shared validation_status As String = "validation_status"
        Shared external_id As String = "external_id"
        Shared slug As String = "slug"
        Shared tags As String = "tags"
    End Structure

    Public Structure Common_Response_Type
        Shared Definition As String = "definition"
        Shared version_number As String = "version_number"
        Shared name As String = "name"
        Shared arguments As String = "arguments"
        Shared created As String = "created"
        Shared Is_active As String = "is_active"
        Shared validation_results As String = "validation_results"
        Shared format As String = "format"
        Shared modified As String = "modified"
        Shared validation_status As String = "validation_status"
        Shared external_id As String = "external_id"
        Shared slug As String = "slug"
        Shared tags As String = "tags"
    End Structure

    Public Class Utils
        Private Shared objSingle As Utils
        Private Shared blCreated As Boolean

        Public Shared Function getInstance() As Utils
            If blCreated = False Then
                objSingle = New Utils()
                blCreated = True
                Return objSingle
            Else
                Return objSingle
            End If
        End Function

        ' F3 Platform uilities Functions
        '************************************************************************************
        Public Function GetServerResponse(data As Object, key As String) As Object
            Dim result As Object = "OK"
            Select Case key
                Case "definition"
                    Return data(key)
                Case "version_number"
                    Return data(key)
                Case "parameters"
                    Return data(key)
                Case "is_active"
                    Return data(key)
                Case "name"
                    Return data(key)
                Case "created"
                    Return data(key)
                Case "validation_results"
                    Return data(key)
                Case "format"
                    Return data(key)
                Case "modified"
                    Return data(key)
                Case "valuation_date"
                    Return data(key)
                Case "validation_status"
                    Return data(key)
                Case "external_id"
                    Return data(key)
                Case "slug"
                    Return data(key)
                Case "tags"
                    Return data(key)
                Case Else
                    result = "GetServerResponseItem: Invalid Input "
            End Select
            Return (result)
        End Function

        Public Function getValuationResult(platform_url As String, slug As String) As Boolean
            Dim result As Boolean = True
            Try
                Dim f3parserUtils As f3parser = New f3parser()

                ' serialize JASON objects to string
                '**************************************************************
                Dim serializer As New JavaScriptSerializer()

                Dim restAPI = "f3platform/api/v1/trade_valuation/" + slug + "?format=json"

                ' send REST Request to server
                '**************************************************************
                Dim data As Byte() = Encoding.UTF8.GetBytes("")
                Dim result_post = f3platformInterface.getInstance(platform_url).HttpGet(restAPI, "GET")

                ' deserialize server response (JSON string to object)
                '**************************************************************
                Dim deserializedResult As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(result_post)
                Dim res As Dictionary(Of String, Object) = deserializedResult("valuation_results")
                For Each item In res
                    Debug.WriteLine(item.Key)
                    For Each i In DirectCast(item.Value, Dictionary(Of String, Object))
                        Debug.WriteLine(i.Key)
                        For Each j In DirectCast(i.Value, Dictionary(Of String, Object))
                            If (j.Value.ToString().IndexOf("Not ready") = 0) Then
                                Return False
                            Else
                                Dim objList As New List(Of Object)()
                                f3parserUtils.parseResultRows2(objList, j.Value)
                                Debug.Write("OK " + j.Key + "===>")
                                For Each obj In objList
                                    Debug.Write(obj.ToString() + " ")
                                Next
                                Debug.WriteLine("")
                            End If
                        Next
                    Next
                Next
            Catch ex As Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Debug.WriteLine(exception_str)
                Throw ex
            End Try
            Return result
        End Function

    End Class
End Namespace

