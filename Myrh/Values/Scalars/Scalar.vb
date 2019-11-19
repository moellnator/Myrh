Imports Myrh.Entities

Namespace Values.Scalars
    Public MustInherit Class Scalar(Of T As Scalar(Of T)) : Inherits Value(Of Scalar(Of T))

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

        Public Function Multiply(Of U As Scalar(Of U), V As Value(Of V))(other As U) As V
            Return other.Scale(Of T, V)(Me)
        End Function

        Public Overridable Function Divide(Of U As Scalar(Of U), V As Value(Of V))(other As U) As V
            Return Me.Scale(Of T, V)(other.Power(Of Real, Scalar(Of T))(-1))
        End Function

        Public Function Power(Of U As Scalar(Of U), V As Value(Of V))(exponent As U) As V
            If Not exponent.Unit.IsDimensionless Then Throw New ArgumentException("Exponent cannot have a dimension.")
            If Not exponent.Unit.IsDefault Then exponent.WithUnit(exponent.Unit.DefaultUnit)
            Return Me._Power(Of U, V)(exponent)
        End Function

        Protected MustOverride Function _Power(Of U As Scalar(Of U), V As Value(Of V))(exponent As U) As V

    End Class

End Namespace
