namespace Godot.Sharp.RemoteTree.Inspector;

public partial class GroupContainer : FoldableContainer
{
    private VBoxContainer _container;

    public GroupContainer()
    {
        _container = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        AddChild(_container);
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
