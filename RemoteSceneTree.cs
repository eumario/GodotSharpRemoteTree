using System.Reflection;
using Godot.Sharp.RemoteTree.Inspector;

namespace Godot.Sharp.RemoteTree;

public partial class RemoteSceneTree : Window
{
    private static RemoteSceneTree? _instance;
    private HSplitContainer _container;
    private Tree _sceneTree;
    private LineEdit _filterProperties;
    private InspectorPanel _inspectorPanel;
    private Theme? _theme;

    private static readonly StringName EditorIcons = nameof(EditorIcons);

    private TreeItem? _root;
    private readonly Dictionary<Node, TreeItem> _treeItems = [];
    private readonly Dictionary<TreeItem, Node> _treeNodes = [];
    private readonly Callable _enterTree;
    private readonly Callable _exitTree;

    public static void Enable()
    {
        if (!OS.HasFeature("editor")) return;
        _instance = new RemoteSceneTree();
        var sceneTree = (SceneTree?)Engine.GetMainLoop();
        sceneTree?.Root.CallDeferred(Node.MethodName.AddChild, _instance);
    }

    public static void Disable()
    {
        if (_instance == null) return;
        _instance.QueueFree();
        _instance = null;
    }

    private RemoteSceneTree()
    {
        _enterTree = Callable.From<Node>(HandleChildEntered);
        _exitTree = Callable.From<Node>(HandleChildExited);
        Visible = false;
        ForceNative = true;
        AlwaysOnTop = false;
        PopupWindow = false;
        Transient = false;
        Size = new Vector2I(800, 500);
        Name = "_RemoteSceneTree";
        Title = "Remote Scene Tree Inspector";
        _container = new HSplitContainer();
        _container.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _sceneTree = new Tree();
        _sceneTree.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.LeftWide);
        _sceneTree.CustomMinimumSize = new Vector2(200, 0);
        _inspectorPanel = new InspectorPanel();
        _filterProperties = new LineEdit();
        _filterProperties.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.TopWide);
        _theme = LoadAssemblyTheme();
        _inspectorPanel.IconTheme = _theme;
    }

    public override void _ExitTree()
    {
        _theme?.Dispose();
        _theme = null;
        _treeItems.Clear();
        _treeNodes.Clear();
        foreach (var item in _root?.GetChildren() ?? [])
        {
            _root?.RemoveChild(item);
        }
        _root = null;
    }

    private Theme? LoadAssemblyTheme()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("Godot.Sharp.RemoteTree.remote_debugger_tree.res");
        if (stream == null)
        {
            GD.PushError("Unable to find Remote Debugger Tree Theme.");
            return null;
        }
        using var reader = new BinaryReader(stream);
        var data = reader.ReadBytes((int)stream.Length);
        var path = OS.GetTempDir();
        File.WriteAllBytes(path.PathJoin("remote_debugger_tree.res"), data);
        return ResourceLoader.Load<Theme>(path.PathJoin("remote_debugger_tree.res"));
    }

    public override void _Ready()
    {
        _container.AddChild(_sceneTree);
        _container.AddChild(_inspectorPanel);
        AddChild(_container);
        CloseRequested += () => Visible = false;
        GetTree().Root.ChildEnteredTree += HandleChildEntered;
        GetTree().Root.ChildExitingTree += HandleChildExited;
        GetTree().Root.WindowInput += HandleInput;
        _sceneTree.ItemSelected += HandleSceneTreeItemSelected;
        InitTree();
    }

    private void HandleInput(InputEvent @event)
    {
        if (@event is not InputEventKey { Pressed: true } keyEvent || keyEvent.Keycode != Key.F11) return;
        PopupCentered();
    }

    private void HandleChildEntered(Node node)
    {
        if (node == null) return;
        
        if (node == this || node.Name == "_RemoteSceneTree")
        {
            _treeNodes.Clear();
            _treeItems.Clear();
            QueueFree();
            return;
        }
        EnsureSignalConnected(node);
        if (_treeItems.ContainsKey(node)) return;
        var parent = node.GetParent();
        if (parent is null)
        {
            GD.PushError($"Node doesn't have a Parent! {node.Name}");
            return;
        }

        if (_treeItems.TryGetValue(parent, out var parentItem))
        {
            PopulateChildren(node, parentItem);
        }
        else
        {
            GD.PushError($"Node {parent.Name} doesn't have a TreeItem. (Called from: {node.Name})");
            PopulateChildren(node, _root!);
        }
    }

    private void HandleChildExited(Node node)
    {
        if (node == this || node.Name == "_RemoteSceneTree") return;

        EnsureSignalDisconnected(node);
        if (!_treeItems.Remove(node, out var nodeItem)) return;
        _treeNodes.Remove(nodeItem);
        nodeItem.GetParent().RemoveChild(nodeItem);
    }

    private void InitTree()
    {
        _sceneTree.Clear();
        _root = _sceneTree.CreateItem();
        _root.SetText(0, "root");
        _root.SetIcon(0, _theme?.GetIcon("Window", EditorIcons));
        _treeItems[GetTree().Root] = _root;
        _treeNodes[_root] = GetTree().Root;
        foreach (var node in GetTree().Root.GetChildren())
        {
            if (node.Name == "_RemoteSceneTree" || node == this) continue;
            PopulateChildren(node, _root);
        }
    }

    private void PopulateChildren(Node node, TreeItem item)
    {
        var childClass = node.GetClass();
        var icon = _theme?.GetIcon(childClass, EditorIcons);
        while (icon == null)
        {
            childClass = ClassDB.GetParentClass(childClass);
            icon = _theme?.GetIcon(childClass, EditorIcons);
        }

        EnsureSignalConnected(node);
        var childItem = item.CreateChild();
        childItem.SetText(0, node.Name);
        childItem.SetIcon(0, icon);
        childItem.Collapsed = true;
        _treeItems[node] = childItem;
        _treeNodes[childItem] = node;
        if (node.GetChildCount() == 0) return;
        foreach (var child in node.GetChildren())
        {
            PopulateChildren(child, childItem);
        }
    }

    private void EnsureSignalConnected(Node node)
    {
        if (!node.IsConnected(Node.SignalName.ChildEnteredTree, _enterTree))
            node.Connect(Node.SignalName.ChildEnteredTree, _enterTree);
        if (!node.IsConnected(Node.SignalName.ChildExitingTree, _exitTree))
            node.Connect(Node.SignalName.ChildExitingTree, _exitTree);
    }

    private void EnsureSignalDisconnected(Node node)
    {
        if (node.IsConnected(Node.SignalName.ChildEnteredTree, _enterTree))
            node.Disconnect(Node.SignalName.ChildEnteredTree, _enterTree);
        if (node.IsConnected(Node.SignalName.ChildExitingTree, _exitTree))
            node.Disconnect(Node.SignalName.ChildExitingTree, _exitTree);
    }

    private void HandleSceneTreeItemSelected()
    {
        var item = _sceneTree.GetSelected();
        if (item == null) return;
        _treeNodes.TryGetValue(item, out Node? node);
        _inspectorPanel.SetObject(node);
    }
}
