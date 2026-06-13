namespace Godot.Sharp.RemoteTree.Inspector.BaseTypes;

public partial class EnumEditor : InspectorEditor
{
    private OptionButton _enumValues;
    private bool _programatically;
    
    public EnumEditor()
    {
        _enumValues = new OptionButton
        {
            SizeFlagsHorizontal = SizeFlags.Expand | SizeFlags.ShrinkBegin,
            
        };
        _enumValues.ItemSelected += HandleEnumSelected;
        AddChild(_enumValues);
    }

    public override void UpdateFields()
    {
        base.UpdateFields();
        _programatically = true;
        var values = _gpi?.HintString.Split(",") ?? [];
        _enumValues.Clear();
        var i = 0;
        var val = _gpi?.Get().AsInt32();
        foreach (var enumVal in values)
        {
            if (enumVal.Contains(':'))
            {
                var parts = enumVal.Split(':');
                var enumName = parts[0];
                var value = parts[1].ToInt();
                _enumValues.AddItem(enumName, value);
                if (val == value)
                    _enumValues.Select(_enumValues.ItemCount-1);
            }
            else
            {
                _enumValues.AddItem(enumVal, i);
                if (val == i)
                    _enumValues.Select(i);
                i++;
            }
        }
        _programatically = false;
    }

    private void HandleEnumSelected(long index)
    {
        if (_programatically) return;
        var id = _enumValues.GetItemId((int)index);
        _gpi?.Set(id);
    }
}