Imports Myrh.Entities
Imports Myrh.Values.Scalars

Namespace Values

    Public MustInherit Class Value : Inherits Entity(Of Value)

        Private Const PATTERN_COMPOUND As String = "((\([\w-[\p{Nd}]]+\))|([\p{L}]+))(\^([+-]?[0-9]+(\.[0-9]+)?)|([⁻⁺]?[⁰¹²³⁴⁵⁶⁷⁸⁹]+(˙[⁰¹²³⁴⁵⁶⁷⁸⁹]+)?))?([ ·\*]((\([\w-[\p{Nd}]]+\))|([\p{L}]+))(\^([+-]?[0-9]+(\.[0-9]+)?)|([⁻⁺]?[⁰¹²³⁴⁵⁶⁷⁸⁹]+(˙[⁰¹²³⁴⁵⁶⁷⁸⁹]+)?))?)*"
        Private Shared ReadOnly MATCH_QUANTITY As New Text.RegularExpressions.Regex("^" & PATTERN_COMPOUND, Text.RegularExpressions.RegexOptions.Compiled)
        Private Shared ReadOnly MATCH_UNIT As New Text.RegularExpressions.Regex(PATTERN_COMPOUND & "$", Text.RegularExpressions.RegexOptions.Compiled)
        Private Delegate Function Parser(text As String, quantity As Quantity, unit As Unit) As Object
        Private Shared ReadOnly MATCH_VALUE As Tuple(Of Text.RegularExpressions.Regex, Parser)() =
            GetType(Value).Assembly.GetTypes.Where(
                Function(t) t.GetCustomAttributes(False).Any(Function(a) TypeOf a Is ValueAttribute)
            ).Select(
                Function(t) New Tuple(Of Text.RegularExpressions.Regex, Parser)(
                    New Text.RegularExpressions.Regex(
                        "^" & DirectCast(t.GetCustomAttributes(GetType(ValueAttribute), False).First, ValueAttribute).Pattern & "$",
                        Text.RegularExpressions.RegexOptions.Compiled
                    ),
                    [Delegate].CreateDelegate(
                        GetType(Parser),
                        Activator.CreateInstance(t),
                        t.GetMethod("_Parse", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
                    )
                )
            ).ToArray

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

        Public Shared Function Parse(text As String) As Object
            Dim q As Quantity = Nothing
            If MATCH_QUANTITY.IsMatch(text) Then
                Dim m As Text.RegularExpressions.Match = MATCH_QUANTITY.Match(text)
                q = Quantity.Parse(m.ToString)
                text = text.Substring(m.Length).Trim
            End If
            Dim u As Unit = Nothing
            If MATCH_UNIT.IsMatch(text) Then
                Dim m As Text.RegularExpressions.Match = MATCH_UNIT.Match(text)
                u = Unit.Parse(m.ToString)
                text = text.Substring(0, text.Count - m.Length).Trim
            End If
            Return MATCH_VALUE.First(Function(p) p.Item1.IsMatch(text)).Item2(text, q, u)
        End Function

        Protected MustOverride Function _Parse(text As String, quantity As Quantity, unit As Unit) As Object

    End Class

    Public MustInherit Class Value(Of T As Value(Of T)) : Inherits Value

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
