Imports Myrh.Graphics.Rendering
Imports Myrh.Graphics.Rendering.Model
Imports Myrh.Graphics.Rendering.ModelViews
Imports Myrh.Graphics.Rendering.Contexts
Imports Myrh.Graphics.Rendering.Projections
Imports Myrh.Graphics.Rendering.Instructions

Public Class UserControl1
    Private Sub UserControl1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim page As PageSetup = PageSetup.DIN.A4.Flipped
        Dim doc As New Document("test", page, ModelView.Default, New P2DFlat(page.AspectRatio, 1.0))
        doc.AttachContexts({ContextFactory.Bitmap, ContextFactory.MetaFile})

        Dim instructions As New List(Of Instruction) From {
            New ClearModelView,
            New Primitives.Lines({New Vertex(-0.5, -0.5), New Vertex(0.5, 0.5)}, Color.FromARGB(255, 255, 0, 0), 2.0F, {1.0F})
        }
        doc.Execute(instructions)

        Me.ClientSize = page.Device.ToSize
        Me.BackColor = Drawing.Color.White
        Me.BackgroundImageLayout = ImageLayout.Center
        Me.BackgroundImage = DirectCast(doc.AttachedContexts.First, Bitmap)
        doc.Save("D:\Nutzer\Documents\Visual Studio 2017\Projects\Myrh")
    End Sub

End Class
