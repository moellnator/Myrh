Imports SimpleTexUnicode

Namespace Entities.Model
    Public Class Component(Of T As Compound(Of T)) : Implements INamed, IEquatable(Of Compound(Of T)), IEnumerable(Of Component(Of T))

        Public ReadOnly Property Base As Atom(Of T)
        Public ReadOnly Property Exponent As Double

        Public Sub New(base As Atom(Of T), exponent As Double)
            Me.Base = base
            Me.Exponent = exponent
        End Sub

        Private Function IEquatable_Equals(other As Compound(Of T)) As Boolean Implements IEquatable(Of Compound(Of T)).Equals
            Return other.Name = Me.Name
        End Function

        Public ReadOnly Property Name As String Implements INamed.Name
            Get
                Return _to_string(Me.Base.Name, Me.Exponent)
            End Get
        End Property

        Public ReadOnly Property Symbol As String Implements INamed.Symbol
            Get
                Return _to_string(Me.Base.Symbol, Me.Exponent)
            End Get
        End Property

        Private Shared Function _to_string(value As String, exponent As String) As String
            Return value & If(exponent <> 1, SimpleTex.LatexToUnicode("^{" & exponent.ToString.Replace(",", ".") & "}"), "")
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim retval As Boolean = False
            If GetType(Compound(Of T)).IsAssignableFrom(obj.GetType) Then
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

        Public Function GetEnumerator() As IEnumerator(Of Component(Of T)) Implements IEnumerable(Of Component(Of T)).GetEnumerator
            Return {Me}.AsEnumerable.GetEnumerator
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me.GetEnumerator
        End Function

    End Class

End Namespace
