# Godot.Sharp.RemoteTree
A library to create a UI Control that can be ran from within a Godot Application, to inspect the SceneTree at runtime in C#/Dotnet release of Godot.  This is a tool that will let you
inspect the SceneTree of a Godot Application, at runtime.  It is similar to tools such as AvaloniaUI DevTools, allowing you to inspect the SceneTree at runtime, inspecting various
nodes that exist in the tree.

Godot already has a Remote SceneTree inspector, which is fine, if you run your Project from Godot itself.  However, this will let you run your project directly from a C# IDE such as
Rider, VSCode, and still give you access to the SceneTree for your application/game.  It is not a 1:1 replication of the Inspector panel in the Godot Editor, more controls need to be
added in order to have as close to 1:1 replication.

# Installation
To install this library, simply use dotnet command line tool, to add a package reference for this library.

```
dotnet add package Godot.Sharp.RemoteTree
```

Alternatively, you can add to your CSProj file, the following:

```xml
<ItemGroup Condition="'$(Configuration)' == 'Debug'">
  <PackageReference Include="Godot.Sharp.RemoteTree"/>
</ItemGroup>
```

This will add the package reference for inclusion only when building your project in Debug configuration, and will not include it in your release builds.

# Usage
In order to use the Library, you will need to register the RemoteTree in your source code, so that it can initalize and setup the RemoteSceneTree.  This can
be enabled, by doing the following in your Main Scene script, in the _Ready() function or the _EnterTree() function.

```cs
using Godot;
using Godot.Sharp.RemoteTree;

public partial class MyNode : Node
{
	public override _Ready()
	{
		RemoteSceneTree.Enable();
		// Initialize all your other stuff for your node.
	}
	
	public override _ExitTree()
	{
		RemoteSceneTree.Disable();
	}
}
```

As seen above, cleanup is handled by using RemoteSceneTree.Disable().  When running RemoteSceneTree.Enable(), it will listen for the 'F11' key to show the
RemoteSceneTree window.  In the future, this will be configurable.  The RemoteSceneTree is editable of the current properties to a certain extent.  Properties
that are currently implemented, will let you adjust values of the current running process, but only for Properties that are currently implemented in the
RemoteSceneTree, and will only effect the current run of the application.  It does not save the values to your project.

## Note:
The same limitation that you normally have with C# and Godot with Variable types work the exact same here.  Currently, it cannot display C# types, that do not
inherit from GodotObject class.  Also, currently there is no support for Resource inspection, and will just be displayed as a string of <classname:-instance_id>.
Resource Inspection will be added in the future.