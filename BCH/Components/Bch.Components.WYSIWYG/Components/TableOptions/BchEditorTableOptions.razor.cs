using Microsoft.AspNetCore.Components;

namespace Bch.Components.WYSIWYG.Components.TableOptions;

public partial class BchEditorTableOptions
{
    [Parameter] public EventCallback OnInsertRowAbove { get; set; }
    [Parameter] public EventCallback OnInsertRowBelow { get; set; }
    [Parameter] public EventCallback OnRemoveSelectedRow { get; set; }
    [Parameter] public EventCallback OnInsertColumnToTheLeft { get; set; }
    [Parameter] public EventCallback OnInsertColumnToTheRight { get; set; }
    [Parameter] public EventCallback OnRemoveSelectedColumn { get; set; }
}