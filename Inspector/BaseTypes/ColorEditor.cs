namespace Godot.Sharp.RemoteTree.Inspector.BaseTypes;

public partial class ColorEditor : InspectorEditor
{
    private ColorPickerButton _colorButton;

    public ColorEditor() : base()
    {
        _colorButton = new ColorPickerButton
        {
            SizeFlagsHorizontal = SizeFlags.ShrinkBegin,
            CustomMinimumSize = new Vector2(80,30),
        };

        AddChild(_colorButton);

        _colorButton.ColorChanged += (color) => _gpi?.Set(color);
    }

    public override void UpdateFields()
    {
        base.UpdateFields();
        _colorButton.Color = _gpi?.Get().AsColor() ?? Colors.White;
    }
}
