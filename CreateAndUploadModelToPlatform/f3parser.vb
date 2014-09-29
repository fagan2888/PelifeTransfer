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

    End Class
End Namespace
