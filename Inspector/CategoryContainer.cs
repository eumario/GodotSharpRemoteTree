namespace Godot.Sharp.RemoteTree.Inspector;

public partial class CategoryContainer : PanelContainer
{
    private string _title = string.Empty;
    private Texture2D? _icon = null!;
    [Export]
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            UpdateLabel();
        }
    }

    [Export]
    public Texture2D? Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            UpdateLabel();
        }
    }

    private RichTextLabel _label;
    private VBoxContainer _container;

    public CategoryContainer()
    {
        _label = new RichTextLabel
        {
            BbcodeEnabled = true,
            HorizontalAlignment = HorizontalAlignment.Center,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            FitContent = true,
        };
        var outerContainer = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill
        };
        _container = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        outerContainer.AddChild(_label);
        outerContainer.AddChild(_container);
        AddChild(outerContainer);
    }

    public override void _Ready() => UpdateLabel();

    private void UpdateLabel()
    {
        _label.Clear();
        _label.PushBold();
        if (Icon != null)
            _label.AddImage(Icon);

        _label.AddText(Title);
        _label.PopAll();
    }

    public void AddItem(Control control) => _container.AddChild(control);
    public void GetItems() => _container.GetChildren();
    public void ClearItems()
    {
        foreach (var child in _container.GetChildren())
            child.QueueFree();
    }
    public int ChildCount => _container.GetChildCount();

    public void ApplyFilter(string text)
    {
        foreach (var child in _container.GetChildren())
        {
            switch (child)
            {
                case GroupContainer group:
                    group.ApplyFilter(text);
                    group.Visible = group.AnyEditorVisible();
                    if (string.IsNullOrEmpty(text))
                        group.Folded = true;
                    else
                        group.Folded = !group.AnyEditorVisible();
                    break;
                case Control control when string.IsNullOrEmpty(text) || control.Name.ToString().ToLower().Contains(text) || control.HasMeta("property_name") &&
                    control.GetMeta("property_name").AsString().ToLower().Contains(text):
                    control.Visible = true;
                    break;
                case Control control:
                    control.Visible = false;
                    break;
            }
        }
    }
    
    public bool AnyEditorVisible()
    {
        foreach (var child in _container.GetChildren())
        {
            switch (child)
            {
                case GroupContainer group when group.AnyEditorVisible():
                case Control control when control.Visible:
                    return true;
            }
        }

        return false;
    }
}
