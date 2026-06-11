namespace Godot.Sharp.RemoteTree.Inspector.BaseTypes;

public partial class BooleanEditor : InspectorEditor
{
    private CheckBox _check;

    public BooleanEditor() : base()
    {
        _check = new CheckBox();
        _check.Toggled += pressed => _gpi?.Set(pressed);
        AddChild(_check);
    }

    public override void UpdateFields()
    {
        base.UpdateFields();
        _check.ButtonPressed = _gpi?.Get().AsBool() ?? false;
    }
}
