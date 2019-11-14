Namespace Entities
    Public MustInherit Class Entity(Of T As {Entity(Of T)})

        Public ReadOnly Property Unit As Unit
        Public ReadOnly Property Quantity As Quantity

        Public Sub New(unit As Unit)
            Me.Unit = unit
            Me.Quantity = Quantity.InferFrom(Me.Unit)
        End Sub

        Public Sub New(quantity As Quantity)
            Me.Quantity = quantity
            Me.Unit = Me.Quantity.AssociatedUnit
        End Sub

        Public Sub New(quantity As Quantity, unit As Unit)
            If Not quantity.AssociatedUnit.Dimension.Equals(unit.Dimension) Then Throw New ArgumentException("Unit does not match dimension of quantity.")
            Me.Quantity = quantity
            Me.Unit = unit
        End Sub

        Public Function AsQuantity(newQuantity As Quantity) As T
            If Not Me.Quantity.Base.Equals(newQuantity.Base) Then Throw New ArgumentException($"Unable to cast {Me.Quantity} to {newQuantity}.")
            Return MakeEntity(newQuantity, Link.Make(Me.Unit))
        End Function

        Public Function WithUnit(newUnit As Unit) As T
            Dim link As Link = Link.Make(Me.Unit, newUnit)
            Return MakeEntity(Me._Quantity, link)
        End Function

        Protected MustOverride Function MakeEntity(quantity As Quantity, link As Link) As T

    End Class

End Namespace
