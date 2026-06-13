namespace Godot.Sharp.RemoteTree.Inspector.BaseTypes;

public partial class LayersEditor : InspectorEditor
{

    private LayersGrid _grid;
    private string _baseName = string.Empty;
    private PopupMenu _layers = null!;
    
    public LayersEditor()
    {
        _grid = new LayersGrid
        {
            SizeFlagsHorizontal = SizeFlags.Expand | SizeFlags.ShrinkBegin,
        };
        AddChild(_grid);

        _grid.FlagChanged += GridFlagChanged;
    }

    public override void UpdateFields()
    {
        base.UpdateFields();

        _grid.Value = _gpi?.Get().AsUInt32() ?? 0u;
        
        var layerGroupSize = 0;
        var layerCount = 0;
        
        switch (_gpi?.Hint)
        {
            case PropertyHint.Layers2DRender:
                _baseName = "layer_names/2d_render";
                layerGroupSize = 5;
                layerCount = 20;
                break;
            case PropertyHint.Layers2DPhysics:
                _baseName = "layer_names/2d_physics";
                layerGroupSize = 4;
                layerCount = 32;
                break;
            case PropertyHint.Layers2DNavigation:
                _baseName = "layer_names/2d_navigation";
                layerGroupSize = 4;
                layerCount = 32;
                break;
            case PropertyHint.Layers3DRender:
                _baseName = "layer_names/3d_render";
                layerGroupSize = 5;
                layerCount = 20;
                break;
            case PropertyHint.Layers3DPhysics:
                _baseName = "layer_names/3d_physics";
                layerGroupSize = 4;
                layerCount = 32;
                break;
            case PropertyHint.Layers3DNavigation:
                _baseName = "layer_names/3d_navigation";
                layerGroupSize = 4;
                layerCount = 32;
                break;
            case PropertyHint.LayersAvoidance:
                _baseName = "layer_names/avoidance";
                layerGroupSize = 4;
                layerCount = 32;
                break;
        }

        _grid.Names.Clear();
        _grid.Tooltips.Clear();

        for (var i = 0; i < layerCount; i++)
        {
            var name = string.Empty;
            if (ProjectSettings.Singleton.HasSetting($"{_baseName}/layer_{i + 1}"))
                name = ProjectSettings.Singleton.GetSetting($"{_baseName}/layer_{i + 1}", string.Empty).AsString();

            if (string.IsNullOrEmpty(name))
                name = $"Layer {i + 1}";

            _grid.Names.Add(name);
            _grid.Tooltips.Add($"{name}\nBit {i}, value {1u << i}");
        }
        _grid.LayerGroupSize = layerGroupSize;
        _grid.LayerCount = (uint)layerCount;
    }

    private void GridFlagChanged(uint flag)
    {
        _gpi?.Set(flag);
    }
}