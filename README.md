[![AllyCat logo](logo.png)](https://github.com/mysticfall/AlleyCat)

## Introduction
_Alley Cat_ is a programmer friendly game framework for 
[Godot](https://github.com/godotengine/godot) engine.

## Status
Don't even think about using it for anything serious yet!

## Usage

### Typed Node API

```c#
using AlleyCat.Common;

this.GetNode<MyNode>("Children/MyNode");
this.GetNodeOrDefault<MyNode>("Children/MyNode"); // Returns null when not found.
this.GetOrCreateNode<MyNode>("Children/MyNode", _ => new MyNode());

this.GetChild<MyNode>();
this.GetChildOrDefault<MyNode>(); // Returns null when not found.
this.GetOrCreateChild<MyNode>(_ => new MyNode());
this.GetChildren<MyNode>();
```

### Auto Wiring API

#### Node Injection

```c#
[Node("AnimationPlayer")]
private AnimationPlayer _animationPlayer;

[Node("Children/MyNode", required = false)] // Optional node.
public MyNode MyNode { get; set; }

[Node] // Resolve with member name, when path is omitted.
public MyNode MyNode;
```

#### Service Injection

_AlleyCat_ provides simple dependency injection service using 
`Microsoft.Extensions.DependencyInjection` API.

```c#
[Service]
private ILogger _logger;

[Service(required = false)] // Optional service.
public IMyService MyNode { get; set; }
```

### Reactive Integration

_AlleyCat_ exposes various `Node`'s callback methods as `IObservable<T>`. 

```c#
using AlleyCat.Event;

// Inside a node class.
this
    .OnInput()
    .Where(e => e.IsActionPressed("ui_console"))
    .Select(_ => Visible ? "Hide" : "Show")
    .Subscribe(a => _player.Play(a))
    .AddTo(this);
```

## LICENSE
This project is provided under the terms of _[MIT License](LICENSE)_.
