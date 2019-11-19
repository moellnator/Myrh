Imports Myrh.Entities.Model

Namespace Entities
    Public Class Unit : Inherits Compound(Of Unit)

        Public Class Atom : Inherits Atom(Of Unit)

            Public ReadOnly Property Prefix As Prefix
            Public ReadOnly Property System As String
            Public ReadOnly Property Dimension As Dimension
            Public ReadOnly Property RootName As String

            Public Sub New(name As String, symbol As String, prefix As Prefix, system As String, dimension As Dimension)
                MyBase.New(
                    If(name <> "", If(Not prefix.Equals(Prefix.One), prefix.Name & If(name.Any(Function(c) c = " " Or c = "-"), " ", "") & name, name), ""),
                    prefix.Symbol & symbol
                )
                Me.RootName = "(" & name & ")"
                Me.Prefix = prefix
                Me.System = system
                Me.Dimension = dimension
            End Sub

            Public Function AsUnit() As Unit
                Return New Unit(Me)
            End Function

            Public ReadOnly Property DefaultUnit As Unit
                Get
                    If Me.System = Domain.Current.DefaultSystem Then Return Me.AsUnit
                    Dim roots As Unit() = Domain.Current.Units.Where(Function(u) u.AsAtom.RootName.Equals(Me.RootName) And u.AsAtom.System = Domain.Current.DefaultSystem).ToArray
                    If roots.Count <> 0 Then Return roots.First
                    Dim links As Link() = Domain.Current.Links.Where(Function(l) l.Source.AsAtom.RootName = Me.RootName).ToArray
                    If links.Count <> 0 Then Return links.First.Target
                    Throw New Exception("Default unit has not been linked!")
                End Get
            End Property

        End Class

        Public Shared ReadOnly Property Empty As Unit = New Unit("", "", Prefix.One, Domain.Current.DefaultSystem, Dimension.One)

        Public ReadOnly Property Dimension As Dimension
            Get
                Return Me.Aggregate(Dimension.One, Function(a, b) a * DirectCast(b.Base, Atom).Dimension ^ b.Exponent)
            End Get
        End Property

        Private Sub New(name As String, symbol As String, prefix As Prefix, system As String, dimension As Dimension)
            MyBase.New(New Atom(name, symbol, prefix, system, dimension), 1.0)
        End Sub

        Private Sub New(components As IEnumerable(Of Component(Of Unit)))
            MyBase.New(_FilterEmpty(components, Empty))
        End Sub

        Protected Overrides Function CreateNew(components As IEnumerable(Of Component(Of Unit))) As Unit
            Return New Unit(components)
        End Function

        Public Shared Sub Define(name As String, symbol As String, prefix As Prefix, system As String, dimension As Dimension)
            Dim unit As New Unit(name, symbol, prefix, system, dimension)
            If Domain.Current.Units.Contains(unit) Then Throw New Exception("Unit has already been defined!")
            Domain.Current.Units.Add(unit)
        End Sub

        Public Shared Shadows Function Parse(text As String) As Unit
            Dim retval As New List(Of Unit)
            For Each component As Tuple(Of String, Double) In Compound(Of Unit).Parse(text)
                Dim base_name As String = component.Item1
                Dim is_name As String = base_name.Contains("(")
                Dim base As Unit = Domain.Current.Units.FirstOrDefault(Function(d) base_name = If(base_name.Contains("("), d.Name, d.Symbol))
                If base Is Nothing Then
                    Dim prefix As Prefix = Prefix.HasPrefix(base_name)
                    If prefix Is Nothing Then
                        prefix = Prefix.One
                    Else
                        base_name = If(is_name, "(" & base_name.Substring(1 + prefix.Name.Count).Trim, base_name.Substring(prefix.Symbol.Count))
                    End If
                    base = Domain.Current.Units.First(
                        Function(d) base_name = If(
                            base_name.Contains("("),
                            d.AsAtom.RootName,
                            d.Symbol.Substring(d.AsAtom.Prefix.Symbol.Count)
                         )
                    )
                    base = New Unit(
                        base.AsAtom.RootName.Trim("("c, ")"c),
                        base.Symbol.Substring(base.AsAtom.Prefix.Symbol.Count),
                        prefix,
                        base.AsAtom.System & "+" & prefix.Name,
                        base.Dimension
                    )
                    If Not Domain.Current.Links.Any(
                            Function(link) link.Source.Name.Equals(base.Name) And link.Target.AsAtom.System.Equals(Domain.Current.DefaultSystem)
                        ) Then
                        Dim defaultAtom As Atom = base.DefaultUnit.AsAtom
                        Link.Define(base.AsAtom, defaultAtom, prefix.Value / defaultAtom.Prefix.Value)
                    End If
                End If
                retval.Add(base ^ component.Item2)
            Next
            Return New Unit(retval.SelectMany(Function(d) d))
        End Function

        Public Function AsAtom() As Atom
            If Me.Count <> 1 Or Me.First.Exponent <> 1.0 Then Throw New Exception("Cannot convert unit to atom.")
            Return DirectCast(Me.First.Base, Atom)
        End Function

        Public ReadOnly Property IsDimensionless As Boolean
            Get
                Return Me.Dimension.Simplified.Equals(Dimension.One)
            End Get
        End Property

        Public ReadOnly Property IsDefault As Boolean
            Get
                Return Me.All(Function(c) DirectCast(c.Base, Atom).System = Domain.Current.DefaultSystem)
            End Get
        End Property

        Public ReadOnly Property DefaultUnit As Unit
            Get
                Dim retval As Unit = Nothing
                If Me.IsDefault Then
                    retval = Me
                Else
                    retval = Me.Aggregate(Empty, Function(a, b) a * DirectCast(b.Base, Atom).DefaultUnit ^ b.Exponent)
                End If
                Return retval
            End Get
        End Property

    End Class

End Namespace
