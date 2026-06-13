using Godot.Sharp.RemoteTree.Inspector.BaseTypes;

namespace Godot.Sharp.RemoteTree.Inspector;

public partial class InspectorPanel : PanelContainer
{
    private LineEdit _filterField;
    private VBoxContainer _propertyContainer;
    private Label _currentObjectLabel;
    private Node? _currentEditedObject;

    public Theme? IconTheme { get; set; }

    public InspectorPanel()
    {
        Name = "InspectorPanel";

        var rootVBox = new VBoxContainer();
        rootVBox.SizeFlagsVertical = SizeFlags.ExpandFill;
        rootVBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;

        var header = new HBoxContainer();
        header.SizeFlagsHorizontal = SizeFlags.ExpandFill;

        _currentObjectLabel = new Label
        {
            Text = "Select an object to inspect",
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        rootVBox.AddChild(_currentObjectLabel);

        var searchBar = new HBoxContainer();
        searchBar.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        var searchLabel = new Label
        {
            Text = "Search:",
            SizeFlagsHorizontal = SizeFlags.Fill | SizeFlags.ShrinkBegin,
            CustomMinimumSize = new Vector2(56, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        searchBar.AddChild(searchLabel);
        _filterField = new LineEdit()
        {
            PlaceholderText = "Filter properties...",
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        _filterField.TextChanged += OnFilterChanged;

        searchBar.AddChild(_filterField);
        rootVBox.AddChild(searchBar);

        var sectionScroll = new ScrollContainer
        {
            SizeFlagsVertical = SizeFlags.ExpandFill,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            VerticalScrollMode = ScrollContainer.ScrollMode.Auto,
        };

        _propertyContainer = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
            CustomMinimumSize = new Vector2(0, 200),
        };
        sectionScroll.AddChild(_propertyContainer);
        rootVBox.AddChild(sectionScroll);
        AddChild(rootVBox);
    }

    public override void _ExitTree()
    {
        _currentEditedObject = null;
        IconTheme = null;
    }

    public void SetObject(Node? node)
    {
        _currentEditedObject = node;
        RefreshForSelection();
    }

    private void OnFilterChanged(string filter)
    {
        ApplyFilter(filter.Trim().ToLower());
    }

    // TODO: Refactor this into a better method that doesn't query every object for 'meta' value with key 'property_name'
    // TODO: to prevent errors from occuring.
    private void ApplyFilter(string text)
    {
        foreach (var child in _propertyContainer.GetChildren())
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
                case CategoryContainer category:
                    category.ApplyFilter(text);
                    break;
                case Control control when string.IsNullOrEmpty(text) || control.Name.ToString().ToLower().Contains(text) || control.HasMeta("property_name") &&
                    control.GetMeta("property_name").AsString().ToLower().Contains(text):
                    control.Visible = true;
                    break;
            }
        }
    }

    private void RefreshForSelection()
    {
        if (_currentEditedObject == null)
        {
            _currentObjectLabel.Text = "Select an object to inspect";
            RebuildProperties(null);
            return;
        }
        _currentObjectLabel.Text = $"{_currentEditedObject?.Name} ({_currentEditedObject?.GetClass()})";
        RebuildProperties(_currentEditedObject);
    }

    private void RebuildProperties(Node? obj)
    {
        ClearPropertyContainer();
        if (obj == null)
            return;

        var propertyList = obj.GetPropertyList();
        CategoryContainer? lastCategory = null;
        GroupContainer? lastGroup = null;
        GroupContainer? lastSubGroup = null;
        foreach (var property in propertyList)
        {
            if (property is not { } propertyInfo) continue;
            var gpi = GodotPropertyInfo.FromDictionary(obj, propertyInfo);

            switch (gpi.Usage)
            {
                case PropertyUsageFlags.Category:
                    lastGroup = null;
                    lastSubGroup = null;
                    Texture2D? icon = null;
                    if (IconTheme != null && IconTheme.HasIcon(gpi.Name, "EditorIcons"))
                        icon = IconTheme.GetIcon(gpi.Name, "EditorIcons");
                    else if (IconTheme != null)
                    {
                        var script = obj.Get("script").As<Script>();
                        if (script is CSharpScript)
                            icon = IconTheme.GetIcon("CSharpScript", "EditorIcons");
                        else if (script is GDScript)
                            icon = IconTheme.GetIcon("GDScript", "EditorIcons");
                    }

                    var cat = new CategoryContainer
                    {
                        Title = gpi.Name,
                        Icon = icon,
                        SizeFlagsHorizontal = SizeFlags.ExpandFill,
                    };
                    _propertyContainer.AddChild(cat);
                    lastCategory = cat;
                    break;
                case PropertyUsageFlags.Group:
                    lastSubGroup = null;
                    var group = new GroupContainer
                    {
                        Title = gpi.Name,
                        SizeFlagsHorizontal = SizeFlags.ExpandFill,
                        Folded = true,
                    };
                    _propertyContainer.AddChild(group);
                    lastGroup = group;
                    break;
                case PropertyUsageFlags.Subgroup:
                    var subGroup = new GroupContainer
                    {
                        Title = gpi.Name,
                        SizeFlagsHorizontal = SizeFlags.ExpandFill,
                        Folded = true,
                    };
                    if (lastGroup == null)
                    {
                        GD.PushWarning("We have a subgroup, without a parent group!  Adding it as a normal group!");
                        _propertyContainer.AddChild(subGroup);
                        lastGroup = subGroup;
                    }
                    else
                    {
                        lastGroup.AddItem(subGroup);
                        lastSubGroup = subGroup;
                    }
                    break;

                default:
                    var row = CreateEditorControl(gpi);
                    
                    row.Name = gpi.Name;
                    row.SetMeta("property_name", gpi.Name);
                    
                    if (lastSubGroup != null)
                        lastSubGroup.AddItem(row);
                    else if (lastGroup != null)
                        lastGroup.AddItem(row);
                    else if (lastCategory != null)
                        lastCategory.AddItem(row);
                    else
                        _propertyContainer.AddChild(row);
                    break;
            }
        }
        
        if (!string.IsNullOrEmpty(_filterField.Text))
            ApplyFilter(_filterField.Text.Trim().ToLower());
    }

    private void ClearPropertyContainer()
    {
        foreach (var child in _propertyContainer.GetChildren())
            child.QueueFree();
    }
    

    private InspectorEditor CreateEditorControl(GodotPropertyInfo gpi)
    {
        var value = gpi.Get();
        if (value.Obj is null)
            return new NullEditor { GodotPropertyInfo = gpi };

        var editor = value.VariantType switch
        {
            Variant.Type.Bool => new BooleanEditor { GodotPropertyInfo = gpi },
            Variant.Type.Int => CreateIntControl(gpi),
            Variant.Type.Float => new NumericEditor { GodotPropertyInfo = gpi },
            Variant.Type.String => new StringEditor { GodotPropertyInfo = gpi },
            Variant.Type.Vector2 or Variant.Type.Vector2I or Variant.Type.Vector3 or Variant.Type.Vector3I or 
                Variant.Type.Vector4 or Variant.Type.Vector4I => new VectorEditor { GodotPropertyInfo = gpi },
            Variant.Type.Color => new ColorEditor { GodotPropertyInfo = gpi },
            _ => new StringEditor { GodotPropertyInfo = gpi }
        };
        
        var actionButton = new Button
        {
            Text = "...",
            SizeFlagsHorizontal = SizeFlags.Fill | SizeFlags.ShrinkEnd,
            Flat = true,
        };

        

        actionButton.Pressed += () =>
        {
            var popup = new PopupMenu();
            popup.AddItem("Copy GDScript Variable Name", 0);
            popup.AddItem("Copy C# Variable Name", 1);
            popup.AddItem("Copy NodePath", 2);
            popup.AddItem("Revert Value", 3);
            actionButton.AddChild(popup);
            var pos = new Vector2I((int)GetGlobalMousePosition().X, (int)GetGlobalMousePosition().Y);
            popup.Popup(new Rect2I(pos, Vector2I.Zero));
            popup.IdPressed += (id) =>
            {
                switch (id)
                {
                    case 0:
                        DisplayServer.ClipboardSet(gpi.Name);
                        break;
                    case 1:
                        DisplayServer.ClipboardSet(gpi.Name.ToPascalCase());
                        break;
                    case 2:
                        if (gpi.Target == null)
                            DisplayServer.ClipboardSet(gpi.Name);
                        else if (gpi.Target is Node node)
                        {
                            var path = $"{node.Name}:{gpi.Name}";
                            var cnode = node;
                            while (cnode != GetTree().Root)
                            {
                                cnode = cnode.GetParent();
                                path = $"{cnode.Name}/{path}";
                            }

                            path = $"/{path}";
                            DisplayServer.ClipboardSet(path);
                        }
                        break;
                    case 3:
                        gpi.RestoreValue();
                        editor.UpdateFields();
                        break;
                }
            };
        };

        editor.AddChild(actionButton);
        
        return editor;
    }

    private InspectorEditor CreateIntControl(GodotPropertyInfo gpi)
    {
        return gpi.Hint switch
        {
            PropertyHint.Enum => new EnumEditor { GodotPropertyInfo = gpi },
            PropertyHint.Flags => new FlagsEditor { GodotPropertyInfo = gpi },
            PropertyHint.Layers2DRender or PropertyHint.Layers2DNavigation or PropertyHint.Layers2DPhysics or
                PropertyHint.Layers3DRender or PropertyHint.Layers3DNavigation or PropertyHint.Layers3DPhysics or 
                PropertyHint.LayersAvoidance => new LayersEditor { GodotPropertyInfo = gpi },
            
            _ => new NumericEditor { GodotPropertyInfo = gpi }
        };
    }
}
