Imports Myrh.Entities

Namespace Values.Scalars

    <Value(Complex.Match)>
    Public Class Complex : Inherits Scalar(Of Complex)

        Public Const Match As String = "((\(" & Real.Match & "[ ]*[+-][ ]*" & Real.Match & "[ ]*[\*·]?[i𝑖]\))|(" & Real.Match & "[ ]*[\*·]?[i𝑖]" & "))"
        Private ReadOnly _real As Double
        Private ReadOnly _imag As Double

        Public ReadOnly Property Real As Real
            Get
                Return New Real(Me.Quantity, Me._real, Me.Unit)
            End Get
        End Property

        Public ReadOnly Property Imaginary As Real
            Get
                Return New Real(Me.Quantity, Me._imag, Me.Unit)
            End Get
        End Property

        Public Sub New()
            Me.New(0.0, 0.0)
        End Sub

        Public Sub New(real As Double, imaginary As Double)
            MyBase.New()
            Me._real = real
            Me._imag = imaginary
        End Sub

        Public Sub New(c As Complex)
            Me.New(c.Real, c.Imaginary)
        End Sub

        Public Sub New(real As Double, imaginary As Double, unit As Unit)
            MyBase.New(unit)
            Me._real = real
            Me._imag = imaginary
        End Sub

        Public Sub New(c As Complex, unit As Unit)
            Me.New(c.Real, c.Imaginary, unit)
        End Sub

        Public Sub New(quantity As Quantity, real As Double, imaginary As Double)
            MyBase.New(quantity)
            Me._real = real
            Me._imag = imaginary
        End Sub

        Public Sub New(quantity As Quantity, c As Complex)
            Me.New(quantity, c.Real, c.Imaginary)
        End Sub

        Public Sub New(quantity As Quantity, real As Double, imaginary As Double, unit As Unit)
            MyBase.New(quantity, unit)
            Me._real = real
            Me._imag = imaginary
        End Sub

        Public Sub New(quantity As Quantity, c As Complex, unit As Unit)
            Me.New(quantity, c.Real, c.Imaginary, unit)
        End Sub

        Public Shared Widening Operator CType(value As Double) As Complex
            Return New Complex(value, 0.0)
        End Operator

        Public Shared Widening Operator CType(value As Numerics.Complex) As Complex
            Return New Complex(value.Real, value.Imaginary)
        End Operator

        Public Shared Narrowing Operator CType(value As Complex) As Real
            If Not value._imag = 0 Then Throw New InvalidCastException("Cannot cast imaginary value to real.")
            Return New Real(value.Quantity, value._real, value.Unit)
        End Operator

        Public Shared Narrowing Operator CType(value As Complex) As Double
            If Not value.Unit.IsDimensionless Then Throw New InvalidCastException("Cannot cast entity to Double: Dimension is not one.")
            If Not value.Unit.IsDefault Then value = value.WithUnit(value.Unit.DefaultUnit)
            Return CType(value, Real)
        End Operator

        Public Shared Narrowing Operator CType(value As Complex) As Numerics.Complex
            If Not value.Unit.IsDimensionless Then Throw New InvalidCastException("Cannot cast entity to Double: Dimension is not one.")
            If Not value.Unit.IsDefault Then value = value.WithUnit(value.Unit.DefaultUnit)
            Return New Numerics.Complex(value._real, value._imag)
        End Operator

        Public Overrides Function Scale(Of U As Scalar(Of U), V As Value(Of V))(byValue As U) As V
            Select Case True
                Case byValue.GetType.Equals(GetType(Complex))
                    Dim c As Complex = CTypeDynamic(Of Complex)(byValue)
                    Return CTypeDynamic(Of V)(New Complex(Me.Quantity * c.Quantity, Me._real * c._real - Me._imag * c._imag, Me._real * c._imag + Me._imag * c._real, Me.Unit * c.Unit))
                Case byValue.GetType.Equals(GetType(Real))
                    Return Me.Scale(Of Complex, V)(CTypeDynamic(Of Complex)(byValue))
                Case Else
                    Return byValue.Scale(Of Complex, V)(Me)
            End Select
        End Function

        Protected Overrides Function _Power(Of U As Scalar(Of U), V As Value(Of V))(exponent As U) As V
            Select Case True
                Case exponent.GetType.Equals(GetType(Real))
                    Dim r As Double = CTypeDynamic(Of Real)(exponent)
                    Dim retval As Complex = Me
                    If r = 0 Then
                        retval = New Complex(Me.Quantity ^ 0, 1.0, 0.0, Me.Unit ^ 0)
                    ElseIf r = -1 Then
                        Return CTypeDynamic(Of V)(Me.Inverted)
                    Else
                        If r < 0 Then
                            retval = retval.Inverted
                            r = Math.Abs(r)
                        End If
                        Dim rad As Double = (retval._real ^ 2 + retval._imag ^ 2) ^ (r / 2)
                        Dim phi As Double = Math.Atan2(retval._imag, retval._real) * r
                        retval = New Complex(retval.Quantity ^ r, rad * Math.Cos(phi), rad * Math.Sin(phi), retval.Unit ^ r)
                    End If
                    Return CTypeDynamic(Of V)(retval)
                Case Else
                    Return CTypeDynamic(Of U)(Me).Power(Of U, V)(exponent)
            End Select
        End Function

        Public Function Inverted() As Complex
            Dim l As Double = Me._real ^ 2 + Me._imag ^ 2
            Return New Complex(Me.Quantity ^ -1, Me._real / l, -Me._imag / l, Me.Unit ^ -1)
        End Function

        Public Function Conjugated() As Complex
            Return New Complex(Me.Quantity, Me._real, -Me._imag, Me.Unit)
        End Function

        Public ReadOnly Property Magnitude() As Real
            Get
                Return New Real(Me.Quantity, Me._real ^ 2 + Me._imag ^ 2, Me.Unit)
            End Get
        End Property

        Public ReadOnly Property Phase() As Real
            Get
                Return New Real(Math.Atan2(Me._imag, Me._real))
            End Get
        End Property

        Protected Overrides Function _Add(Of U As Value(Of U), V As Value(Of V))(other As U) As V
            Select Case True
                Case other.GetType.Equals(GetType(Complex))
                    Dim c As Complex = CTypeDynamic(Of Complex)(other)
                    Return CTypeDynamic(Of V)(New Complex(Me.Quantity, Me._real + c._real, Me._imag + c._imag, Me.Unit))
                Case other.GetType.Equals(GetType(Real))
                    Return Me.Add(Of Scalar(Of Complex), V)(CTypeDynamic(Of Complex)(other))
                Case Else
                    Return other.Add(Of Scalar(Of Complex), V)(Me)
            End Select
        End Function

        Protected Overrides Function _FormatValue() As String
            If Me._real = 0 Then
                If Me._imag = 0 Then
                    Return "0"
                Else
                    Return Formatting.ToScientific(Me._imag) & "𝑖"
                End If
            Else
                If Me._imag = 0 Then
                    Return Formatting.ToScientific(Me._real)
                Else
                    Return "(" & Formatting.ToScientific(Me._real) & " " & If(Math.Sign(Me._imag) = "1", "+", "-") & " " &
                        Formatting.ToScientific(Math.Abs(Me._imag)) & "𝑖)"
                End If
            End If
        End Function

        Protected Overrides Function MakeEntity(quantity As Quantity, link As Link) As Value
            Return New Complex(Me.Quantity, Me._real * link.Scaling, Me._imag * link.Scaling, link.Target)
        End Function

        Public Overloads Shared Operator +(a As Complex, b As Complex) As Complex
            Return a.Add(Of Scalar(Of Complex), Scalar(Of Complex))(b)
        End Operator

        Public Overloads Shared Operator -(a As Complex, b As Complex) As Complex
            Return a.Subtract(Of Scalar(Of Complex), Scalar(Of Complex))(b)
        End Operator

        Public Overloads Shared Operator *(a As Complex, b As Complex) As Complex
            Return a.Multiply(Of Complex, Scalar(Of Complex))(b)
        End Operator

        Public Overloads Shared Operator /(a As Complex, b As Complex) As Complex
            Return a.Divide(Of Complex, Scalar(Of Complex))(b)
        End Operator

        Public Overloads Shared Operator ^(a As Complex, b As Real) As Complex
            Return a.Power(Of Real, Scalar(Of Complex))(b)
        End Operator

        Protected Overrides Function _Equals(Of U As Value(Of U))(other As U) As Boolean
            If other.GetType.Equals(GetType(Complex)) Then
                Dim r As Complex = CTypeDynamic(Of Complex)(other)
                Return Me._real = r._real And Me._imag = r._imag
            Else
                Return other.Equals(Of Scalar(Of Complex))(Me)
            End If
        End Function

        Public Shared Operator =(a As Complex, b As Complex) As Boolean
            Return a.Equals(Of Scalar(Of Complex))(b)
        End Operator

        Public Shared Operator <>(a As Complex, b As Complex) As Boolean
            Return Not a = b
        End Operator

        Protected Overrides Function _Parse(text As String, quantity As Quantity, unit As Unit) As Object
            Dim imag As Double = 0
            Dim real As Double = 0
            Dim sign As Integer = 1
            If text.Contains("(") Then
                text = text.Trim("("c, ")"c)
                Dim m As Text.RegularExpressions.Match = New Text.RegularExpressions.Regex(Scalars.Real.Match).Match("^" & text)
                real = Formatting.FromScientific(m.ToString)
                text = text.Substring(m.Length).Trim
                sign = If(text(0) = "-"c, -1, 1)
                text = text.Substring(1).Trim
            End If
            text = text.Trim(" ", "*", "·", "i", "𝑖")
            imag = Formatting.FromScientific(text) * sign
            Dim retval As Complex = Nothing
            If quantity IsNot Nothing Then
                If unit IsNot Nothing Then
                    retval = New Complex(quantity, real, imag, unit)
                Else
                    retval = New Complex(quantity, real, imag)
                End If
            Else
                If unit IsNot Nothing Then
                    retval = New Complex(real, imag, unit)
                Else
                    retval = New Complex(real, imag)
                End If
            End If
            Return retval
        End Function

    End Class

End Namespace
