Option Explicit Off

Imports Myrh.Entities
Imports Myrh.Values
Imports Myrh.Values.Scalars
Imports SimpleTexUnicode

<TestClass()> Public Class MyrhTest

    <TestMethod> Public Sub TestDimension()
        Dim d As Dimension = Dimension.Parse("(length)*(time)^2M²T^-3(mass)I²(temperature)")
        Dim d_text As String = d.ToString
        Dim d_re As Dimension = Dimension.Parse(d_text)
        Assert.AreEqual(d, d_re)
    End Sub

    <TestMethod> Public Sub TestUnit()
        Domain.Current.Reset()
        Domain.Current.SetDefaultSystem("metric")
        Unit.Define("gramm", "g", Prefix.Kilo, "metric", Dimension.Mass)
        Dim u As Unit = Unit.Parse("g")
        Assert.AreEqual("(gramm)", u.Name)
    End Sub

    <TestMethod> Public Sub TestUnitOne()
        Domain.Current.Reset()
        Domain.Current.SetDefaultSystem("metric")
        Unit.Define("meter", "m", Prefix.One, "metric", Dimension.Length)
        Dim u As Unit = Unit.Parse("m m^-1").Simplified
        Assert.AreEqual(u, Unit.Empty)
    End Sub

    <TestMethod> Public Sub TestLink()
        Domain.Current.Reset()
        Domain.Current.SetDefaultSystem("metric")
        Unit.Define("gramm", "g", Prefix.Kilo, "metric", Dimension.Mass)
        Unit.Define("meter", "m", Prefix.One, "metric", Dimension.Length)
        Dim l As Link = Link.Make(Unit.Parse("g m^-1"), Unit.Parse("g mm^-1"))
        Debug.Print(l.ToString)
        Assert.AreEqual(0.001, l.Scaling)
    End Sub

    <TestMethod> Public Sub TestQuantity()
        Domain.Current.Reset()
        Domain.Current.SetDefaultSystem("metric")
        Unit.Define("gramm", "g", Prefix.Kilo, "metric", Dimension.Mass)
        Unit.Define("meter", "m", Prefix.One, "metric", Dimension.Length)
        Quantity.Define("length", "l", Unit.Parse("m"), Nothing)
        Quantity.Define("mass", "m", Unit.Parse("kg"), Nothing)
        Quantity.Define("volume", "V", Unit.Parse("m³"), Quantity.Parse("l³"))
        Quantity.Define("mass density", SimpleTex.LatexToUnicode("\rho"), Nothing, Quantity.Parse("m V^-1"))
        Dim q As Quantity = Quantity.Parse(SimpleTex.LatexToUnicode("\rho"))
        Assert.AreEqual(Quantity.Parse("m l^-3"), q.Base)
    End Sub


    <TestMethod> Public Sub TestPerm()
        For Each combination As IEnumerable(Of Char()) In Model.Subset.All("ABCDEF")
            Debug.Print(String.Join(" ", combination.Select(Function(c) New String(c)).ToArray))
        Next
    End Sub

    <TestMethod> Public Sub TestInfer()
        Domain.Current.Reset()
        Domain.Current.SetDefaultSystem("metric")
        Unit.Define("gramm", "g", Prefix.Kilo, "metric", Dimension.Mass)
        Unit.Define("meter", "m", Prefix.One, "metric", Dimension.Length)
        Unit.Define("time", "s", Prefix.One, "metric", Dimension.Length)
        Quantity.Define("length", "l", Unit.Parse("m"), Nothing)
        Quantity.Define("mass", "m", Unit.Parse("kg"), Nothing)
        Quantity.Define("time", "t", Unit.Parse("s"), Nothing)
        Quantity.Define("volume", "V", Nothing, Quantity.Parse("l³"))
        Quantity.Define("mass density", SimpleTex.LatexToUnicode("\rho"), Nothing, Quantity.Parse("m V^-1"))
        Quantity.Define("velocity", "v", Nothing, Quantity.Parse("l t^-1"))
        Quantity.Define("acceleation", "a", Nothing, Quantity.Parse("v t^-1"))
        Quantity.Define("momentum", "p", Nothing, Quantity.Parse("m v"))
        Unit.Define("Newton", "N", Prefix.One, "metric", Dimension.Parse("M L T^-2"))
        Quantity.Define("force", "F", Unit.Parse("N"), Quantity.Parse("m a"))
        Unit.Define("Joule", "J", Prefix.One, "metric", Dimension.Parse("M L^2 T^-2"))
        Quantity.Define("energy", "E", Unit.Parse("J"), Quantity.Parse("F l"))

        Dim q As Quantity = Quantity.InferFrom(Unit.Parse("J m^-1 kg^-1"))
    End Sub

    <TestMethod> Public Sub TestDefaultUnit()
        Domain.Current.Reset()
        Domain.Current.SetDefaultSystem("metric")

        Unit.Define("meter", "m", Prefix.One, "metric", Dimension.Length)
        Unit.Define("bla", "b", Prefix.One, "blanit", Dimension.Length)
        Link.Define(Unit.Parse("b").AsAtom, Unit.Parse("m").AsAtom, 0.01)

        Dim u As Unit = Unit.Parse("b")
        Debug.Print(u.DefaultUnit.ToString)
    End Sub

    <TestMethod> Public Sub TestRealCast()
        Domain.Current.Reset()
        Domain.Current.SetDefaultSystem("metric")

        Unit.Define("percent", "%", Prefix.One, "fraction", Dimension.One)
        Link.Define(Unit.Parse("%").AsAtom, Unit.Empty.AsAtom, 0.01)
        Quantity.Define("probability", "p", Unit.Empty, Nothing)

        Dim p As New Real(Quantity.Parse("p"), 51.257, Unit.Parse("%"))
        Assert.AreEqual(0.51257, CDbl(p))
    End Sub

    <TestMethod> Public Sub TestRealMultiply()
        Domain.Current.Reset()
        Domain.Current.SetDefaultSystem("metric")

        Unit.Define("gramm", "g", Prefix.Kilo, "metric", Dimension.Mass)
        Unit.Define("meter", "m", Prefix.One, "metric", Dimension.Length)
        Unit.Define("time", "s", Prefix.One, "metric", Dimension.Length)
        Quantity.Define("length", "l", Unit.Parse("m"), Nothing)
        Quantity.Define("mass", "m", Unit.Parse("kg"), Nothing)
        Quantity.Define("time", "t", Unit.Parse("s"), Nothing)
        Quantity.Define("velocity", "v", Nothing, Quantity.Parse("l t^-1"))

        Dim u As New Real(5, Unit.Parse("m"))
        Dim v As New Real(10, Unit.Parse("s^-1"))
        Dim c As New Real(0.4, Unit.Parse("km s^-1"))

        Debug.Print((u * v + c).AsQuantity(Quantity.Parse("v")).ToString)
        Debug.Print((c + u * v).ToString)
        Debug.Print((5 * u - u).ToString)

    End Sub

    <TestMethod> Public Sub TestRealPower()
        Domain.Current.Reset()
        Domain.Current.SetDefaultSystem("metric")

        Unit.Define("meter", "m", Prefix.One, "metric", Dimension.Length)
        Quantity.Define("length", "l", Unit.Parse("m"), Nothing)
        Quantity.Define("area", "A", Nothing, Quantity.Parse("l²"))

        Dim u As New Real(5, Unit.Parse("m"))
        Debug.Print((u ^ 2).AsQuantity(Quantity.Parse("A")).ToString)

    End Sub

    <TestMethod> Public Sub TestComplex()
        Domain.Current.Reset()
        Domain.Current.SetDefaultSystem("metric")

        Unit.Define("gramm", "g", Prefix.Kilo, "metric", Dimension.Mass)
        Unit.Define("meter", "m", Prefix.One, "metric", Dimension.Length)
        Unit.Define("time", "s", Prefix.One, "metric", Dimension.Length)
        Quantity.Define("length", "l", Unit.Parse("m"), Nothing)
        Quantity.Define("mass", "m", Unit.Parse("kg"), Nothing)
        Quantity.Define("time", "t", Unit.Parse("s"), Nothing)
        Quantity.Define("velocity", "v", Nothing, Quantity.Parse("l t^-1"))

        Dim u As New Complex(5, 6, Unit.Parse("m"))
        Dim r As New Real(4, Unit.Parse("m"))
        Debug.Print((u + u).ToString)
        Debug.Print((u + r).ToString)
        Debug.Print((u * r).ToString)
        Debug.Print((u * u).ToString)
        Debug.Print((u ^ 2).ToString)

        Debug.Print(Numerics.Complex.Cos(New Complex(1, 2)).ToString)

        Assert.IsTrue(New Complex(5, 0, Unit.Parse("m")) = New Real(5, Unit.Parse("m")))

    End Sub

    <TestMethod> Public Sub TestSubGrouping()
        Dim compound As String = "ABC"
        For Each permutation In Model.Subset.All(compound)
            Debug.Print(String.Join(",", permutation.Select(Function(g) "(" & New String(g.ToArray) & ")").ToArray))
        Next
    End Sub

    <TestMethod> Public Sub TestValueParsing()
        Domain.Current.Reset()
        Domain.Current.SetDefaultSystem("metric")

        Unit.Define("gramm", "g", Prefix.Kilo, "metric", Dimension.Mass)
        Unit.Define("meter", "m", Prefix.One, "metric", Dimension.Length)
        Unit.Define("time", "s", Prefix.One, "metric", Dimension.Length)
        Quantity.Define("length", "l", Unit.Parse("m"), Nothing)
        Quantity.Define("mass", "m", Unit.Parse("kg"), Nothing)
        Quantity.Define("time", "t", Unit.Parse("s"), Nothing)

        Dim test_string As String = "(length)^-1 (mass)²·t -12.3 * 10² m^-1 (kilogramm)^2*s"
        Dim retval As Object = Value.Parse(test_string)
        Debug.Print(retval.ToString)

    End Sub

End Class