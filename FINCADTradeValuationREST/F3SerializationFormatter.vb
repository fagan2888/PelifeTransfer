Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Runtime.Serialization
Imports System.Reflection
Imports System.IO
Imports System.Collections.Specialized
Imports System.Collections



Namespace fincad


    Public Class F3Formatter
        Implements IFormatter
        Private m_binder As SerializationBinder
        Private m_context As StreamingContext
        Private m_surrogateSelector As ISurrogateSelector

        Public Sub New()
            m_context = New StreamingContext(StreamingContextStates.All)
        End Sub

        Public Function Deserialize(serializationStream As System.IO.Stream) As Object Implements IFormatter.Deserialize
            Dim sr As New StreamReader(serializationStream)

            ' Get Type from serialized data.
            Dim line As String = sr.ReadLine()
            Dim delim As Char() = New Char() {"="c}
            Dim sarr As String() = line.Split(delim)
            Dim className As String = sarr(1)
            Dim t As Type = Type.[GetType](className)

            ' Create object of just found type name.
            Dim obj As [Object] = FormatterServices.GetUninitializedObject(t)


            ' TODO: Use GetBetween Populate object values and return object.

            Return Nothing
            'FormatterServices.PopulateObjectMembers(obj, members, data);
        End Function

        ''' <summary>
        ''' F3 Function Call string builder
        ''' </summary>
        ''' <param name="function_name"></param>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Shared Function f3_style_serialization(function_name As String, obj As Object) As String

            Dim call_string As String = ""
            Dim f As New F3Formatter()
            Dim buffer As Byte() = New Byte(9999) {}

            '''///////////////////////////////////////////////
            ' 
            ' constructing f3 style xml function call string
            ' 
            '''///////////////////////////////////////////////

            Using f3_call_str = New MemoryStream(buffer)
                f.Serialize2(f3_call_str, function_name, obj)
                If f3_call_str.CanRead Then
                    Dim enc As New System.Text.UTF8Encoding()
                    call_string = enc.GetString(buffer, 0, CInt(f3_call_str.Position))
                End If
                f3_call_str.Close()
            End Using
            Return call_string
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="serializationStream"></param>
        ''' <param name="f3obj"></param>
        Public Sub Serialize(serializationStream As System.IO.Stream, f3obj As Object) Implements IFormatter.Serialize
        End Sub

        ''' <summary>
        ''' F3 Object Serialzation (Function call string builder)
        ''' </summary>
        ''' <param name="serializationStream"></param>
        ''' <param name="function_name"></param>
        ''' <param name="f3obj"></param>
        ''' 
        Public Sub Serialize2(serializationStream As System.IO.Stream, function_name As String, f3obj As Object)
            ' Get fields data.
            Dim properties As PropertyInfo() = f3obj.[GetType]().GetProperties()
            Dim sw As New StreamWriter(serializationStream)
            'TODO: write the <f><n>
            sw.Write([String].Format("<f><n>{0}</n><a>", function_name))


            If f3obj.[GetType]() Is GetType(Hashtable) Then
                Dim m_hashtable As New Hashtable()
                m_hashtable = DirectCast(f3obj, Hashtable)
                For Each name As String In m_hashtable.Keys
                    Console.WriteLine(name & vbTab & Convert.ToString(m_hashtable(name)))
                    If m_hashtable(name) Is Nothing Then
                        Dim temp As String = [String].Format("<p><n>{0}</n><v><r><m/></r></v></p>", name)
                        sw.Write(temp)
                    ElseIf m_hashtable(name).[GetType]() Is GetType(String) Then
                        Dim temp As String = [String].Format("<p><n>{0}</n><v><r><s>{1}</s></r></v></p>", name, m_hashtable(name))
                        sw.Write(temp)
                    ElseIf m_hashtable(name).[GetType]() Is GetType(Double) Then
                        Dim temp As String = [String].Format("<p><n>{0}</n><v><r><d>{1}</d></r></v></p>", name, m_hashtable(name))
                        sw.Write(temp)
                    ElseIf m_hashtable(name).[GetType]() Is GetType(Boolean) Then
                        Dim temp As String = [String].Format("<p><n>{0}</n><v><r><b>{1}</b></r></v></p>", name, m_hashtable(name))
                        sw.Write(temp)
                    ElseIf m_hashtable(name).[GetType]() Is GetType(Integer) Then
                        Dim temp As String = [String].Format("<p><n>{0}</n><v><r><e>{1}</e></r></v></p>", name, m_hashtable(name))
                        sw.Write(temp)
                    ElseIf m_hashtable(name).[GetType]() Is GetType(DateTime) Then
                        Dim temp As String = [String].Format("<p><n>{0}</n><v><r><D>{1}</D></r></v></p>", name, m_hashtable(name))
                        sw.Write(temp)
                    ElseIf m_hashtable(name).[GetType]() Is GetType(List(Of List(Of Object))) Then
                        Dim temp As String = [String].Format("<p><n>{0}</n><v>", name)
                        sw.Write(temp)

                        Dim objValue As Object = m_hashtable(name)

                        Dim ObjList As List(Of List(Of Object)) = TryCast(objValue, List(Of List(Of Object)))
                        Dim temp2 As String = [String].Format("<p><m/><v>")

                        For Each node As Object In ObjList
                            If node Is Nothing Then
                                temp2 = [String].Format("<r><m/></r>")
                            ElseIf node.[GetType]() Is GetType(String) Then
                                temp2 = [String].Format("<r><s>{0}</s></r>", node)
                            ElseIf node.[GetType]() Is GetType(Double) Then
                                temp2 = [String].Format("<r><d>{0}</d></r>", node)
                            ElseIf node.[GetType]() Is GetType(Boolean) Then
                                temp2 = [String].Format("<r><b>{0}</b></r>", node)
                            ElseIf node.[GetType]() Is GetType(Integer) Then
                                temp2 = [String].Format("<r><e>{0}</e></r>", node)
                            ElseIf node.[GetType]() Is GetType(DateTime) Then
                                temp2 = [String].Format("<r><D>{0}</D></r>", node)
                            ElseIf node.[GetType]() Is GetType(List(Of Object)) Then
                                sw.Write("<r>")
                                Dim ObjList1 As List(Of Object) = TryCast(node, List(Of Object))
                                For Each node1 As Object In ObjList1
                                    If node Is Nothing Then
                                        temp2 = [String].Format("<r><m/></r>")
                                    ElseIf node1.[GetType]() Is GetType(String) Then
                                        temp2 = [String].Format("<s>{0}</s>", node1)
                                    ElseIf node1.[GetType]() Is GetType(Double) Then
                                        temp2 = [String].Format("<d>{0}</d>", node1)
                                    ElseIf node1.[GetType]() Is GetType(Boolean) Then
                                        temp2 = [String].Format("<b>{0}</b>", node1)
                                    ElseIf node1.[GetType]() Is GetType(Integer) Then
                                        temp2 = [String].Format("<e>{0}</e>", node1)
                                    ElseIf node1.[GetType]() Is GetType(DateTime) Then
                                        temp2 = [String].Format("<D>{0}</D>", node1)
                                    End If
                                    sw.Write(temp2)
                                Next
                                sw.Write("</r>")
                            End If
                        Next
                        sw.Write("</v></p>")
                    ElseIf m_hashtable(name).[GetType]() Is GetType(List(Of Object)) Then
                        Dim temp As String = [String].Format("<p><n>{0}</n><v><r>", name)
                        sw.Write(temp)
                        Dim objValue As Object = m_hashtable(name)
                        Dim ObjList As List(Of Object) = TryCast(objValue, List(Of Object))
                        Dim temp2 As String = [String].Format("<p><m/><v>")
                        For Each node As Object In ObjList
                            If node Is Nothing Then
                                temp2 = [String].Format("<r><m/></r>")
                            ElseIf node.[GetType]() Is GetType(String) Then
                                temp2 = [String].Format("<s>{0}</s>", node)
                            ElseIf node.[GetType]() Is GetType(Double) Then
                                temp2 = [String].Format("<d>{0}</d>", node)
                            ElseIf node.[GetType]() Is GetType(Boolean) Then
                                temp2 = [String].Format("<b>{0}</b>", node)
                            ElseIf node.[GetType]() Is GetType(Integer) Then
                                temp2 = [String].Format("<e>{0}</e>", node)
                            ElseIf node.[GetType]() Is GetType(DateTime) Then
                                temp2 = [String].Format("<D>{0}</D>", node)
                            End If
                            sw.Write(temp2)
                        Next
                        sw.Write("</r></v></p>")
                    Else
                        Dim temp As String = [String].Format("<p><n>{0}</n><v><r><m/></r></v></p>", name)
                        sw.Write(temp)
                    End If
                Next
            End If

            sw.WriteLine("</a></f>")
            sw.Flush()
        End Sub

        Public Property SurrogateSelector() As ISurrogateSelector Implements IFormatter.SurrogateSelector
            Get
                Return m_surrogateSelector
            End Get
            Set(value As ISurrogateSelector)
                m_surrogateSelector = Value
            End Set
        End Property
        Public Property Binder() As SerializationBinder Implements IFormatter.Binder
            Get
                Return m_binder
            End Get
            Set(value As SerializationBinder)
                m_binder = Value
            End Set
        End Property
        Public Property Context() As StreamingContext Implements IFormatter.Context
            Get
                Return m_context
            End Get
            Set(value As StreamingContext)
                m_context = Value
            End Set
        End Property
    End Class

End Namespace
