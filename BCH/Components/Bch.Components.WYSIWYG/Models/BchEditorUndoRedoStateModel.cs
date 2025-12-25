using Bch.Modules.GlobalEvents.Models;

namespace Bch.Components.WYSIWYG.Models;

public class BchEditorUndoRedoStateModel
{
    public bool Undo { get; set; }
    public bool Redo { get; set; }
    public List<ElementParameters> PathCoordinates { get; set; } = new();
}