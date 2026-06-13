namespace Godot.Sharp.RemoteTree.Inspector.BaseTypes;

public partial class NumericEditor : InspectorEditor
{

    private SpinBox _spin;

    public NumericEditor()
    {
        _spin = new SpinBox
        {
            SizeFlagsHorizontal = SizeFlags.Expand | SizeFlags.ShrinkBegin,
        };
        _spin.ValueChanged += HandleValueChanged;
        AddChild(_spin);
    }

    private void HandleValueChanged(double val)
    {
        if (_gpi == null) return;
        if (_gpi.Type == Variant.Type.Int)
            _gpi.Set(Variant.From((int)val));
        else
            _gpi.Set(Variant.From((float)val));
    }

    public override void UpdateFields()
    {
        base.UpdateFields();
        if (_gpi == null) return;
        switch (_gpi.Type)
        {
            case Variant.Type.Int:
                _spin.Value = _gpi.Get().AsInt32();
                _spin.MinValue = int.MinValue;
                _spin.MaxValue = int.MaxValue;
                _spin.Step = 1.0d;
                break;
            case Variant.Type.Float:
                _spin.Value = _gpi.Get().AsSingle();
                _spin.MinValue = -100000.0d;
                _spin.MaxValue = 100000.0d;
                _spin.Step = 0.01d;
                break;
        }
    }
}
