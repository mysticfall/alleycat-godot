[![AllyCat logo](logo.png)](https://github.com/mysticfall/AlleyCat)

## Introduction
_Alley Cat_ is a programmer friendly game framework for [Godot](https://github.com/godotengine/godot) engine.

## Status
It's highly experimental at this stage, so don't even think about using it for anything serious yet!

If you feel adventurous though, you are welcome to test it and file bug reports, or even send pull requests.

## Usage

### Installation

Actually, there's no easy way to use this framework in your project yet. The reason is, Godot 3.0 
still lacks proper support for writing addons in C#, so until it gets resolved you'll need to download 
the project itself and reuse it in the source level. 

### Typed Node API

_Alley Cat_ provides various generic extension methods for `Node` class with which you can find nodes 
in a more idiomatic manner for C#:

```c#
using AlleyCat.Common;

// Throws an excetion when not found.
this.GetNode<MyNode>("Children/MyNode");

// Returns null when not found.
this.GetNodeOrDefault<MyNode>("Children/MyNode");

// Create and add node if necessary.
this.GetOrCreateNode<MyNode>("Children/MyNode", _ => new MyNode());

// Find children by type.
this.GetChild<MyNode>();
this.GetChildren<MyNode>();

this.GetChildOrDefault<MyNode>();
this.GetOrCreateChild<MyNode>(_ => new MyNode());
```

### Autowiring API

_Alley Cat_ implements a simple dependency injection API with which you can easily reference 
other nodes or scoped services from any node.

#### Requirements

In order to make autowiring to work, you need to override both `_EnterTree()` and `_Ready()` 
methods from every node in which you want to use the feature as follows.

```c#
using AlleyCat.Autowire;

public override void _EnterTree() => this.Prewire();

public override void _Ready() => this.Postwire();
```

And also, it is required to add `AlleyCat.Autowire.AutowireContext` class at the _last place_ 
in the project's autoload list with the name `AutowireContext`.

It serves as the root context for all autowiring processes in the project, and you can read 
more about context objects below.

#### Injection Callback

Any method annotated with `[PostConstruct]` attribute will get called once every declared 
dependencies are resolved and injected to the node.

It is preferrable to use such methods instead of `_Ready()` for initialization, since the node 
might need required dependencies which will not be available before the `[PostConstruct]` phase.

```c#
[PostConstruct]
private void OnInitialize() {
    // It's safe to access your dependencies here.
}
```

#### Node Injection

You can inject other nodes as dependencies using `[Node]` attribute like shown below:

```c#
// Find a node named `AnimationPlayer` under the current node. It will throw an 
// exception when the specified node cannot be found.
[Node("AnimationPlayer")] 
private AnimationPlayer _animationPlayer;

// Node can be referenced by specifying its path.
[Node("Children/MyNode")]
public MyNode MyNode { get; set; } // Auto property is also supported.

// Node name can be omitted when the member has the same name except for the leading `_`.
// It is assumed node name starts with a capital letter, so you can use either `_camera` or 
// `Camera` as the member name for a child node named `Camera`.   
[Node] 
private Camera _camera;

// Singletons can be referenced in the same manner, and you can add `required = false` to 
// make the dependency optional (does not throw an exception when missing).
[Node("/root/MyService", required = false)]
private MyService Service { get; private set; }

// You can also reference multiple nodes by their common type, as `IEnumerable<T>`.
[Node]
private IEnumerable<Button> Buttons;

// When you specify a node path, all child nodes under the node matching the type will be selected.
[Node("ButtonPanel")]
private IEnumerable<Button> _buttons { get; private set; }
```

#### Service Injection

##### Basic Usage

You can also declare or reference any class as a dependency, using `Microsoft.Extensions.DependencyInjection` API.

`IAutowireContext` inherits from `IServiceProvider` which can be referenced by `Node.GetAutowireContext()`.
However, it is more convenient to use attributes to declare and reference services like you can with 
with node injection service instead.

To declare a singleton service, you can simply add `[Singleton]` attribute to a node class as follows: 

```c#
[Singleton(typeof(IMyService)]
public class MyService : Node, IMyService {
    // ...
}
```

`[Singleton]` accepts one or more type arguments with which you can reference the service, in conjunction 
with `[Service]` attribute:

```c#
[Service]
private IMyService _service;

// It follows similar semantics as the [Node] attribute.
[Service(required = false)]
private IMyService OptionalService { get; private set; }
```

Alternatively, you can register your services using `IServiceCollection` API directly, by making 
your class implementing `IServiceConfiguration` interface. It can be useful when you want to register 
non node type classes or transient scoped services, for example:

```c#
public class MyServiceProvider : Node, IServiceConfiguration {

    public void Register(IServiceCollection collection)
    {
        var factory = new LoggerFactory();
        var providers = this.GetChildren<ILoggerProvider>();

        foreach (var provider in providers)
        {
            factory.AddProvider(provider);
        }

        collection.AddSingleton<ILoggerFactory>(factory);
    }

    //...
}
```

##### Context Hierarchies

By default, all registered services belongs to the 'root context', which is represented by an 
`AutowireContext` instance with the same name under `/root`. It will be automatically added when 
requested, unless manually provided.

On the other hand, you can define other contexts other than the root and nest them to form a 
context hierachy. In order to create a local context, all you have to do is to annotate a node 
where you want to bind your context with `[AutowireContext]` attribute:

```c#
[AutowireContext]
public class Console : Node {
    //...
}
```

What the above example does is creating a new context for the `GameConsole` class, and add it 
to the closest context found in the node hierarchy as a child. If there is no other context 
found in the hierarchy, it will be added to the root context instead.

Having a local context in a hierarchy means that all descendant nodes of the node that the 
context is bound will use it to resolve and register dependencies.

For exaample, if you make `ParentNode` a local autowiring context and its descendant nodes 
`ChildNode` and `DescendantNode` declare themselves as singletons, they will be registered to 
`ParentNode` rather than the root node, and only be available for injection for other nodes 
under `ParentNode`.

When a child node cannot find a suitable injection candidate from the immediate local context, 
it will try again with the next parent context instead of throwing an exception. It will follow 
up the context hierarchy until it can find a dependency or reaches the root context. If it fails 
to find a dependency in the root context, it will throw an exception as it would normally do.

An important thing to remember about the context hierarchy is that dependencies registered to 
a context are shared between every nodes under its subtree regardless of their positions.

For example, if `MyNode` has a `[Singleton]` attribute on its class definition, it can be 
injected from its parent, ancestors, children, grandchildren, siblings, or any other nodes 
that share the same context including the context node itself.  

The only exception to the rule is the node to which the context is defined. If such a node 
declares itself as `[Singleton]` or add other dependencies via `IServiceConfiguration` 
interface, they will be registered to the closest parent context rather to itself, while it 
can reference other dependencies declared by its descendants.

One of the notable use case of such a feature would be when you need to extend a node's 
functionalities in a pluggable manner. For example, if there's a `Console` class that serves as 
an in-game console, and if you want to make it accept custom commands without hardcoding it, 
you might consider making `Console` as a local autowiring context while adding various command 
objects as its dependencies: 

```c#
// The game console class.
[AutowireContext]
public class Console : Node {

    [Service]
    public IEnumerable<IConsoleCommand> Commands { get; private set; }

    ...
}

public interface IConsoleCommand {

    void Execute(string[] args);
}

[Singleton]
public class HelpCommand : Node, IConsoleCommand {
    //...
}

[Singleton]
public class QuitCommand : Node, IConsoleCommand {
    //...
}
```
You can add an empty node named `Commands` under `Console` node, for example, and add 
`HelpCommand` and `QuitCommand` under it and they will be automatically injected into `Commands` 
property of `Console` once the context initializes.


### Reactive Integration

_AlleyCat_ exposes various `Node`'s callback methods as `IObservable<T>`, so you can manipulate 
them in a reactive manner like this:

```c#
using AlleyCat.Event;

// Inside a node class.
this
    .OnInput()
    .Where(e => e.IsActionPressed("ui_console"))
    .Select(_ => Visible ? "Hide" : "Show")
    .Subscribe(a => _player.Play(a))
    .AddTo(this);

this
    .OnProcess()
    .Select(delta => delta * speed)
    .Subscribe(MovePlayer)
    .AddTo(this);
```

`AddTo` adds the `IDisposable` instance returned by `Subscribe` method to the enclosing node, 
making it automatically disposed when the node is detached from the tree.

Note that due to limitations of the implementation, the actual events you received from such 
observables are not from the node itself, but a child node that is automatically added to it.

It is the reason why there's no `OnReady()` method, as it is impossible to intercept `_Ready()` 
in this manner due to the order the method is invoked between a parent and its children.

Aside from lifecycle callbacks, signals can be accessed in the same manner as well:

```c#
using AlleyCat.Event;

[Node]
private AnimationPlayer _player;

public void _Ready() {
    _player.OnAnimationStart()
        .Where(e => e.Animation == "Show")
        .Select(e => e.Animation)
        .Subscribe(name => GD.Print("Playing animation: " + name))
        .AddTo(this);
}
```

Currently, only `AnimationPlayer` is supported but as it's very easy to add support for other 
node types, you can report an issue or create a pull request for missing classes and it will be 
added to the project.

## File API

_Ally Cat_ provides `Microsoft.Extensions.FileProviders` and `System.IO.Stream` implementations 
for Godot's `File` and `Directory` API, which means you can use 
[data paths](http://docs.godotengine.org/en/latest/learning/features/misc/data_paths.html) to 
manipulate them as they were normal file paths:

```c#
var provider = new FileProvider();
var directory = provider.GetDirectoryContents("user://");

for (var file in directory) {
    GD.Print(file);
}

var file = new FileInfo("res://README.md");

using (var reader = new StreamReader(file.CreateReadStream()))
{
    string line;

    while ((line = reader.ReadLine()) != null)
    {
        GD.Print(line);
    }
}
```

## Contact

If you have any questions or suggestions about the project, please visit 
[the official forum thread](https://godotdevelopers.org/forum/discussion/18920/alley-cat)
and leave a comment there. It's also a good place to keep track of the project's progress, so I 
recommend you to check the thread from time to time.
 
You can also contact me by visiting Godot's [Discord channel](https://discord.gg/zH7NUgz) and leave 
a message to `@mysticfall`.

## LICENSE
This project is provided under the terms of _[MIT License](LICENSE)_.
