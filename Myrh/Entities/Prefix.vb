Imports Myrh.Entities.Model

Namespace Entities
    Public Class Prefix : Implements INamed : Implements IEquatable(Of Prefix)


        Public Shared ReadOnly Property Yotta As Prefix = New Prefix("Y", "yotta", 1.0E+24)
        Public Shared ReadOnly Property Zetta As Prefix = New Prefix("Z", "zetta", 1.0E+21)
        Public Shared ReadOnly Property Exa As Prefix = New Prefix("E", "exa", 1.0E+18)
        Public Shared ReadOnly Property Peta As Prefix = New Prefix("P", "peta", 1.0E+15)
        Public Shared ReadOnly Property Tera As Prefix = New Prefix("T", "tera", 1000000000000.0)
        Public Shared ReadOnly Property Giga As Prefix = New Prefix("G", "giga", 1000000000.0)
        Public Shared ReadOnly Property Mega As Prefix = New Prefix("M", "mega", 1000000.0)
        Public Shared ReadOnly Property Kilo As Prefix = New Prefix("k", "kilo", 1000)
        Public Shared ReadOnly Property Hecta As Prefix = New Prefix("h", "hecta", 100)
        Public Shared ReadOnly Property Deca As Prefix = New Prefix("da", "deca", 10)
        Public Shared ReadOnly Property One As Prefix = New Prefix("", "one", 1)
        Public Shared ReadOnly Property Deci As Prefix = New Prefix("d", "deci", 0.1)
        Public Shared ReadOnly Property Centi As Prefix = New Prefix("c", "centi", 0.01)
        Public Shared ReadOnly Property Milli As Prefix = New Prefix("m", "milli", 0.001)
        Public Shared ReadOnly Property Micro As Prefix = New Prefix("µ", "micro", 0.000001)
        Public Shared ReadOnly Property Nano As Prefix = New Prefix("n", "nano", 0.000000001)
        Public Shared ReadOnly Property Pico As Prefix = New Prefix("p", "pico", 0.000000000001)
        Public Shared ReadOnly Property Femto As Prefix = New Prefix("f", "femto", 0.000000000000001)
        Public Shared ReadOnly Property Atto As Prefix = New Prefix("a", "atto", 1.0E-18)
        Public Shared ReadOnly Property Zepto As Prefix = New Prefix("z", "zepto", 1.0E-21)
        Public Shared ReadOnly Property Yocto As Prefix = New Prefix("y", "yocto", 1.0E-24)

        Private Shared ReadOnly _prefixes As Prefix() = {
            Yotta,
            Zetta,
            Exa,
            Peta,
            Tera,
            Giga,
            Mega,
            Kilo,
            Hecta,
            Deca,
            One,
            Deci,
            Centi,
            Milli,
            Micro,
            Nano,
            Pico,
            Femto,
            Atto,
            Zepto,
            Yocto
        }

        Public ReadOnly Property Symbol As String Implements INamed.Symbol
        Public ReadOnly Property Name As String Implements INamed.Name
        Public ReadOnly Property Value As Double

        Private Sub New(symbol As String, name As String, value As String)
            Me.Symbol = symbol
            Me.Name = name
            Me.Value = value
        End Sub

        Public Shared Function Parse(text As String) As Prefix
            If text = "" Then Return One
            Return _prefixes.First(Function(p) p.Name = text Or p.Symbol = text)
        End Function

        Public Shared Function HasPrefix(text As String) As Prefix
            Dim retval As Prefix = Nothing
            If text.Contains("(") Then
                retval = _prefixes.FirstOrDefault(Function(p) text.Substring(1).StartsWith(p.Name))
            Else
                retval = _prefixes.FirstOrDefault(Function(p) text.StartsWith(p.Symbol) And Not p.Symbol.Count = 0)
            End If
            Return retval
        End Function

        Private Function IEquatable_Equals(other As Prefix) As Boolean Implements IEquatable(Of Prefix).Equals
            Return Me.Name = other.Name
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim retval As Boolean = False
            If TypeOf obj Is Prefix Then
                retval = Me.IEquatable_Equals(obj)
            Else
                retval = MyBase.Equals(obj)
            End If
            Return retval
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Me.Name.GetHashCode
        End Function

        Public Overrides Function ToString() As String
            Return Me.Name
        End Function

    End Class

End Namespace
