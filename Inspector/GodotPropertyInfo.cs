using System.Text;
using Godot.Collections;
namespace Godot.Sharp.RemoteTree.Inspector;

public class GodotPropertyInfo
{
    private GodotObject? Object { get; init; }
    public string Name { get; set; } = string.Empty;
    public string ClassName { get; init; } = string.Empty;
    public Variant.Type Type { get; set; }
    public PropertyHint Hint { get; set; }
    public string HintString { get; set; } = string.Empty;
    public PropertyUsageFlags Usage { get; set; }
    public Func<Variant>? Getter { get; set; }
    public Action<Variant>? Setter { get; set; }

    public GodotObject? Target => Object;

    private Variant? _origValue;

    public static GodotPropertyInfo FromDictionary(GodotObject obj, Dictionary dict)
    {
        var gpi = new GodotPropertyInfo
        {
            Object = obj,
            Name = dict["name"].ToString(),
            ClassName = dict.ContainsKey("class_name") ? dict["class_name"].AsString() : string.Empty,
            Type = dict["type"].As<Variant.Type>(),
            Hint = dict["hint"].As<PropertyHint>(),
            HintString = dict.ContainsKey("hint_string") ? dict["hint_string"].AsString() : string.Empty,
            Usage = dict.ContainsKey("usage") ? dict["usage"].As<PropertyUsageFlags>() : PropertyUsageFlags.None
        };
        
        return gpi;
    }

    public Variant Get()
    {
        if (Getter == null)
            return Object?.Get(Name) ?? default;
        else
            return Getter.Invoke();
    }
    public void Set(Variant value)
    {
        _origValue ??= Get();
        if (Setter == null)
            Object?.Set(Name, value);
        else
            Setter.Invoke(value);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"<GodotPropertyInfo: Prop: {Name}");
        if (!string.IsNullOrEmpty(ClassName))
            sb.Append($", ClassName: {ClassName}");
        sb.Append('>');        
        return sb.ToString();
    }

    public void RestoreValue()
    {
        if (_origValue != null)
            Set(_origValue.Value);
    }

}
