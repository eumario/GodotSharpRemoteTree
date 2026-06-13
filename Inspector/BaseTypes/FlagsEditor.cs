namespace Godot.Sharp.RemoteTree.Inspector.BaseTypes;

public partial class FlagsEditor : InspectorEditor
{
	private VBoxContainer _vbox = null!;
	private List<CheckBox> _flags = [];
	private List<uint> _flagValues = [];
	
	private void SetReadOnly(bool readOnly)
	{
		foreach(var cb in _flags)
		{
			cb.Disabled = readOnly;
		}
	}
	
	private void FlagToggled(int index)
	{
		var value = _gpi?.Get().AsUInt32() ?? 0;
		if (_flags[index].ButtonPressed)
			value |= _flagValues[index];
		else
			value &= _flagValues[index];
		
		_gpi?.Set(value);
	}

    public override void UpdateFields()
    {
        base.UpdateFields();
		var value = _gpi?.Get().AsUInt32() ?? 0;
		if (_flags.Count == 0)
		{
			var options = _gpi?.HintString.Split(",".ToCharArray());
			var bitValue = 1u;
			foreach (var option in options)
			{
				var optName = option;
				var oldBitValue = bitValue;
				if (option.Contains(':'))
				{
					var tmp = option.Split(":".ToCharArray());
					optName = tmp[0];
					bitValue = uint.Parse(tmp[1]);
				}

				_flagValues.Add(bitValue);
				var cb = new CheckBox();
				cb.Text = optName;
				_vbox.AddChild(cb);
				_flags.Add(cb);
				bitValue = oldBitValue;
				bitValue *= 2;
			}
		}
		
		for (var i = 0; i < _flags.Count; i++) {
			_flags[i].ButtonPressed = (value & _flagValues[i]) == _flagValues[i];
		}
    }
	
	public FlagsEditor()
	{
		_vbox = new VBoxContainer
		{
			SizeFlagsHorizontal = SizeFlags.Expand | SizeFlags.ShrinkBegin,
		};
		AddChild(_vbox);
	}
}
