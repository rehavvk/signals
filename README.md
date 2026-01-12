# Unity Signals
A lightweight event-aggregation framework built specifically for the Unity Engine.

Signals provides a decoupled, declarative way to broadcast and react to events across systems, 
enabling clean communication without tight dependencies. It’s designed to be simple to use, easy to refactor, 
and scalable from small gameplay interactions to large, event-driven architectures.

## Table of Contents

- [📥 Installation](#-installation)
- [📢 Signals](#-signals)
  - [Publish](#-publish)
  - [Subscribe](#-subscribe)
  - [Unsubscribe](#-unsubscribe)

## 📥 Installation

#### Install via git URL

Open the Package Manager in Unity and choose Add package from git URL, then enter:

```
https://github.com/rehavvk/signals.git
```

from the `Add package from git URL` option.

## 📢 Signals
A **signal** can be any user-defined type. For optimal garbage collection behavior, 
it’s **recommended** to use **structs** **or** records (depending on your .NET version).

Signals do **not** need to implement any interfaces or inherit from a base type.

```csharp
public struct MySignal 
{
    public string MyData;
}

public record MySignal(string MyData);
```

### Publish
To notify other parts of your application, **publish** a signal at the moment the event occurs.

When only the generic type is provided, a new instance of the signal is created using its default constructor.

```csharp
Signal.Publish<MySignal>();
```

You can also publish an explicitly constructed instance to pass data or use a specific constructor.

```csharp
Signal.Publish(new MySignal
{
    MyData = "Test"
});
```

### Subscribe
To react to a signal, **subscribe** to it and provide a callback method.

```csharp
Signal.Subscribe<MySignal>(MySignalCallback);

private void MySignalCallback()
{
    // React to the signal
}
```

If your callback defines a parameter of the signal’s type, the published instance will be passed automatically.

```csharp
Signal.Subscribe<MySignal>(MySignalCallback);

private void MySignalCallback(MySignal signal)
{
    // React using the provided signal data
}
```

### Unsubscribe
To stop receiving notifications, **unsubscribe** using the same callback that was originally registered.

```csharp
Signal.Unsubscribe<MySignal>(MySignalCallback);
```

---

***Happy sending with Unity Signals!***