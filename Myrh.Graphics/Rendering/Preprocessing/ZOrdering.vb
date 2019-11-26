
Imports Myrh.Graphics.Rendering.Instructions
Imports Myrh.Graphics.Rendering.Instructions.Primitives
Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Preprocessing
    Public Class ZOrdering : Inherits Preprocessor

        Private Class PathNode

            Private Shared __current_id As Integer = 0
            Private Shared Function __next_id() As Integer
                Dim retval As Integer = __current_id
                __current_id += 1
                Return retval
            End Function

            Public ReadOnly Property ID As Integer
            Public ReadOnly Property Parents As New List(Of PathNode)
            Public ReadOnly Property Children As New List(Of PathNode)
            Public ReadOnly Property Value As Lines

            Public Sub New(value As Lines)
                Me.Value = value
                Me.ID = __next_id()
            End Sub

            Public Function FindValue(value As Lines)
                Dim searched As New List(Of PathNode)
                Return Me._FindValue(value, searched)
            End Function

            Private Function _FindValue(value As Lines, ByRef searched As List(Of PathNode)) As PathNode
                If searched.Any(Function(s) s.ID = Me.ID) Then Return Nothing
                searched.Add(Me)
                If Me.Value.Equals(value) Then Return Me
                Dim retval As PathNode = Nothing
                For Each n As PathNode In Me.Children
                    retval = n._FindValue(value, searched)
                    If retval IsNot Nothing Then Exit For
                Next
                If retval Is Nothing Then
                    For Each n As PathNode In Me.Parents
                        retval = n._FindValue(value, searched)
                        If retval IsNot Nothing Then Exit For
                    Next
                End If
                Return retval
            End Function

            Public Function All() As IEnumerable(Of PathNode)
                Dim retval As New List(Of PathNode)
                Me._All(retval)
                Return retval
            End Function

            Private Sub _All(ByRef retval As List(Of PathNode))
                If retval.Any(Function(s) s.ID = Me.ID) Then Exit Sub
                retval.Add(Me)
                For Each n As PathNode In Me.Children.Concat(Me.Parents)
                    n._All(retval)
                Next
            End Sub

            Public Function GetMaxDepth() As Integer
                Dim paths As New List(Of List(Of PathNode))
                Dim root As New List(Of PathNode) From {Me}
                paths.Add(root)
                Me._GetMaxDepth(root, paths)
                Return paths.Max(Function(s) s.Count)
            End Function

            Private Sub _GetMaxDepth(ByVal current As List(Of PathNode), ByRef paths As List(Of List(Of PathNode)))
                If current.Count > 1 AndAlso current.Take(current.Count - 1).Any(Function(c) c.Equals(Me)) Then Throw New CyclicPathException(current)
                If Me.Parents.Count <> 0 Then
                    For Each n As PathNode In Me.Parents
                        Dim path As New List(Of PathNode)
                        path.AddRange(current)
                        path.Add(n)
                        paths.Add(path)
                        n._GetMaxDepth(path, paths)
                    Next
                    paths.Remove(current)
                End If
            End Sub

        End Class

        Private Class Relation
            Public ReadOnly Property Front As Lines
            Public ReadOnly Property Back As Lines
            Public Sub New(front As Lines, back As Lines)
                Me.Front = front
                Me.Back = back
            End Sub

        End Class

        Private Class CyclicPathException : Inherits Exception
            Public ReadOnly Property Path As IEnumerable(Of PathNode)
            Public Sub New(path As IEnumerable(Of PathNode))
                Me.Path = path
            End Sub
        End Class

        Public Overrides Function Process(list As IEnumerable(Of Instruction)) As IEnumerable(Of Instruction)
            Dim retval As New List(Of Instruction)
            Dim lines As Lines() = list.Where(Function(e) TypeOf (e) Is Lines).Select(Function(t) DirectCast(t, Lines)).ToArray
            Dim relations As New List(Of Relation)
            For i = 0 To lines.Count - 1
                For j = i + 1 To lines.Count - 1
                    If Not _overlap_lines(lines(i), lines(j)) Then Continue For
                    Dim inters As Vertex = _intersect_lines(lines(i), lines(j))
                    If _is_overlap(inters.X) And _is_overlap(inters.Y) Then
                        If inters.Z < inters.W Then
                            relations.Add(New Relation(lines(j), lines(i)))
                        Else
                            relations.Add(New Relation(lines(i), lines(j)))
                        End If
                    End If
                Next
            Next
            retval.Add(list.Where(Function(t) Not relations.Any(Function(r) r.Back.Equals(t) Or r.Front.Equals(t))))
            Try
                Dim toorder As Lines() = list.OfType(Of Lines).Except(retval.OfType(Of Lines)).ToArray
                While toorder.Count <> 0
                    Dim node As New PathNode(toorder.First)
                    _build_tree(relations, node)
                    Dim nodes As IEnumerable(Of PathNode) = node.All
                    toorder = toorder.Except(nodes.Select(Function(n) n.Value)).ToArray
                    retval.Add(nodes.OrderByDescending(Function(n) n.GetMaxDepth).Select(Function(n) n.Value))
                End While
            Catch cex As CyclicPathException
                Dim cycleindex As Integer = cex.Path.Select(Function(n, i) New With {.node = n, .index = i}).First(Function(x) x.node.Equals(cex.Path.Last)).index
                Dim center As Lines = cex.Path(cycleindex + 1).Value
                Dim r1 As Single = _intersect_lines(center, cex.Path(cycleindex).Value).X
                Dim r2 As Single = _intersect_lines(center, cex.Path(cycleindex + 2).Value).X
                Dim c As Vertex = center.Verticies.First + (r1 + r2) / 2 * (center.Verticies.Last - center.Verticies.First)
                Dim newList As New List(Of Instruction)
                Dim p0 As New Lines({center.Verticies.First, c}, center.Color, center.Width, center.Pattern)
                Dim p1 As New Lines({c, center.Verticies.Last}, center.Color, center.Width, center.Pattern)
                newList.Add(list.Except({center}).Concat({p0, p1}))
                retval = Me.Process(newList)
            End Try
            Return retval
        End Function

        Private Shared Sub _build_tree(relations As IEnumerable(Of Relation), node As PathNode)
            Dim value As Lines = node.Value
            Dim children As Lines() = relations.Where(Function(r) r.Front.Equals(value)).Select(Function(r) r.Back).ToArray
            For Each l As Lines In children
                Dim subnode As PathNode = node.FindValue(l)
                If subnode IsNot Nothing Then
                    _addistinct(subnode.Parents, node)
                    _addistinct(node.Children, subnode)
                Else
                    subnode = New PathNode(l)
                    _addistinct(subnode.Parents, node)
                    _addistinct(node.Children, subnode)
                    _build_tree(relations, subnode)
                End If
            Next
            Dim parents As Lines() = relations.Where(Function(r) r.Back.Equals(value)).Select(Function(r) r.Front).ToArray
            For Each l As Lines In parents
                Dim subnode As PathNode = node.FindValue(l)
                If subnode IsNot Nothing Then
                    _addistinct(subnode.Children, node)
                    _addistinct(node.Parents, subnode)
                Else
                    subnode = New PathNode(l)
                    _addistinct(subnode.Children, node)
                    _addistinct(node.Parents, subnode)
                    _build_tree(relations, subnode)
                End If
            Next
        End Sub

        Private Shared Sub _addistinct(list As List(Of PathNode), node As PathNode)
            If Not list.Contains(node) Then list.Add(node)
        End Sub

        Private Shared Function _is_overlap(value As Single) As Boolean
            Return value - Single.Epsilon > 0 And value + Single.Epsilon < 1
        End Function

        Private Shared Function _intersect_lines(l1 As Lines, l2 As Lines) As Vertex
            Dim p1 As Vertex = l1.Verticies.First
            Dim p2 As Vertex = l2.Verticies.First
            Dim q1 As Vertex = l1.Verticies.Last - l1.Verticies.First
            Dim q2 As Vertex = l2.Verticies.Last - l2.Verticies.First
            Dim s As Single = ((p1.Y - p2.Y) + (p2.X - p1.X) * q1.Y / q1.X) / (q2.Y - q2.X * q1.Y / q1.X)
            Dim r As Single = (p2.X - p1.X) / q1.X + s * q2.X / q1.X
            Return New Vertex(r, s, (p1 + r * q1).Z, (p2 + s * q2).Z)
        End Function

        Private Shared Function _rect_from_line(line As Lines) As Single()
            Return {
                Math.Min(line.Verticies.First.X, line.Verticies.Last.X),
                Math.Min(line.Verticies.First.Y, line.Verticies.Last.Y),
                Math.Max(line.Verticies.First.X, line.Verticies.Last.X),
                Math.Max(line.Verticies.First.X, line.Verticies.Last.Y)
            }
        End Function

        Private Shared Function _overlap_lines(l1 As Lines, l2 As Lines) As Boolean
            Dim r1 As Single() = _rect_from_line(l1)
            Dim r2 As Single() = _rect_from_line(l2)
            Return r1(0) < r2(2) And r1(2) > r2(0) And r1(3) > r2(1) And r1(1) < r2(3)
        End Function

    End Class

End Namespace
