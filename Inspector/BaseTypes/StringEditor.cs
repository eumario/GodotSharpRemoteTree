namespace Godot.Sharp.RemoteTree.Inspector.BaseTypes;

public partial class StringEditor : InspectorEditor
{
    private LineEdit _normalEdit;
    private TextEdit _multiEdit;
    private bool _programatically;

    public StringEditor() : base()
    {
        _normalEdit = new LineEdit
        {
            Text = _gpi?.Get().AsString() ?? "",
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        _multiEdit = new TextEdit
        {
            Text = _gpi?.Get().AsString() ?? "",
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            WrapMode = TextEdit.LineWrappingMode.Boundary,
            CustomMinimumSize = new Vector2(0, 120),
        };
        _multiEdit.Visible = false;
        _normalEdit.TextChanged += HandleTextChanged;
        _multiEdit.TextChanged += () => HandleTextChanged(_multiEdit.Text);
        AddChild(_normalEdit);
        AddChild(_multiEdit);
    }

    private void HandleTextChanged(string text)
    {
        if (!_programatically)
            _gpi?.Set(text);
    }

    public override void UpdateFields()
    {
        base.UpdateFields();
        if (_gpi != null)
        {
            _normalEdit.Visible = _gpi.Hint != PropertyHint.MultilineText;
            _multiEdit.Visible = _gpi.Hint == PropertyHint.MultilineText;
        }
        _programatically = true;
        _normalEdit.Text = _gpi?.Get().AsString() ?? "";
        _multiEdit.Text = _gpi?.Get().AsString() ?? "";
        _programatically = false;
    }
}
