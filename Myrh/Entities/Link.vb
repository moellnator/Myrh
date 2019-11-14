Imports Myrh.Entities.Model

Namespace Entities
    Public Class Link : Implements IEquatable(Of Link)

        Public ReadOnly Property Source As Unit
        Public ReadOnly Property Target As Unit
        Public ReadOnly Property Scaling As Double

        Private Sub New(source As Unit, target As Unit, scaling As Double)
            Me.Source = source
            Me.Target = target
            Me.Scaling = scaling
        End Sub

        Private Sub New(source As Unit)
            Me.Source = source
            Me.Target = source
            Me.Scaling = 1.0
        End Sub

        Public Shared Operator *(a As Link, b As Link) As Link
            Return New Link(a.Source * b.Source, a.Target * b.Target, a.Scaling * b.Scaling)
        End Operator

        Public Shared Operator ^(a As Link, b As Double) As Link
            Return New Link(a.Source ^ b, a.Target ^ b, a.Scaling ^ b)
        End Operator

        Public Function Reversed() As Link
            Return New Link(Me.Target, Me.Source, 1 / Me.Scaling)
        End Function

        Public Shared Operator &(a As Link, b As Link) As Link
            If Not a.Target.Equals(b.Source) Then Throw New Exception("Unable to combine links with different reference system.")
            Return New Link(a.Source, b.Target, a.Scaling * b.Scaling)
        End Operator

        Public Shared Sub Define(source As Unit.Atom, target As Unit.Atom, scaling As Double)
            Dim l As New Link(source.AsUnit, target.AsUnit, scaling)
            If Not target.System = Domain.Current.DefaultSystem Then Throw New ArgumentException("Link must point to a unit of the default system.")
            If Domain.Current.Links.Any(Function(link) link.Source.Equals(source)) Then Throw New Exception("Link does already exist.")
            Domain.Current.Links.Add(l)
        End Sub

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim retval As Boolean = False
            If TypeOf obj Is Link Then
                retval = Me.IEquatable_Equals(obj)
            Else
                retval = MyBase.Equals(obj)
            End If
            Return retval
        End Function

        Private Function IEquatable_Equals(other As Link) As Boolean Implements IEquatable(Of Link).Equals
            Return Me.Source.Equals(other.Source) And Me.Target.Equals(other.Target)
        End Function

        Public Overrides Function ToString() As String
            Return $"1 {Me.Source.Symbol} -> {Me.Scaling.ToString.Replace(",", ".")} {Me.Target.Symbol}"
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Me.Source.GetHashCode Xor Me.Target.GetHashCode
        End Function

        Public Shared Function Make(source As Unit, target As Unit) As Link
            Return ToDefault(source) & ToDefault(target).Reversed
        End Function

        Public Shared Function Make(source As Unit) As Link
            Return New Link(source)
        End Function

        Public Shared Function ToDefault(u As Unit) As Link
            Dim retval As New Link(Unit.Empty)
            For Each c As Component(Of Unit) In u
                Dim base As Unit = DirectCast(c.Base, Unit.Atom).AsUnit
                If base.IsDefault Then
                    retval *= New Link(base) ^ c.Exponent
                Else
                    retval *= Domain.Current.Links.First(Function(l) l.Source.Equals(base) And l.Target.Equals(base.DefaultUnit)) ^ c.Exponent
                End If
            Next
            Return retval
        End Function

    End Class

End Namespace
