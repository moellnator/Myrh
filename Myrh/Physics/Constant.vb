Imports Myrh.Entities.Model
Imports Myrh.Values
Imports Myrh.Values.Scalars

Namespace Physics

    Public Class Constant : Implements INamed, IEquatable(Of Constant)

        Protected Shared ReadOnly _Constants As New Dictionary(Of String, Constant)
        Private Shared ReadOnly _ForbiddenName As New Text.RegularExpressions.Regex("[\w-[\P{Nd}]]*")
        Private Shared ReadOnly _ForbiddenSymbol As New Text.RegularExpressions.Regex("[\P{L}]*")

        Public ReadOnly Property Name As String Implements INamed.Name
        Public ReadOnly Property Symbol As String Implements INamed.Symbol
        Public ReadOnly Property Value As Object

        Protected Sub New(name As String, symbol As String, value As Object)
            If Not _ForbiddenName.IsMatch(name) Then Throw New ArgumentException($"Invalid name: {name}.")
            Me.Name = If(name <> "", "<" & name & ">", "")
            If Not _ForbiddenSymbol.IsMatch(symbol) Then Throw New ArgumentException($"Invalid symbol: {symbol}.")
            Me.Symbol = symbol
            Me.Value = value
        End Sub

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim retval As Boolean = False
            If GetType(Constant).IsAssignableFrom(obj.Getype) Then
                retval = Me.IEquatable_Equals(obj)
            Else
                retval = MyBase.Equals(obj)
            End If
            Return retval
        End Function

        Private Function IEquatable_Equals(other As Constant) As Boolean Implements IEquatable(Of Constant).Equals
            Return Me.Name = other.Name
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Me.Name.GetHashCode
        End Function

        Public Shared Sub Define(Of T As Value)(name As String, symbol As String, value As T)
            Dim retval As Constant = New Constant(name, symbol, value)
            _Constants.Add(name, retval)
        End Sub

        Public Shared Function Parse(text As String) As Constant
            Dim retval As Constant = Nothing
            If text.Contains("<") Then
                text = text.Trim("<"c, ">"c)
                If _Constants.ContainsKey(text) Then
                    retval = _Constants(text)
                Else
                    Throw New NotImplementedException
                End If
            Else
                retval = _Constants.Values.First(Function(c) c.Symbol = text)
            End If
            Return retval
        End Function

        Public Overrides Function ToString() As String
            Return Me.Name & " = " & Me.Value.ToString
        End Function

        Private Shared Function LevenshteinDistance(ByVal s As String, ByVal t As String) As Integer
            Dim n As Integer = s.Length
            Dim m As Integer = t.Length
            Dim d As Integer(,) = New Integer(n, m) {}
            If n = 0 Or m = 0 Then Return m
            Dim i As Integer = 0
            For i = 0 To n
                d(i, 0) = i
            Next
            For j = 0 To m
                d(0, j) = j
            Next
            For i = 1 To n
                For j = 1 To m
                    Dim cost As Integer = If((t(j - 1) = s(i - 1)), 0, 1)
                    d(i, j) = Math.Min(Math.Min(d(i - 1, j) + 1, d(i, j - 1) + 1), d(i - 1, j - 1) + cost)
                Next
            Next
            Return d(n, m)
        End Function

    End Class

End Namespace
