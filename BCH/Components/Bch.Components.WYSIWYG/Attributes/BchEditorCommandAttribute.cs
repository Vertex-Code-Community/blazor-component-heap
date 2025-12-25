namespace Bch.Components.WYSIWYG.Attributes;

public class BchEditorCommandAttribute : Attribute
{
    public string Command { get; }
    public int BitShift { get; }

    public BchEditorCommandAttribute(string command, int bitShift = 0)
    {
        Command = command;
        BitShift = bitShift;
    }
}