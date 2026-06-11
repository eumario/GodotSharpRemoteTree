namespace Godot.Sharp.RemoteTree.Inspector.BaseTypes;

public partial class VectorEditor : InspectorEditor
{
    private List<Variant.Type> _intTypes = [
        Variant.Type.Vector2I,
        Variant.Type.Vector3I,
        Variant.Type.Vector4I
    ];

    private NumericEditor _xField;
    private NumericEditor _yField;
    private NumericEditor _zField;
    private NumericEditor _wField;

    public VectorEditor()
    {
        SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
        _xField = new NumericEditor { LabelCustomMinimumSize = Vector2.Zero };
        _yField = new NumericEditor { LabelCustomMinimumSize = Vector2.Zero };
        _zField = new NumericEditor { LabelCustomMinimumSize = Vector2.Zero };
        _wField = new NumericEditor { LabelCustomMinimumSize = Vector2.Zero };
        AddChild(_xField);
        AddChild(_yField);
        AddChild(_zField);
        AddChild(_wField);
    }

    public override void UpdateFields()
    {
        base.UpdateFields();
        if (_gpi == null)
        {
            _xField.Visible = false;
            _yField.Visible = false;
            _zField.Visible = false;
            _wField.Visible = false;
            return;
        }
        var propX = new GodotPropertyInfo
        {
            Name = "X",
            Type = _intTypes.Contains(_gpi.Type) ? Variant.Type.Int : Variant.Type.Float,
            Setter = SetXValue,
            Getter = GetXValue,
        };
        var propY = new GodotPropertyInfo
        {
            Name = "Y",
            Type = _intTypes.Contains(_gpi.Type) ? Variant.Type.Int : Variant.Type.Float,
            Setter = SetYValue,
            Getter = GetYValue,
        };

        _xField.GodotPropertyInfo = propX;
        _yField.GodotPropertyInfo = propY;
        _xField.Visible = true;
        _yField.Visible = true;

        if (_gpi.Type == Variant.Type.Vector3 || _gpi.Type == Variant.Type.Vector3I ||
            _gpi.Type == Variant.Type.Vector4 || _gpi.Type == Variant.Type.Vector4I)
        {
            var propZ = new GodotPropertyInfo
            {
                Name = "Z",
                Type = _intTypes.Contains(_gpi.Type) ? Variant.Type.Int : Variant.Type.Float,
                Setter = SetZValue,
                Getter = GetZValue,
            };
            _zField.GodotPropertyInfo = propZ;
        }
        else
        {
            _zField.Visible = false;
        }

        if (_gpi.Type == Variant.Type.Vector4 || _gpi.Type == Variant.Type.Vector4I)
        {
            var propW = new GodotPropertyInfo
            {
                Name = "W",
                Type = _intTypes.Contains(_gpi.Type) ? Variant.Type.Int : Variant.Type.Float,
                Setter = SetWValue,
                Getter = GetWValue,
            };
            _wField.GodotPropertyInfo = propW;
        }
        else
        {
            _wField.Visible = false;
        }
    }

    private Variant GetXValue()
    {
        if (_gpi == null) return default;
        var val = _gpi.Get();
        return _gpi.Type switch
        {
            Variant.Type.Vector2 => val.AsVector2().X,
            Variant.Type.Vector2I => val.AsVector2I().X,
            Variant.Type.Vector3 => val.AsVector3().X,
            Variant.Type.Vector3I => val.AsVector3I().X,
            Variant.Type.Vector4 => val.AsVector4().X,
            Variant.Type.Vector4I => val.AsVector4I().X,
            _ => default
        };
    }

    private void SetXValue(Variant val)
    {
        if (_gpi == null) return;
        var ival = val.AsInt32();
        var fval = val.As<float>();
        var gval = _gpi.Get();
        switch (_gpi.Type)
        {
            case Variant.Type.Vector2:
                _gpi.Set(gval.AsVector2() with { X = fval });
                break;
            case Variant.Type.Vector2I:
                _gpi.Set(gval.AsVector2I() with { X = ival });
                break;
            case Variant.Type.Vector3:
                _gpi.Set(gval.AsVector3() with { X = fval });
                break;
            case Variant.Type.Vector3I:
                _gpi.Set(gval.AsVector3I() with { X = ival });
                break;
            case Variant.Type.Vector4:
                _gpi.Set(gval.AsVector4() with { X = fval });
                break;
            case Variant.Type.Vector4I:
                _gpi.Set(gval.AsVector4I() with { X = ival });
                break;
        }
    }

    private Variant GetYValue()
    {
        if (_gpi == null) return default;
        var val = _gpi.Get();
        return _gpi.Type switch
        {
            Variant.Type.Vector2 => val.AsVector2().Y,
            Variant.Type.Vector2I => val.AsVector2I().Y,
            Variant.Type.Vector3 => val.AsVector3().Y,
            Variant.Type.Vector3I => val.AsVector3I().Y,
            Variant.Type.Vector4 => val.AsVector4().Y,
            Variant.Type.Vector4I => val.AsVector4I().Y,
            _ => default
        };
    }

    private void SetYValue(Variant val)
    {
        if (_gpi == null) return;
        var ival = val.AsInt32();
        var fval = val.As<float>();
        var gval = _gpi.Get();
        switch (_gpi.Type)
        {
            case Variant.Type.Vector2:
                _gpi.Set(gval.AsVector2() with { Y = fval });
                break;
            case Variant.Type.Vector2I:
                _gpi.Set(gval.AsVector2I() with { Y = ival });
                break;
            case Variant.Type.Vector3:
                _gpi.Set(gval.AsVector3() with { Y = fval });
                break;
            case Variant.Type.Vector3I:
                _gpi.Set(gval.AsVector3I() with { Y = ival });
                break;
            case Variant.Type.Vector4:
                _gpi.Set(gval.AsVector4() with { Y = fval });
                break;
            case Variant.Type.Vector4I:
                _gpi.Set(gval.AsVector4I() with { Y = ival });
                break;
        }
    }

    private Variant GetZValue()
    {
        if (_gpi == null) return default;
        var val = _gpi.Get();
        return _gpi.Type switch
        {
            Variant.Type.Vector3 => val.AsVector3().Z,
            Variant.Type.Vector3I => val.AsVector3I().Z,
            Variant.Type.Vector4 => val.AsVector4().Z,
            Variant.Type.Vector4I => val.AsVector4I().Z,
            _ => default
        };
    }

    private void SetZValue(Variant val)
    {
        if (_gpi == null) return;
        var ival = val.AsInt32();
        var fval = val.As<float>();
        var gval = _gpi.Get();
        switch (_gpi.Type)
        {
            case Variant.Type.Vector3:
                _gpi.Set(gval.AsVector3() with { Z = fval });
                break;
            case Variant.Type.Vector3I:
                _gpi.Set(gval.AsVector3I() with { Z = ival });
                break;
            case Variant.Type.Vector4:
                _gpi.Set(gval.AsVector4() with { Z = fval });
                break;
            case Variant.Type.Vector4I:
                _gpi.Set(gval.AsVector4I() with { Z = ival });
                break;
        }
    }

    private Variant GetWValue()
    {
        if (_gpi == null) return default;
        var val = _gpi.Get();
        return _gpi.Type switch
        {
            Variant.Type.Vector4 => val.AsVector4().W,
            Variant.Type.Vector4I => val.AsVector4I().W,
            _ => default
        };
    }

    private void SetWValue(Variant val)
    {
        if (_gpi == null) return;
        var ival = val.AsInt32();
        var fval = val.As<float>();
        var gval = _gpi.Get();
        switch (_gpi.Type)
        {
            case Variant.Type.Vector4:
                _gpi.Set(gval.AsVector4() with { W = fval });
                break;
            case Variant.Type.Vector4I:
                _gpi.Set(gval.AsVector4I() with { W = ival });
                break;
        }
    }
}
