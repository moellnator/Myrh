Imports Myrh.Entities
Imports SimpleTexUnicode

Namespace Values.Scalars

    <Value(UReal.Match)>
    Public Class UReal : Inherits Real

        Private Const MatchExp As String = "(([eE][+-]?[0-9]+)|([ ]*[\*·×]?[ ]*10((\^[+-]?[0-9]+)|([⁻⁺]?[⁰¹²³⁴⁵⁶⁷⁸⁹]+))))?"
        Private Const MatchReal As String = "[0-9]+(\.[0-9]+)?"
        Public Shadows Const Match As String = "(([+-]?" & MatchReal & "\([0-9]+\)" & MatchExp & ")|" &
                                               "(\([+-]?" & MatchReal & "[ ]*(\+\-|±)[ ]*" & MatchReal & "\)" & MatchExp & ")|" &
                                               "(\(" & Real.Match & "[ ]*(\+\-|±)[ ]*" & MatchReal & MatchExp & "\)))"
        Private ReadOnly _uncertain As Double

        Public ReadOnly Property Value As Real
            Get
                Return New Real(Me.Quantity, Me._internal, Me.Unit)
            End Get
        End Property

        Public ReadOnly Property Uncertainty As Real
            Get
                Return New Real(Me.Quantity, Me._uncertain, Me.Unit)
            End Get
        End Property

        Public Sub New()
            Me.New(0.0, 0.0)
        End Sub

        Public Sub New(value As Double, uncertainty As Double)
            MyBase.New(value)
            Me._uncertain = uncertainty
        End Sub

        Public Sub New(value As Double, uncertainty As Double, unit As Unit)
            MyBase.New(value, unit)
            Me._uncertain = uncertainty
        End Sub

        Public Sub New(quantity As Quantity, value As Double, uncertainty As Double)
            MyBase.New(quantity, value)
            Me._uncertain = uncertainty
        End Sub

        Public Sub New(quantity As Quantity, value As Double, uncertainty As Double, unit As Unit)
            MyBase.New(quantity, value, unit)
            Me._uncertain = uncertainty
        End Sub

        Public Overloads Shared Widening Operator CType(value As Double) As UReal
            Return New UReal(value, 0)
        End Operator

        Public Overloads Shared Narrowing Operator CType(value As UReal) As Double
            If Not value._uncertain = 0 Then Throw New InvalidCastException("Cannot cast uncertain value to double.")
            If Not value.Unit.IsDimensionless Then Throw New InvalidCastException("Cannot cast entity to Double: Dimension is not one.")
            If Not value.Unit.IsDefault Then value = value.WithUnit(value.Unit.DefaultUnit)
            Return value._internal
        End Operator

        Public Overloads Shared Widening Operator CType(value As UReal) As UComplex
            Return New UComplex(value.Quantity, value._internal, 0, value._uncertain, 0, value.Unit)
        End Operator

        Protected Overrides Function MakeEntity(quantity As Quantity, link As Link) As Value
            Return New UReal(quantity, Me._internal * link.Scaling, Math.Abs(Me._uncertain * link.Scaling), link.Target)
        End Function

        Protected Overrides Function _FormatValue() As String
            Dim retval As String = ""
            If Me._uncertain <> 0 Then
                Dim exponent As Double = Math.Floor(Math.Log10(Math.Abs(Me._internal)))
                Dim exp_unc As Double = Math.Floor(Math.Log10(Math.Abs(Me._uncertain)))
                Dim value As Double = Math.Round(CSng(Me._internal / 10 ^ exponent), Formatting.SignificantDigits - 1, MidpointRounding.AwayFromZero)
                Dim unc As Double = Math.Round(CSng(Me._uncertain / 10 ^ exponent), Formatting.SignificantDigits - 1, MidpointRounding.AwayFromZero)
                If exp_unc - exponent <= -1 Then
                    Dim s_unc As String = _FormatDouble(unc)
                    If exponent - exp_unc + 1 < s_unc.Count Then
                        s_unc = s_unc.Substring(exponent - exp_unc + 1)
                    Else
                        s_unc = ""
                    End If
                    If s_unc = "" Then s_unc = "0"
                    retval = _FormatDouble(value) & "(" & s_unc & ")"
                Else
                    retval = "(" & _FormatDouble(value) & " ± " & _FormatDouble(unc) & ")"
                End If
                retval &= If(exponent <> 0, SimpleTex.LatexToUnicode($"\times10^{{{exponent}}}"), "")
            Else
                retval = Formatting.ToScientific(Me._internal)
            End If
            Return retval
        End Function

        Private Shared Function _FormatDouble(value As Double) As String
            Return String.Format(Globalization.CultureInfo.InvariantCulture, "{0:0" & ".".PadRight(Formatting.SignificantDigits, "0"c) & "}", value)
        End Function

        Public Overloads Shared Operator *(a As UReal, b As UReal) As UReal
            Return (a.Value * b.Value).WithUncertainty((a.Uncertainty * b.Value).Absolute + (a.Value * b.Uncertainty).Absolute)
        End Operator

        Public Overloads Shared Operator *(a As UReal, b As Real) As UReal
            Return (a.Value * b).WithUncertainty((a.Uncertainty * b).Absolute)
        End Operator

        Public Overloads Shared Operator *(a As Real, b As UReal) As UReal
            Return b * a
        End Operator

        Public Overloads Shared Operator *(a As UReal, b As Complex) As UComplex
            Return CType(a, UComplex) * b
        End Operator

        Public Overloads Shared Operator *(a As Complex, b As UReal) As UComplex
            Return a * CType(b, UComplex)
        End Operator

        Public Overloads Shared Operator /(a As UReal, b As UReal) As UReal
            Return (a.Value / b.Value).WithUncertainty((a.Uncertainty / b.Value).Absolute + (a.Value * b.Uncertainty / b.Value ^ 2).Absolute)
        End Operator

        Public Overloads Shared Operator /(a As UReal, b As Real) As UReal
            Return (a.Value / b).WithUncertainty((a.Uncertainty / b).Absolute)
        End Operator

        Public Overloads Shared Operator /(a As Real, b As UReal) As UReal
            Return (a / b.Value).WithUncertainty((a * b.Uncertainty / b.Value ^ 2).Absolute)
        End Operator

        Public Overloads Shared Operator /(a As UReal, b As Complex) As UComplex
            Return CType(a, UComplex) / b
        End Operator

        Public Overloads Shared Operator /(a As Complex, b As UReal) As UComplex
            Return a / CType(b, UComplex)
        End Operator

        Public Overloads Shared Operator +(a As UReal, b As UReal) As UReal
            Return (a.Value + b.Value).WithUncertainty(a.Uncertainty + b.Uncertainty)
        End Operator

        Public Overloads Shared Operator +(a As UReal, b As Real) As UReal
            Return (a.Value + b).WithUncertainty(a.Uncertainty)
        End Operator

        Public Overloads Shared Operator +(a As Real, b As UReal) As UReal
            Return (a + b.Value).WithUncertainty(b.Uncertainty)
        End Operator

        Public Overloads Shared Operator +(a As UReal, b As Complex) As UComplex
            Return CType(a, UComplex) + b
        End Operator

        Public Overloads Shared Operator +(a As Complex, b As UReal) As UComplex
            Return a + CType(b, UComplex)
        End Operator

        Public Overloads Shared Operator -(a As UReal, b As UReal) As UReal
            Return (a.Value - b.Value).WithUncertainty(a.Uncertainty + b.Uncertainty)
        End Operator

        Public Overloads Shared Operator -(a As UReal, b As Real) As UReal
            Return (a.Value - b).WithUncertainty(a.Uncertainty)
        End Operator

        Public Overloads Shared Operator -(a As Real, b As UReal) As UReal
            Return (a - b.Value).WithUncertainty(b.Uncertainty)
        End Operator

        Public Overloads Shared Operator -(a As UReal, b As Complex) As UComplex
            Return CType(a, UComplex) - b
        End Operator

        Public Overloads Shared Operator -(a As Complex, b As UReal) As UComplex
            Return a - CType(b, UComplex)
        End Operator

        Public Overloads Shared Operator ^(a As UReal, b As Real) As UReal
            Return (a.Value ^ b).WithUncertainty((b * a.Value ^ (b - 1.0)).Absolute)
        End Operator

        Public Overloads Function Absolute() As UReal
            Return New UReal(Me.Quantity, Math.Abs(Me._internal), Me._uncertain, Me.Unit)
        End Function

        Public Overloads Function SimilarTo(b As UReal) As Real
            If b._uncertain = 0 Then
                Return Me.SimilarTo(b.Value)
            Else
                Return Math.Exp(-1 * (Me.Value - b.Value) ^ 2 / (2 * (Me.Uncertainty ^ 2 + b.Uncertainty ^ 2))) /
                    (2 * Math.PI * (Me.Uncertainty ^ 2 + b.Uncertainty ^ 2)) ^ 0.5
            End If
        End Function

        Public Overloads Function SimilarTo(b As Real) As Real
            If Me._uncertain = 0 Then
                Return If(Me.Value.Equals(b), Double.PositiveInfinity, 0)
            Else
                Return Math.Exp(-1 * (Me.Value - b) ^ 2 / (2 * Me.Uncertainty ^ 2)) / Math.Sqrt(2 * Math.PI * Me.Uncertainty ^ 2)
            End If
        End Function

        Protected Overrides Function _Parse(text As String, quantity As Quantity, unit As Unit) As Object
            Dim retval As UReal = Nothing
            Dim value As Double = 0
            Dim unc As Double = 0
            If text.Contains("+-") Or text.Contains("±") Then
                Dim mant As String = text.Substring(1, text.IndexOf(")") - 1)
                Dim parts As String() = mant.Split({"±", "+-"}, StringSplitOptions.None)
                value = Formatting.FromScientific(parts.First)
                unc = Formatting.FromScientific(parts.Last)
            Else
                Dim mant As String = text.Substring(0, text.IndexOf(")"))
                Dim parts As String() = mant.Split({"("c}, StringSplitOptions.None)
                value = Val(parts.First)
                Dim uncs As String = parts.Last.Trim("("c, ")"c)
                Dim dot_pos As Integer = parts.First.IndexOf(".")
                uncs = If(dot_pos <> -1, New String("0", dot_pos) & ".", "") &
                       New String("0", parts.First.Count - uncs.Length - If(dot_pos <> -1, dot_pos + 1, 0)) &
                       uncs
                unc = Val(uncs)
            End If
            text = text.Substring(text.IndexOf(")") + 1).Trim(" "c, "*", "·", "×")
            Dim exp As Double = 0
            If text.Count <> 0 Then
                If text.Contains("^") Then
                    exp = Val(text.Split("^").Last)
                ElseIf text.ToLower.Contains("e") Then
                    exp = Val(text.Split("e"c, "E"c).Last)
                Else
                    Dim super As String = "⁻⁺⁰˙¹²³⁴⁵⁶⁷⁸⁹"
                    Dim super_map As String = "-+0.123456789"
                    Dim f_map As Func(Of String, String) = Function(s) New String(s.Select(Function(c) super_map(super.IndexOf(c))).ToArray)
                    exp = Val(f_map(text.Substring(text.IndexOf("10") + 2)))
                End If
            End If
            value *= 10 ^ exp
            unc *= 10 ^ exp
            If quantity IsNot Nothing Then
                If unit IsNot Nothing Then
                    retval = New UReal(quantity, value, unc, unit)
                Else
                    retval = New UReal(quantity, value, unc)
                End If
            Else
                If unit IsNot Nothing Then
                    retval = New UReal(value, unc, unit)
                Else
                    retval = New UReal(value, unc)
                End If
            End If
            Return retval
        End Function

    End Class

End Namespace
