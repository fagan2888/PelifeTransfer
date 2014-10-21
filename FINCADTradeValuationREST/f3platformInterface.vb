Imports System
Imports System.Text
Imports System.Net
Imports System.IO
Imports System.Web

Namespace fincad
    Public Class f3platformInterface
        Private Shared objSingle As f3platformInterface
        Private Shared blCreated As Boolean
        Public strI As String

        Private _g_uri As String

        ' property g_uri
        Public Property g_uri() As String
            Get
                Return _g_uri.ToString
            End Get
            Set(ByVal value As String)
                _g_uri = value
            End Set
        End Property ' g_uri

        Private Sub New(ByRef uri As String)
            'Override the default constructor
            _g_uri = uri
        End Sub

        Public Shared Function getInstance(ByRef uri As String) As f3platformInterface
            If blCreated = False Then
                objSingle = New f3platformInterface(uri)
                blCreated = True
                Return objSingle
            Else
                Return objSingle
            End If
        End Function

        Public Function HttpGet(ByRef api As String, method As String) As String
            Dim req As WebRequest = WebRequest.Create(g_uri + api)
            req.Proxy = New WebProxy("localproxyIP:8505", True) 'true means no proxy
            req.Method = method
            Dim resp As HttpWebResponse = CType(req.GetResponse(), HttpWebResponse)
            Console.WriteLine(resp.StatusDescription)
            Dim dataStream As Stream = resp.GetResponseStream()
            ' Open the stream using a StreamReader for easy access. 
            Dim reader As New StreamReader(dataStream)
            ' Read the content. 
            Dim responseFromServer As String = reader.ReadToEnd().Trim()
            reader.Close()
            dataStream.Close()
            resp.Close()
            Return responseFromServer
        End Function

        Public Function f3PlatRPC(ByRef api As String, ByRef method As String, Optional data As String = "", Optional ContentType As String = "application/x-www-form-urlencoded", Optional Accept As String = "application/x-www-form-urlencoded") As String
            Dim responseFromServer As String = ""
            Try
                If (method <> "POST") Then
                    Return HttpGet(api, method)
                Else
                    Dim httpWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(g_uri + api), HttpWebRequest)
                    httpWebRequest.ContentType = ContentType
                    httpWebRequest.Accept = Accept
                    httpWebRequest.Method = method
                    httpWebRequest.ContentLength = data.Length

                    Dim postByteArray() As Byte = Encoding.UTF8.GetBytes(data)
                    Dim postStream As IO.Stream = httpWebRequest.GetRequestStream()
                    postStream.Write(postByteArray, 0, postByteArray.Length)
                    postStream.Close()

                    Dim resp As HttpWebResponse = CType(httpWebRequest.GetResponse(), HttpWebResponse)
                    Console.WriteLine(resp.StatusDescription)
                    Dim dataStream As Stream = resp.GetResponseStream()
                    ' Open the stream using a StreamReader for easy access. 
                    Dim reader As New StreamReader(dataStream)
                    ' Read the content. 
                    responseFromServer = reader.ReadToEnd().Trim()
                    reader.Close()
                    dataStream.Close()
                    resp.Close()
                End If
            Catch e As Exception
                responseFromServer = "An error occurred: " & e.Message
            End Try
            Return responseFromServer
        End Function

        Public Function SendRESTRequest(api As String, jsonDataBytes As Byte(), contentType As String, method As String) As String
            Dim responseFromServer As String = ""
            Try
                Dim req As WebRequest = WebRequest.Create(g_uri + api)
                req.ContentType = contentType
                req.Method = method
                req.ContentLength = jsonDataBytes.Length
                Dim stream = req.GetRequestStream()
                stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
                stream.Close()

                Dim response = req.GetResponse() '.GetResponseStream()

                Dim reader As New StreamReader(response.GetResponseStream())
                responseFromServer = reader.ReadToEnd()
                reader.Close()
                response.Close()
            Catch e As Exception
                responseFromServer = "An error occurred: " & e.Message
            End Try
            Return responseFromServer
        End Function

        Public Function SendRESTRequest(api As String, jsonString As String, contentType As String, method As String) As String

            Dim RequestObject As Object = Nothing 
            Dim RequestURL As String = String.Empty
            Dim RequestResponseStatus As Integer = 0
            Dim RequestResponseContent As String = String.Empty

            RequestURL = g_uri + api
            RequestObject = CreateObject("WinHttp.WinHttpRequest.5.1")

            Try

                RequestObject.Open(method, RequestURL, False)
                RequestObject.SetRequestHeader("Content-Type", contentType)
                RequestObject.Send(jsonString)

                RequestResponseStatus = RequestObject.status
                RequestResponseContent = RequestObject.responseText
            Catch e As Exception
                RequestResponseContent = "An error occurred: " & e.Message
            End Try

            Return RequestResponseContent

        End Function

        Public Function executef3ml(ByRef callstring As String, Optional ByRef session_id As String = "") As String
            Dim responseFromServer As String = ""
            Try
                Dim encodedStr As String
                encodedStr = HttpUtility.UrlEncode(callstring).Trim()
                Dim data As String = "f3ml=" + encodedStr + "&session=" + session_id
                responseFromServer = f3PlatRPC("executef3ml", "POST", data)
            Catch e As Exception
                responseFromServer = "An error occurred: " & e.Message
            End Try
            Return responseFromServer
        End Function

        Public Function ExportModel(file_name As String, Optional session_id As String = "") As String
            Try

                ' create an instance of F3 manager object with the license and version infomation

                Dim output_str As String = ""
                Dim sessionID As String = session_id
                Dim callstring As String = ""
                Dim encodedStr As String = ""
                Dim data As String = ""
                Dim fc As New F3XMLFunctionsBuilder()

                '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                'assuming that F3 Platform have access to the given "file_name"
                '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                callstring = fc.ImportObjects(file_name, Nothing).ToString()
                encodedStr = HttpUtility.UrlEncode(callstring).Trim()
                data = "f3ml=" & encodedStr & "&session=" & session_id
                ' call f3 engine for each line to build the instrument
                Dim result As String = f3PlatRPC("executef3ml", "POST", data)
                output_str = result
                Return output_str
            Catch ex As System.Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return (exception_str)
            End Try
        End Function

        Public Function GetSessionID() As String
            Try
                Dim output_str As String = ""
                    ' call f3 engine for each line to build the instrument
                Dim result As String = f3PlatRPC("startsession", "POST")
                    output_str = result
                Return output_str
            Catch ex As System.Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return (exception_str)
            End Try
        End Function

        Public Function DeleteSessionID(session_id As String) As String
            Try
                ' create an instance of F3 manager object with the license and version infomation

                Dim api As String = "endsession"
                Dim data As String = ""
                Dim output_str As String = ""

                data = "id=" & session_id
                ' call f3 engine for each line to build the instrument

                Dim result As String = f3PlatRPC(api, "POST", data)
                output_str = result
                Return output_str
            Catch ex As System.Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return (exception_str)
            End Try
        End Function

        Public Function GetStatus() As String
            Try
                Dim output_str As String = ""
                ' call f3 engine for each line to build the instrument
                Dim result As String = f3PlatRPC("status", "GET")
                output_str = result
                Return output_str
            Catch ex As System.Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return (exception_str)
            End Try
        End Function

        Public Function ListSessions() As String
            Try
                Dim output_str As String = ""
                ' call f3 engine for each line to build the instrument
                Dim result As String = f3PlatRPC("listsessions", "GET")
                output_str = result
                Return output_str
            Catch ex As System.Exception
                Dim exception_str As String = ex.[GetType]().ToString() & " from " & ex.Source & " object. " & ex.Message
                Return (exception_str)
            End Try
        End Function

    End Class
End Namespace