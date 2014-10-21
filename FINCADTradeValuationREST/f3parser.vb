Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Namespace fincad
    Public Class f3parser
        Public Sub parseResult(ByRef obj As List(Of Object), f3_result_string As String)
            obj.Clear()
            'extract between <s> </s>
            Dim stringSeparators As String() = New String() {"<s>", "</s>", "<r>", "</r>", "<d>", "</d>", _
             "<D>", "</D>"}

            Dim result As String() = f3_result_string.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)

            For Each s As String In result
                obj.Add(If([String].IsNullOrEmpty(s), "<>", s))
            Next
        End Sub

        Public Sub parseResultRows(ByRef obj As List(Of Object), f3_result_string As String)
            'extract between <s> </s>
            Dim stringSeparators As String() = New String() {"<r>", "</r>"}

            Dim result As String() = f3_result_string.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)

            For Each s As String In result
                obj.Add(If([String].IsNullOrEmpty(s), "<>", s))
            Next
        End Sub

        Public Sub parseResultRows2(ByRef obj As List(Of Object), f3_result_string As String)
            'extract between <s> </s>
            Dim stringSeparators As String() = New String() {"<r>", "</r>", "<s>", "</s>", "<d>", "</d>", _
             "<D>", "</D>", ";", "&gt", "&lt"}

            Dim result As String() = f3_result_string.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)

            For Each s As String In result
                obj.Add(If([String].IsNullOrEmpty(s), "<>", s))
            Next
        End Sub

        Public Sub parseResultRows3(ByRef obj As List(Of Object), f3_result_string As String)
            'extract between <s> </s>
            Dim stringSeparators As String() = New String() {"<r>", "</r>", "<s>", "</s>", "<d>", "</d>", _
             "<D>", "</D>", ";", "&gt", "&lt"}

            parseResultRows2(obj, f3_result_string)

            Dim result As String() = f3_result_string.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)

            For Each s As String In result
                obj.Add(If([String].IsNullOrEmpty(s), "<>", s))
            Next
        End Sub

        Public Sub f3mlToHashTable(ByRef table As Hashtable, f3_result_string As String)
            table.Clear()
            'extract between <s> </s>
            Dim stringSeparators As String() = New String() {"<a>", "</a>", "<p>", "</p>"}
            Dim nameSeparators As String() = New String() {"<n>", "</n>"}
            Dim valueSeparators As String() = New String() {"<v>", "</v>"}

            Dim result As String() = f3_result_string.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
            table.Clear()
            For Each s As String In result
                If (s.IndexOf("<f>") = 0 Or _
                    s.IndexOf("</f>") = 0 Or _
                    s.IndexOf("<n>ProductName</n>") = 0 Or _
                    s.IndexOf("<n>ValuationMethod</n>") = 0) Then
                Else
                    table.Add(s.Split(nameSeparators, StringSplitOptions.RemoveEmptyEntries), s.Split(valueSeparators, StringSplitOptions.RemoveEmptyEntries))
                    Debug.WriteLine(s)
                End If
            Next
        End Sub

        Public Function AjustParameters(ByVal table As IDictionary(Of String, Object)) As IDictionary(Of String, Object)

            Dim value As String = String.Empty
            Dim returnValue As IDictionary(Of String, Object) = Nothing

            If table Is Nothing Then Return Nothing

            returnValue = New Dictionary(Of String, Object)

            For Each item In table

                value = String.Empty

                Select Case table(item.Key).[GetType]()
                    Case GetType(String)
                        value = String.Format("<r><s>{0}</s></r>", table(item.Key))
                    Case GetType(Double)
                        value = String.Format("<r><d>{0}</d></r>", table(item.Key))
                    Case GetType(Boolean)
                        Dim stringValue As String
                        If DirectCast(table(item.Key), Boolean) Then
                            stringValue = "T"
                        Else
                            stringValue = "F"
                        End If
                        value = String.Format("<r><b>{0}</b></r>", stringValue)
                    Case GetType(Integer)
                        value = String.Format("<r><d>{0:F1}</d></r>", table(item.Key))
                    Case GetType(DateTime)
                        value = String.Format("<r><D>{0}</D></r>", CType(table(item.Key), DateTime).ToString("yyyy-MM-dd"))
                    Case Else
                        value = String.Format("<r><s>{0}</s></r>", table(item.Key))
                End Select

                'table(item.Key) = value
                returnValue(item.Key) = value
            Next

            Return returnValue

        End Function




    End Class
End Namespace
