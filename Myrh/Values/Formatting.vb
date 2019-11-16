Imports SimpleTexUnicode

Namespace Values
    Public Class Formatting

        Public Shared Property SignificantDigits As Integer = 3

        Public Shared Function ToScientific(value As Double) As String
            Dim retval As String = ""
            value = CSng(value)
            Dim exponent As Double = Math.Floor(Math.Log10(Math.Abs(value)))
            Dim shift As Integer = exponent Mod 3
            exponent = 3 * (exponent \ 3)
            value = Math.Round(value / 10 ^ exponent, SignificantDigits - shift - 1)
            Return value.ToString.Replace(",", ".") & If(exponent <> 0, SimpleTex.LatexToUnicode($"\times 10^{{{exponent}}}"), "")
        End Function

        Public Shared Function FromScientific(text As String) As Double
            Dim v As Double = 0
            If text.Contains(" ") Or text.Contains("*") Or text.Contains("×") Then
                Dim vp As String() = text.Split({"*"c, "×"c})
                Dim exp As Double
                v = Val(vp.First)
                If text.Contains("^") Then
                    exp = Val(vp.Last.Split("^").Last)
                Else
                    Dim super As String = "⁻⁺⁰˙¹²³⁴⁵⁶⁷⁸⁹"
                    Dim super_map As String = "-+0.123456789"
                    Dim f_map As Func(Of String, String) = Function(s) New String(s.Select(Function(c) super_map(super.IndexOf(c))).ToArray)
                    exp = Val(f_map(text.Substring(text.IndexOf("10") + 2)))
                End If
                v *= 10 ^ exp
            Else
                v = Val(text)
            End If
            Return v
        End Function

    End Class

End Namespace
