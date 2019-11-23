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
                retval = _Constants(text.Trim("<"c, ">"c))
            Else
                retval = _Constants.Values.First(Function(c) c.Symbol = text)
            End If
            Return retval
        End Function

        Public Overrides Function ToString() As String
            Return Me.Name & " = " & Me.Value.ToString
        End Function

    End Class

End Namespace
