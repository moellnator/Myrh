Imports Myrh.Entities.Model

Namespace Entities
    Public Class Quantity : Inherits Compound(Of Quantity)

        Public Class Atom : Inherits Atom(Of Quantity)

            Public ReadOnly Property Base As Quantity
            Public ReadOnly Property AssociatedUnit As Unit
            Public ReadOnly Property Definition As Quantity

            Public Sub New(name As String, symbol As String, unit As Unit, definition As Quantity)
                MyBase.New(name, symbol)
                If name = "" Then
                    Me.AssociatedUnit = Unit.Empty
                    Me.Definition = Me.AsQuantity
                    Me.Base = Me.AsQuantity
                Else
                    Me.Definition = If(definition, Me.AsQuantity)
                    Me.AssociatedUnit = unit
                    If unit Is Nothing Then
                        If Me.IsBaseQuantity Then Throw New ArgumentException("Qunatity must contain unit and/or definition.")
                        Me.AssociatedUnit = definition.AssociatedUnit
                    End If
                    Me.Base = Me._get_base
                    If Not Me.Base.AssociatedUnit.Dimension.Equals(Me.AssociatedUnit.Dimension) Then
                        If Not Me.Base.AssociatedUnit.Dimension.Simplified.Equals(Me.AssociatedUnit.Dimension.Simplified) Then _
                            Throw New Exception("Unit and quantity have different dimensions.")
                    End If
                End If
            End Sub

            Private Function _get_base() As Quantity
                If Me.IsBaseQuantity Then Return Me.AsQuantity
                Dim retval As New List(Of Quantity)
                Me._iter_base(retval, 1.0)
                Return retval.Aggregate(None, Function(a, b) a * b)
            End Function

            Private Sub _iter_base(ByRef retval As List(Of Quantity), exponent As Double)
                If Me.IsBaseQuantity Then
                    If Not Me.Equals(None) Then retval.Add(Me.AsQuantity ^ exponent)
                Else
                    For Each q As Component(Of Quantity) In Me.Definition
                        DirectCast(q.Base, Atom)._iter_base(retval, exponent * q.Exponent)
                    Next
                End If
            End Sub

            Public Function AsQuantity() As Quantity
                Return New Quantity(Me)
            End Function

            Public ReadOnly Property IsBaseQuantity As Boolean
                Get
                    Return Me.Equals(Me.Definition)
                End Get
            End Property

        End Class

        Public Shared ReadOnly Property None As New Quantity

        Public ReadOnly Property Definition As Quantity
        Public ReadOnly Property AssociatedUnit As Unit
        Public ReadOnly Property Base As Quantity

        Private Sub New()
            Me.New(New Atom("", "", Unit.Empty, Nothing))
        End Sub

        Private Sub New(name As String, symbol As String, unit As Unit, definition As Quantity)
            Me.New(New Atom(name, symbol, unit, definition))
        End Sub

        Private Sub New(atom As Atom)
            MyBase.New(atom, 1.0)
            Me.AssociatedUnit = atom.AssociatedUnit
            Me.Definition = atom.Definition
            Me.Base = atom.Base
        End Sub

        Private Sub New(components As IEnumerable(Of Component(Of Quantity)))
            MyBase.New(_FilterEmpty(components, None))
            Me.AssociatedUnit = _product_value(Me, Unit.Empty, Function(q) q.AssociatedUnit)
            Dim is_base As Boolean = Me.All(Function(c) DirectCast(c.Base, Atom).IsBaseQuantity)
            Me.Definition = If(is_base, Me, _product_value(Me, None, Function(q) q.Definition))
            Me.Base = If(is_base, Me, _product_value(Me, None, Function(q) q.Base))
        End Sub

        Private Shared Function _product_value(Of T As Compound(Of T))(q As Quantity, empty As T, f As Func(Of Atom, T)) As T
            Return q.Aggregate(empty, Function(a, b) a * f(DirectCast(b.Base, Atom)) ^ b.Exponent)
        End Function

        Protected Overrides Function CreateNew(components As IEnumerable(Of Component(Of Quantity))) As Quantity
            Return New Quantity(components)
        End Function

        Public Function AsAtom() As Atom
            If Me.Count <> 1 Or Me.First.Exponent <> 1.0 Then Throw New Exception("Cannot convert unit to atom.")
            Return DirectCast(Me.First.Base, Atom)
        End Function

        Public Shared Sub Define(name As String, symbol As String, unit As Unit, definition As Quantity)
            Dim q As New Quantity(name, symbol, unit, definition)
            If Domain.Current.Quantities.Contains(q) Then Throw New DuplicateNameException
            Domain.Current.Quantities.Add(q)
        End Sub

        Public Shared Shadows Function Parse(text As String) As Quantity
            Dim retval As Quantity = None
            For Each component As Tuple(Of String, Double) In Compound(Of Quantity).Parse(text)
                Dim defined As Quantity = Domain.Current.Quantities.First(Function(c) component.Item1.Equals(If(component.Item1.Contains("("), c.Name, c.Symbol)))
                retval *= defined ^ component.Item2
            Next
            Return retval
        End Function

        Public Shared Function InferFrom(unit As Unit) As Quantity
            Dim candidates As Quantity() = Domain.Current.Quantities.Where(Function(q) q.AssociatedUnit.Equals(unit) Or q.AssociatedUnit.Equals(unit.DefaultUnit)).ToArray
            If candidates.Count = 0 Then
                Dim q_full As Quantity = unit.Select(Function(c) InferFrom(DirectCast(c.Base, Unit.Atom).AsUnit) ^ c.Exponent).Aggregate(None, Function(a, b) a * b)
                While _Length(q_full.Definition) < _Length(q_full)
                    q_full = q_full.Definition.Simplified
                End While
                Return q_full
            Else
                Return candidates.First
            End If
        End Function

        Private Shared Function _Length(q As Quantity) As Double
            Return q.Sum(Function(c) Math.Abs(c.Exponent))
        End Function


    End Class

End Namespace
