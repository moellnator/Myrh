Imports Myrh.Entities
Imports Myrh.Values.Scalars

Namespace Values

    Public MustInherit Class Value(Of T As Value(Of T)) : Inherits Entity(Of Value(Of T))

        Public Sub New()
            MyBase.New(Quantity.None)
        End Sub

        Public Sub New(unit As Unit)
            MyBase.New(unit)
        End Sub

        Public Sub New(quantity As Quantity)
            MyBase.New(quantity)
        End Sub

        Public Sub New(quantity As Quantity, unit As Unit)
            MyBase.New(quantity, unit)
        End Sub

        Public MustOverride Function Scale(Of U As Scalar(Of U), V As Value(Of V))(byValue As U) As V

        Public Function Add(Of U As Value(Of U), V As Value(Of V))(other As U) As V
            If Not Me.Unit.Dimension.Equals(other.Unit.Dimension) Then Throw New ArgumentException("Unable to sum over different quantities.")
            If Not Me.Unit.Equals(other.Unit) Then other = other.WithUnit(Me.Unit)
            Return Me._Add(Of U, V)(other)
        End Function

        Public Function Subtract(Of U As Value(Of U), V As Value(Of V))(other As U) As V
            Return Me._Add(Of U, V)(other.Scale(Of Real, U)(-1))
        End Function

        Protected MustOverride Function _Add(Of U As Value(Of U), V As Value(Of V))(other As U) As V

        Public Overrides Function ToString() As String
            Return If(Me.Quantity.Equals(Quantity.None), "", Me.Quantity.ToString & " ") & Me._FormatValue & If(Me.Unit.Equals(Unit.Empty), "", " " & Me.Unit.Symbol)
        End Function

        Protected MustOverride Function _FormatValue() As String

        Public Overloads Function Equals(Of U As Value(Of U))(other As U) As Boolean
            Return Me.Quantity.Equals(other.Quantity) AndAlso Me._Equals(Of U)(If(other.Unit.Equals(Me.Unit), other, other.WithUnit(Me.Unit)))
        End Function

        Protected MustOverride Function _Equals(Of U As Value(Of U))(other As U) As Boolean

    End Class

End Namespace
