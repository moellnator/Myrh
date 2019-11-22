Imports Myrh.Entities

Namespace Values.Scalars

    <Value(UComplex.Match)>
    Public Class UComplex : Inherits Complex

        Public Shadows Const Match As String = "\(" & UReal.Match & "[ ]*[+-][ ]*" & UReal.Match & "[ ]*[\*·]?[i𝑖]\)"

        Private ReadOnly _real_unc As Double
        Private ReadOnly _imag_unc As Double

        Public ReadOnly Property Value As Complex
            Get
                Return New Complex(Me.Quantity, Me._real, Me._imag, Me.Unit)
            End Get
        End Property

        Public ReadOnly Property Uncertainty As Complex
            Get
                Return New Complex(Me.Quantity, Me._real_unc, Me._imag_unc, Me.Unit)
            End Get
        End Property

        Public Overloads ReadOnly Property Real As UReal
            Get
                Return New UReal(Me.Quantity, Me._real, Me._real_unc, Me.Unit)
            End Get
        End Property

        Public Overloads ReadOnly Property Imaginary As UReal
            Get
                Return New UReal(Me.Quantity, Me._imag, Me._imag_unc, Me.Unit)
            End Get
        End Property

        Public Sub New()
            Me.New(0.0, 0.0, 0.0, 0.0)
        End Sub

        Public Sub New(real As Double, imaginary As Double, realUncertain As Double, imaginaryUncertain As Double)
            MyBase.New(real, imaginary)
            Me._real_unc = realUncertain
            Me._imag_unc = imaginaryUncertain
        End Sub

        Public Sub New(value As Numerics.Complex, uncertainty As Numerics.Complex)
            MyBase.New(value)
            Me._real_unc = uncertainty.Real
            Me._imag_unc = uncertainty.Imaginary
        End Sub

        Public Sub New(real As Double, imaginary As Double, realUncertain As Double, imaginaryUncertain As Double, unit As Unit)
            MyBase.New(real, imaginary, unit)
            Me._real_unc = realUncertain
            Me._imag_unc = imaginaryUncertain
        End Sub

        Public Sub New(value As Numerics.Complex, uncertainty As Numerics.Complex, unit As Unit)
            MyBase.New(value, unit)
            Me._real_unc = uncertainty.Real
            Me._imag_unc = uncertainty.Imaginary
        End Sub

        Public Sub New(quantity As Quantity, real As Double, imaginary As Double, realUncertain As Double, imaginaryUncertain As Double)
            MyBase.New(quantity, real, imaginary)
            Me._real_unc = realUncertain
            Me._imag_unc = imaginaryUncertain
        End Sub

        Public Sub New(quantity As Quantity, value As Numerics.Complex, uncertainty As Numerics.Complex)
            MyBase.New(quantity, value)
            Me._real_unc = uncertainty.Real
            Me._imag_unc = uncertainty.Imaginary
        End Sub

        Public Sub New(quantity As Quantity, real As Double, imaginary As Double, realUncertain As Double, imaginaryUncertain As Double, unit As Unit)
            MyBase.New(quantity, real, imaginary, unit)
            Me._real_unc = realUncertain
            Me._imag_unc = imaginaryUncertain
        End Sub

        Public Sub New(quantity As Quantity, value As Numerics.Complex, uncertainty As Numerics.Complex, unit As Unit)
            MyBase.New(quantity, value, unit)
            Me._real_unc = uncertainty.Real
            Me._imag_unc = uncertainty.Imaginary
        End Sub

        Public Overloads Shared Widening Operator CType(value As Double) As UComplex
            Return New UComplex(value, 0, 0, 0)
        End Operator

        Public Overloads Shared Widening Operator CType(value As Numerics.Complex) As UComplex
            Return New UComplex(value.Real, value.Imaginary, 0, 0)
        End Operator

        Public Overloads Shared Narrowing Operator CType(value As UComplex) As Numerics.Complex
            If Not (value._real_unc = 0 And value._imag_unc = 0) Then Throw New InvalidCastException("Cannot cast uncertain value to complex.")
            If Not value.Unit.IsDimensionless Then Throw New InvalidCastException("Cannot cast entity to Double: Dimension is not one.")
            If Not value.Unit.IsDefault Then value = value.WithUnit(value.Unit.DefaultUnit)
            Return value.Value
        End Operator

        Public Overloads Shared Narrowing Operator CType(value As UComplex) As Double
            If Not (value._real_unc = 0 And value._imag_unc = 0) Then Throw New InvalidCastException("Cannot cast uncertain value to double.")
            Return CType(value.Value, Real)
        End Operator

        Public Overloads Shared Narrowing Operator CType(value As UComplex) As UReal
            If Not value._imag = 0 And value._imag_unc = 0 Then Throw New Exception("Unable to cast complex value to real.")
            Return New UReal(value.Quantity, value._real, value._real_unc, value.Unit)
        End Operator

        Protected Overrides Function MakeEntity(quantity As Quantity, link As Link) As Value
            Return New UComplex(quantity, Me._real * link.Scaling, Me._imag * link.Scaling,
                                Math.Abs(Me._real_unc * link.Scaling), Math.Abs(Me._imag_unc * link.Scaling), link.Target)
        End Function

        Public Overloads Shared Operator *(a As UComplex, b As UComplex) As UComplex
            Return (a.Value * b.Value).WithUncertainty((a.Uncertainty * b.Value).Absolute + (a.Value * b.Uncertainty).Absolute)
        End Operator

        Public Overloads Shared Operator *(a As UComplex, b As Complex) As UComplex
            Return (a.Value * b).WithUncertainty((a.Uncertainty * b).Absolute)
        End Operator

        Public Overloads Shared Operator *(a As Complex, b As UComplex) As UComplex
            Return b * a
        End Operator

        Public Overloads Shared Operator /(a As UComplex, b As UComplex) As UComplex
            Return (a.Value / b.Value).WithUncertainty((a.Uncertainty / b.Value).Absolute + (a.Value * b.Uncertainty / b.Value ^ 2).Absolute)
        End Operator

        Public Overloads Shared Operator /(a As UComplex, b As Complex) As UComplex
            Return (a.Value / b).WithUncertainty((a.Uncertainty / b).Absolute)
        End Operator

        Public Overloads Shared Operator /(a As Complex, b As UComplex) As UComplex
            Return (a / b.Value).WithUncertainty((a * b.Uncertainty / b.Value ^ 2).Absolute)
        End Operator

        Public Overloads Shared Operator +(a As UComplex, b As UComplex) As UComplex
            Return (a.Value + b.Value).WithUncertainty(a.Uncertainty + b.Uncertainty)
        End Operator

        Public Overloads Shared Operator +(a As UComplex, b As Complex) As UComplex
            Return (a.Value + b).WithUncertainty(a.Uncertainty)
        End Operator

        Public Overloads Shared Operator +(a As Complex, b As UComplex) As UComplex
            Return (a + b.Value).WithUncertainty(b.Uncertainty)
        End Operator

        Public Overloads Shared Operator -(a As UComplex, b As UComplex) As UComplex
            Return (a.Value - b.Value).WithUncertainty(a.Uncertainty + b.Uncertainty)
        End Operator

        Public Overloads Shared Operator -(a As UComplex, b As Complex) As UComplex
            Return (a.Value - b).WithUncertainty(a.Uncertainty)
        End Operator

        Public Overloads Shared Operator -(a As Complex, b As UComplex) As UComplex
            Return (a - b.Value).WithUncertainty(b.Uncertainty)
        End Operator

        Public Overloads Shared Operator ^(a As UComplex, b As Real) As UComplex
            Return (a.Value ^ b).WithUncertainty((b * a.Value ^ (b - 1.0)).Absolute)
        End Operator

        Public Overloads Function Inverted() As UComplex
            Return Me.Value.Inverted.WithUncertainty((Me.Uncertainty / Me.Value ^ 2).Absolute)
        End Function

        Public Overloads Function Conjugated() As UComplex
            Return Me.Value.Conjugated.WithUncertainty(Me.Uncertainty)
        End Function

        Public Overloads ReadOnly Property Magnitude() As UReal
            Get
                Return Me.Value.Magnitude.WithUncertainty(((Me.Real.Value * Me.Real.Uncertainty).Absolute + (Me.Imaginary.Value * Me.Imaginary.Uncertainty).Absolute) / Me.Value.Magnitude)
            End Get
        End Property

        Public Overloads ReadOnly Property Phase() As Real
            Get
                Return Me.Value.Phase.WithUncertainty(
                    ((Me.Imaginary.Value * Me.Real.Uncertainty).Absolute + (Me.Real.Value * Me.Imaginary.Uncertainty).Absolute) /
                    (Me.Real.Value ^ 2 + Me.Imaginary.Value ^ 2)
                )
            End Get
        End Property

        Protected Overrides Function _Parse(text As String, quantity As Quantity, unit As Unit) As Object
            Dim imag As UReal = 0
            Dim real As UReal = 0
            Dim sign As Integer = 1
            If text.Contains("(") Then
                text = text.Substring(text.IndexOf("(") + 1).Substring(0, text.LastIndexOf(")") - 1)
                Dim ur As New Text.RegularExpressions.Regex("^" & UReal.Match)
                Dim match As Text.RegularExpressions.Match = ur.Match(text)
                text = text.Substring(match.Length)
                real = Parse(match.ToString)
                sign = If(text.Trim.First = "-", -1, 1)
                text = text.Trim.Substring(1)
            End If
            If text.Contains("i") Or text.Contains("𝑖") Then
                text = text.Trim(" ", "*", "·", "i", "𝑖")
                imag = Parse(text) * sign
            Else
                real = Parse(text)
            End If
            Dim retval As UComplex = Nothing
            If quantity IsNot Nothing Then
                If unit IsNot Nothing Then
                    retval = New UComplex(quantity, real.Value, imag.Value, real.Uncertainty, imag.Uncertainty, unit)
                Else
                    retval = New UComplex(quantity, real.Value, imag.Value, real.Uncertainty, imag.Uncertainty)
                End If
            Else
                If unit IsNot Nothing Then
                    retval = New UComplex(real.Value, imag.Value, real.Uncertainty, imag.Uncertainty, unit)
                Else
                    retval = New UComplex(real.Value, imag.Value, real.Uncertainty, imag.Uncertainty)
                End If
            End If
            Return retval
        End Function

        Protected Overrides Function _FormatValue() As String
            If Me._real And Me._real_unc = 0 Then
                If Me._imag And Me._imag_unc = 0 Then
                    Return "0"
                Else
                    Return New UReal(Me._imag, Me._imag_unc).ToString & "𝑖"
                End If
            Else
                If Me._imag And Me._imag_unc = 0 Then
                    Return New UReal(Me._real, Me._real_unc).ToString
                Else
                    Return "(" & New UReal(Me._real, Me._real_unc).ToString & " " & If(Math.Sign(Me._imag) = "1", "+", "-") & " " &
                        New UReal(Me._imag, Me._imag_unc).Absolute.ToString & "𝑖)"
                End If
            End If
        End Function

    End Class

End Namespace
