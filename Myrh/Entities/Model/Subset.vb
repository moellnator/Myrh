Namespace Entities.Model
    Public Class Subset

        Public Class MultiIndex : Implements IComparable(Of MultiIndex), IEquatable(Of MultiIndex)

            Private ReadOnly _data As Integer()

            Private Sub New(data As IEnumerable(Of Integer))
                Me._data = data.ToArray
            End Sub

            Public Function SelectIn(Of T)(group As T()) As T()
                Return Me._data.Select(Function(index) group(index)).ToArray
            End Function

            Public Shared Function AllPermutations(size As Integer) As IEnumerable(Of MultiIndex)
                Return _iter_group(Enumerable.Range(0, size).ToArray).Select(Function(g) New MultiIndex(g))
            End Function

            Public Function OrderedGroups() As IEnumerable(Of MultiIndex)
                Return _OrderedGroups(Me).OrderBy(Function(group) group)
            End Function

            Public Shared Function AllOrderedGroups(size As Integer) As IEnumerable(Of MultiIndex())
                Return AllPermutations(size).Select(Function(perm) perm.OrderedGroups.ToArray).Distinct(New MultiIndexGroupEqualityComparer)
            End Function

            Private Shared Iterator Function _OrderedGroups(index As MultiIndex) As IEnumerable(Of MultiIndex)
                Dim current As Integer = 0
                Dim size As Integer = index._data.Count
                While current < size
                    Dim group_end As Integer = size
                    For i = current + 1 To size - 1
                        If index._data(i) < index._data(i - 1) Then
                            group_end = i
                            Exit For
                        End If
                    Next
                    Yield New MultiIndex(index._data.Skip(current).Take(group_end - current).OrderBy(Function(i) i))
                    current = group_end
                End While
            End Function

            Private Shared Iterator Function _iter_group(pool As Integer()) As IEnumerable(Of IEnumerable(Of Integer))
                If pool.Count = 1 Then
                    Yield {pool.First}
                ElseIf pool.Count = 0 Then
                Else
                    For Each index As Integer In pool
                        Dim subpool As Integer() = pool.Where(Function(i) i <> index).ToArray
                        For Each g As IEnumerable(Of Integer) In _iter_group(subpool)
                            Yield g.Prepend(index)
                        Next
                    Next
                End If
            End Function

            Public Overrides Function ToString() As String
                Return "{" & String.Join(", ", Me._data.Select(Function(d) d.ToString).ToArray) & "}"
            End Function

            Public Function CompareTo(other As MultiIndex) As Integer Implements IComparable(Of MultiIndex).CompareTo
                Return Me._data.First.CompareTo(other._data.First)
            End Function

            Private Function IEquatable_Equals(other As MultiIndex) As Boolean Implements IEquatable(Of MultiIndex).Equals
                Return Me._data.SequenceEqual(other._data)
            End Function

            Public Overrides Function Equals(obj As Object) As Boolean
                If TypeOf obj Is MultiIndex Then
                    Return Me.IEquatable_Equals(obj)
                Else
                    Return False
                End If
            End Function

            Private Class MultiIndexGroupEqualityComparer : Inherits EqualityComparer(Of MultiIndex())

                Public Overrides Function Equals(x() As MultiIndex, y() As MultiIndex) As Boolean
                    Return x.SequenceEqual(y)
                End Function

                Public Overrides Function GetHashCode(obj() As MultiIndex) As Integer
                    Return String.Join(", ", obj.Select(Function(c) c.ToString).ToArray).GetHashCode
                End Function

            End Class

        End Class

        Public Shared Function All(Of T)(enumeration As IEnumerable(Of T)) As IEnumerable(Of IEnumerable(Of T()))
            Dim values As T() = enumeration.ToArray
            Return MultiIndex.AllOrderedGroups(values.Count).Select(Function(g) g.Select(Function(m) m.SelectIn(values)))
        End Function

    End Class

End Namespace