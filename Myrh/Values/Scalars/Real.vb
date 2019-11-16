Imports Myrh.Entities

Namespace Values.Scalars

    <Value(Real.Match)>
    Public Class Real : Inherits Scalar(Of Real)

        Public Const Match As String = "[+-]?[0-9]+(\.[0-9]+)?(([eE][+-]?[0-9]+)|([ ]*[\*·×][ ]*10((\^[+-][0-9]+)|([⁻⁺]?[⁰¹²³⁴⁵⁶⁷⁸⁹]+))))?"
        Private ReadOnly _internal As Double

        Public Sub New()
            Me.New(0.0)
        End Sub

        Public Sub New(value As Double)
            MyBase.New()
            Me._internal = value
        End Sub

        Public Sub New(value As Double, unit As Unit)
            MyBase.New(unit)
            Me._internal = value
        End Sub

        Public Sub New(quantity As Quantity, value As Double)
            MyBase.New(quantity)
            Me._internal = value
        End Sub

        Public Sub New(quantity As Quantity, value As Double, unit As Unit)
            MyBase.New(quantity, unit)
            Me._internal = value
        End Sub

        Public Shared Widening Operator CType(value As Double) As Real
            Return New Real(value)
        End Operator

        Public Shared Narrowing Operator CType(value As Real) As Double
            If Not value.Unit.IsDimensionless Then Throw New InvalidCastException("Cannot cast entity to Double: Dimension is not one.")
            If Not value.Unit.IsDefault Then value = value.WithUnit(value.Unit.DefaultUnit)
            Return value._internal
        End Operator

        Public Shared Widening Operator CType(value As Real) As Complex
            Return New Complex(value.Quantity, value._internal, 0.0, value.Unit)
        End Operator

        Protected Overrides Function _Power(Of U As Scalar(Of U), V As Value(Of V))(exponent As U) As V
            If exponent.GetType.Equals(GetType(Real)) Then
                Dim r As Real = CTypeDynamic(Of Real)(exponent)
                Dim retval As New Real(Me.Quantity ^ r._internal, Me._internal ^ r._internal, Me.Unit ^ r._internal)
                Return CTypeDynamic(Of V)(retval)
            Else
                Return CTypeDynamic(Of U)(Me).Power(Of U, V)(exponent)
            End If
        End Function

        Protected Overrides Function _Add(Of U As Value(Of U), V As Value(Of V))(other As U) As V
            If other.GetType.Equals(GetType(Real)) Then
                Dim r As Real = CTypeDynamic(Of Real)(other)
                Dim retval As New Real(Me.Quantity, r._internal + Me._internal, Me.Unit)
                Return CTypeDynamic(Of V)(retval)
            Else
                Return other.Add(Of Scalar(Of Real), V)(Me)
            End If
        End Function

        Protected Overrides Function _FormatValue() As String
            Return Formatting.ToScientific(Me._internal)
        End Function

        Public Overrides Function Scale(Of U As Scalar(Of U), V As Value(Of V))(byValue As U) As V
            If byValue.GetType.Equals(GetType(Real)) Then
                Dim r As Real = CTypeDynamic(Of Real)(byValue)
                Dim retval As New Real(Me.Quantity * r.Quantity, r._internal * Me._internal, Me.Unit * r.Unit)
                Return CTypeDynamic(Of V)(retval)
            Else
                Return byValue.Scale(Of Real, V)(Me)
            End If
        End Function

        Protected Overrides Function MakeEntity(quantity As Quantity, link As Link) As Value
            Return New Real(quantity, Me._internal * link.Scaling, link.Target)
        End Function

        Public Overloads Shared Operator *(a As Real, b As Real) As Real
            Return a.Multiply(Of Real, Scalar(Of Real))(b)
        End Operator

        Public Overloads Shared Operator /(a As Real, b As Real) As Real
            Return a.Divide(Of Real, Scalar(Of Real))(b)
        End Operator

        Public Overloads Shared Operator +(a As Real, b As Real) As Real
            Return a.Add(Of Scalar(Of Real), Scalar(Of Real))(b)
        End Operator

        Public Overloads Shared Operator -(a As Real, b As Real) As Real
            Return a.Subtract(Of Scalar(Of Real), Scalar(Of Real))(b)
        End Operator

        Public Overloads Shared Operator ^(a As Real, b As Real) As Real
            Return a.Power(Of Real, Scalar(Of Real))(b)
        End Operator

        Public Function Absolute() As Real
            Return New Real(Me.Quantity, Math.Abs(Me._internal), Me.Unit)
        End Function

        Protected Overrides Function _Equals(Of U As Value(Of U))(other As U) As Boolean
            If other.GetType.Equals(GetType(Real)) Then
                Dim r As Real = CTypeDynamic(Of Real)(other)
                Return Me._internal = r._internal
            Else
                Return other.Equals(Of Scalar(Of Real))(Me)
            End If
        End Function

        Public Shared Operator =(a As Real, b As Real) As Boolean
            Return a.Equals(Of Scalar(Of Real))(b)
        End Operator

        Public Shared Operator <>(a As Real, b As Real) As Boolean
            Return Not a = b
        End Operator

        Public Shared Operator <(a As Real, b As Real) As Boolean
            Return a._internal < _MakeSimilar(a, b)._internal
        End Operator

        Public Shared Operator >(a As Real, b As Real) As Boolean
            Return a._internal > _MakeSimilar(a, b)._internal
        End Operator

        Private Shared Function _MakeSimilar(a As Real, b As Real) As Real
            If Not a.Quantity.Equals(b.Quantity) Then Throw New ArgumentException("Cannot compare entities with different quantities.")
            If a.Unit.Equals(b.Unit) Then Return a
            Return b.WithUnit(a.Unit)
        End Function

        Protected Overrides Function _Parse(text As String, quantity As Quantity, unit As Unit) As Object
            Dim v As Double = Formatting.FromScientific(text)
            Dim retval As Real = Nothing
            If quantity IsNot Nothing Then
                If unit IsNot Nothing Then
                    retval = New Real(quantity, v, unit)
                Else
                    retval = New Real(quantity, v)
                End If
            Else
                If unit IsNot Nothing Then
                    retval = New Real(v, unit)
                Else
                    retval = New Real(v)
                End If
            End If
            Return retval
        End Function

    End Class

End Namespace
