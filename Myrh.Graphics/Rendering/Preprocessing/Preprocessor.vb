Imports Myrh.Graphics.Rendering.Instructions

Namespace Rendering.Preprocessing
    Public MustInherit Class Preprocessor

        Public MustOverride Function Process(list As IEnumerable(Of Instruction)) As IEnumerable(Of Instruction)

    End Class

End Namespace
