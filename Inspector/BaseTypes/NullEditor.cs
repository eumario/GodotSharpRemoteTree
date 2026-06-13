namespace Godot.Sharp.RemoteTree.Inspector.BaseTypes;

public partial class NullEditor : InspectorEditor
{
    public NullEditor() : base()
    {
        AddChild(new Label { Text = "<null>", SizeFlagsHorizontal = SizeFlags.Expand | SizeFlags.ShrinkBegin });
    }
}
