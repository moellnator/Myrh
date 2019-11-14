Namespace Entities.Model
    Public Class Atom(Of T As Compound(Of T)) : Implements INamed, IEquatable(Of Compound(Of T)), IEquatable(Of Atom(Of T)), IEnumerable(Of Component(Of T))

        Public Overridable ReadOnly Property Name As String Implements INamed.Name
        Public Overridable ReadOnly Property Symbol As String Implements INamed.Symbol

        Public Sub New(name As String, symbol As String)
            Me.Name = If(name <> "", "(" & name & ")", "")
            Me.Symbol = symbol
        End Sub

        Private Function IEquatable_Equals(other As Compound(Of T)) As Boolean Implements IEquatable(Of Compound(Of T)).Equals
            Return other.Name = Me.Name
        End Function

        Private Function IEquatable_Atom_Equals(other As Atom(Of T)) As Boolean Implements IEquatable(Of Atom(Of T)).Equals
            Return other.Name = Me.Name
        End Function

        Public Overrides Function ToString() As String
            Return Me.Name
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim retval As Boolean = False
            If GetType(Compound(Of T)).IsAssignableFrom(obj.GetType) Then
                retval = Me.IEquatable_Equals(obj)
            ElseIf GetType(Atom(Of T)).IsAssignableFrom(obj.GetType) Then
                retval = Me.IEquatable_Atom_Equals(obj)
            Else
                retval = MyBase.Equals(obj)
            End If
            Return retval
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Me.Name.GetHashCode
        End Function

        Public Function GetEnumerator() As IEnumerator(Of Component(Of T)) Implements IEnumerable(Of Component(Of T)).GetEnumerator
            Return {New Component(Of T)(Me, 1.0)}.AsEnumerable.GetEnumerator
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me.GetEnumerator
        End Function

    End Class

End Namespace
