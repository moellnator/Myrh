Imports Myrh.Graphics.Rendering.Instructions.Primitives
Imports Myrh.Graphics.Rendering.Model

Namespace Rendering.Contexts
    Public MustInherit Class Context : Implements IDisposable

        Private _is_disposed As Boolean
        Public ReadOnly Property Display As Display

        Public Sub New(display As Display)
            Me.Display = display
        End Sub
        Public MustOverride Sub Save(Filename As String)

        Public MustOverride Sub Primitive(line As Lines, engine As Engine)
        Public MustOverride Sub Primitive(text As Text, engine As Engine)
        Public MustOverride Sub Primitive(filled As Filled, engine As Engine)

        Protected Sub Dispose(disposing As Boolean)
            If Not Me._is_disposed Then
                If disposing Then
                    Me._dispose()
                End If
            End If
            Me._is_disposed = True
        End Sub

        Public MustOverride ReadOnly Property DefaultExtension As String

        Protected MustOverride Sub _dispose()

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub


    End Class

End Namespace
