namespace Godot.Sharp.RemoteTree.Inspector.BaseTypes;

public partial class InspectorEditor : HBoxContainer
{
    internal GodotPropertyInfo? _gpi;

    private Vector2 _labelCustomMinimumSize = new(160, 0);
    public Vector2 LabelCustomMinimumSize
    {
        get => _labelCustomMinimumSize;
        set
        {
            _labelCustomMinimumSize = value;
            if (_titleLabel != null)
                _titleLabel.CustomMinimumSize = value;
        }
    }

    public GodotPropertyInfo? GodotPropertyInfo
    {
        get => _gpi;
        set
        {
            _gpi = value;
            UpdateFields();
        }
    }

    public InspectorEditor()
    {
        _titleLabel = new Label
        {
            Text = _gpi?.Name.Replace("_", " ").Capitalize() ?? "Unknown",
            SizeFlagsHorizontal = SizeFlags.ShrinkBegin,
            CustomMinimumSize = LabelCustomMinimumSize,
            MouseFilter = MouseFilterEnum.Stop,
        };
        AddChild(_titleLabel);
    }

    private Label _titleLabel;

    public override void _Ready() => UpdateFields();

    public virtual void UpdateFields()
    {
        _titleLabel.Text = _gpi == null ? "Unknown" : _gpi.Name.Replace("_", " ").Capitalize();
        _titleLabel.TooltipText = _gpi == null ? "Unknown" : _gpi.Name;
    }
}
