Namespace Entities.Model
    Public MustInherit Class Compound(Of T As Compound(Of T)) : Implements IEnumerable(Of Component(Of T)), IEquatable(Of Compound(Of T)), INamed

        Protected ReadOnly _components As Component(Of T)()
        Protected Shared ReadOnly _cache_alg As New Dictionary(Of String, T)

        Protected Sub New(components As IEnumerable(Of Component(Of T)))
            Me._components = components.GroupBy(
                Function(c) c.Base
            ).Select(
                Function(g) New Component(Of T)(g.Key, g.Sum(Function(c) c.Exponent))
            ).OrderBy(
                Function(c) c.Name
            ).ToArray
        End Sub

        Protected Shared Function _FilterEmpty(components As IEnumerable(Of Component(Of T)), empty As T) As IEnumerable(Of Component(Of T))
            If components.Count = 0 Then Return empty
            Dim retval As IEnumerable(Of Component(Of T)) = components
            If components.Any(Function(c) c.Base.Equals(empty)) Then
                If components.Any(Function(c) Not c.Base.Equals(empty)) Then
                    retval = components.Where(Function(c) Not c.Base.Equals(empty))
                Else
                    retval = empty
                End If
            End If
            Return retval
        End Function

        Protected Sub New(component As Atom(Of T), exponent As Double)
            Me.New({New Component(Of T)(component, exponent)})
        End Sub

        Protected Sub New(name As String, symbol As String)
            Me.New(New Atom(Of T)(name, symbol), 1.0)
        End Sub

        Public Function Simplified() As T
            Return Me.CreateNew(Me._components.Where(Function(c) c.Exponent <> 0))
        End Function

        Public ReadOnly Property Name As String Implements INamed.Name
            Get
                Return String.Join("·"c, Me._components.Select(Function(c) c.Name).ToArray)
            End Get
        End Property

        Public ReadOnly Property Symbol As String Implements INamed.Symbol
            Get
                Return String.Join("·"c, Me._components.Select(Function(c) c.Symbol).ToArray)
            End Get
        End Property

        Public Function GetEnumerator() As IEnumerator(Of Component(Of T)) Implements IEnumerable(Of Component(Of T)).GetEnumerator
            Return Me._components.AsEnumerable.GetEnumerator
        End Function

        Private Function IEquatable_Equals(other As Compound(Of T)) As Boolean Implements IEquatable(Of Compound(Of T)).Equals
            Return Me.Name = other.Name
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me.GetEnumerator
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim retval As Boolean = False
            If GetType(Compound(Of T)).IsAssignableFrom(obj.GetType) Then
                retval = Me.IEquatable_Equals(obj)
            Else
                retval = MyBase.Equals(obj)
            End If
            Return retval
        End Function

        Public Shared Operator *(a As Compound(Of T), b As Compound(Of T)) As T
            Dim key As String = "(" & a.ToString & ")*(" & b.ToString & ")"
            If _cache_alg.ContainsKey(key) Then
                Return _cache_alg(key)
            Else
                Dim retval As T = a.CreateNew(a._components.Concat(b._components))
                _cache_alg.Add(key, retval)
                Return retval
            End If
        End Operator

        Public Shared Operator ^(a As Compound(Of T), e As Double) As T
            Dim key As String = "(" & a.ToString & ")^" & e
            If _cache_alg.ContainsKey(key) Then
                Return _cache_alg(key)
            Else
                Dim retval As T = a.CreateNew(a.Select(Function(c) New Component(Of T)(c.Base, c.Exponent * e)))
                _cache_alg.Add(key, retval)
                Return retval
            End If
        End Operator

        Public Overrides Function ToString() As String
            Return Me.Name
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Me.Name.GetHashCode
        End Function

        Protected MustOverride Function CreateNew(components As IEnumerable(Of Component(Of T))) As T

        Public Shared Iterator Function Parse(text As String) As IEnumerable(Of Tuple(Of String, Double))
            Dim current As Integer = 0
            While Not current > text.Count - 1
                Yield _parse_component(text, current)
            End While
        End Function

        Private Shared Function _parse_component(text As String, ByRef position As Integer) As Tuple(Of String, Double)
            Dim retval As New Text.StringBuilder
            Dim super As String = "⁻⁺⁰˙¹²³⁴⁵⁶⁷⁸⁹"
            Dim super_map As String = "-+0.123456789"
            Dim f_map As Func(Of String, String) = Function(s) New String(s.Select(Function(c) super_map(super.IndexOf(c))).ToArray)
            Dim state As Integer = 0
            Dim name As String = ""
            Dim exp As Double = 1.0
            While Not position > text.Count - 1
                Dim current As Char = text(position)
                Select Case state
                    Case 0
                        Select Case current
                            Case "("
                                If retval.ToString.Contains("(") Then Throw New Exception("Multiple opening brackets not allowed in name.")
                                retval.Append("(")
                            Case ")"
                                If Not retval.ToString.StartsWith("(") Then Throw New Exception("Starting bracket not found.")
                                name = retval.ToString & ")"
                                If position = text.Count - 1 Then
                                    position += 1
                                    Exit While
                                ElseIf text(position + 1) = "^"c Then
                                    state = 1
                                    position += 1
                                    retval.Clear()
                                ElseIf super.Contains(text(position + 1)) Then
                                    state = 2
                                    retval.Clear()
                                Else
                                    If " *·".Contains(text(position + 1)) Then position += 1
                                    position += 1
                                    Exit While
                                End If
                            Case "^"
                                If retval.ToString.StartsWith("(") Then Throw New Exception("Closing bracket not found.")
                                name = retval.ToString
                                state = 1
                                retval.Clear()
                            Case " ", "*", "·"
                                If retval.ToString.StartsWith("(") And current = " " Then
                                    retval.Append(current)
                                Else
                                    If retval.ToString.StartsWith("(") Then Throw New Exception("Closing bracket not found.")
                                    name = retval.ToString
                                    position += 1
                                    Exit While
                                End If
                            Case Else
                                If super.Contains(current) Then
                                    If retval.ToString.StartsWith("(") Then Throw New Exception("Closing bracket not found.")
                                    name = retval.ToString
                                    state = 2
                                    position -= 1
                                    retval.Clear()
                                Else
                                    retval.Append(current)
                                End If
                        End Select
                        If position = text.Count - 1 Then name = retval.ToString
                    Case 1
                        If (IsNumeric(retval.ToString & current) Or "+-.".Contains(current)) And Not " *·(".Contains(current) Then
                            retval.Append(current)
                        Else
                            exp = Val(retval.ToString)
                            If Not " *·".Contains(current) Then position -= 1
                            position += 1
                            Exit While
                        End If
                        If position = text.Count - 1 Then exp = Val(retval.ToString)
                    Case 2
                        If super.Contains(current) Then
                            retval.Append(current)
                        Else
                            If Not " *·".Contains(current) Then position -= 1
                            position += 1
                            exp = Val(f_map(retval.ToString))
                            Exit While
                        End If
                        If position = text.Count - 1 Then exp = Val(f_map(retval.ToString))
                End Select
                position += 1
            End While
            Return New Tuple(Of String, Double)(name, exp)
        End Function

    End Class

End Namespace
