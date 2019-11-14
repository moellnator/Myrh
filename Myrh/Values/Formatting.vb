Imports SimpleTexUnicode

Namespace Values
    Public Class Formatting

        Public Shared Property SignificantDigits As Integer = 3

        Public Shared Function Scientific(value As Double) As String
            Dim retval As String = ""
            value = CSng(value)
            Dim exponent As Double = Math.Floor(Math.Log10(Math.Abs(value)))
            Dim shift As Integer = exponent Mod 3
            exponent = 3 * (exponent \ 3)
            value = Math.Round(value / 10 ^ exponent, SignificantDigits - shift - 1)
            Return value.ToString.Replace(",", ".") & If(exponent <> 0, SimpleTex.LatexToUnicode($"\times 10^{{{exponent}}}"), "")
        End Function

    End Class

End Namespace
