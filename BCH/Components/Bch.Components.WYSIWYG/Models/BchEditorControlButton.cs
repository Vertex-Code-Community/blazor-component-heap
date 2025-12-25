using Bch.Components.WYSIWYG.Attributes;

namespace Bch.Components.WYSIWYG.Models;

public enum BchEditorControlButton
{
    [BchEditorCommand("bold", 0)] Bold,
    [BchEditorCommand("italic", 1)] Italic,
    [BchEditorCommand("underline", 2)] Underline,

    [BchEditorCommand("undo", 3)] Undo,
    [BchEditorCommand("redo", 4)] Redo,

    [BchEditorCommand("justifyLeft", 5)] JustifyLeft,
    [BchEditorCommand("justifyRight", 6)] JustifyRight,
    [BchEditorCommand("justifyCenter", 7)] JustifyCenter,
    [BchEditorCommand("formatBlock", 8)] FormatBlock,

    [BchEditorCommand("insertOrderedList", 9)] OrderedList,
    [BchEditorCommand("insertUnorderedList", 10)] UnorderedList,

    [BchEditorCommand("indent", 11)] Indent,
    [BchEditorCommand("outdent", 12)] Outdent,

    [BchEditorCommand("foreColor")] ColorText,
    [BchEditorCommand("insertImage", 13)] InsertImage,
    [BchEditorCommand("unlink", 14)] RemoveLink,
    [BchEditorCommand("createLink")] InsertLink,
    [BchEditorCommand("fontSize")] FontSize,
    [BchEditorCommand("insertHTML")] InsertHTML,

    Insert3Images,
    Insert2Images,
    Insert1Image,

    Separator,

    Source,
    Preview
}