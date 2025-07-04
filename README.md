# FakeMessageBus
A minimal message bus for Unity that is easy to use. FakeMessageBus can form the basis of an message bus or command bus.

## Installation

### Unity Package Manager
```
https://github.com/Danila-Che/FakeMessageBus.git?path=/Assets/FakeMessageBus
```

1. In Unity, open **Window** → **Package Manager**.
2. Press the **+** button, choose "**Add package from git URL...**"
3. Enter url above and press **Add**.

## Usage
Full code that uses the message bus (any type can be used, such as value types or reference types):

1. Declare a message type.

```csharp
using System;

public class ExampleMessage { ... }

public struct AthotherExampleMessage { ... }
```

2. Then simply register/unregister an object with callbacks decorated with the `ObserveMessage` attribute on the message bus.

> The callback method must have only one parameter.

```csharp
using FakeMessageBus;
using System;

public class ExampleObserver : IDisposable
{
    private MessageBus m_MessageBus;

    public ExampleObserver(MessageBus messageBus)
    {
        m_MessageBus = messageBus;

        m_MessageBus.Register(this);
    }

    public void Dispose()
    {
        m_MessageBus.Unregister(this);
    }

    [ObserveMessage]
    public void On(ExampleMessage message)
    {
        ...
    }

    [ObserveMessage]
    public void On(AthotherExampleMessage message)
    {
        ...
    }
    
    [ObserveMessage]
    public void On(int message)
    {
        ...
    }
    
    [ObserveMessage]
    public void On(string message)
    {
        ...
    }
}
```

3. At the end, simply send the message using the `Send` method.

```csharp
using FakeMessageBus;

public class ExampleMessageHandler
{
    private MessageBus m_MessageBus;

    public OnRaiseMessage()
    {
        m_MessageBus.Send(new ExampleMessage(42));
    }
}
```

## MessageBusProxy
`MessageBusProxy` encapsulates an message bus using the singleton pattern. It has static methods implementing various registration and unregistration strategies for `GameObject`.

### RegisterSingle(GameObject)
### RegisterObject(GameObject)
### RegisterRecursive(GameObject)

### UnregisterSingle(GameObject)
### UnregisterObject(GameObject)
### UnregisterRecursive(GameObject)

Other static methods:
### Clear()
### Notify<T>(T)

## GameObjectSelfRegistration
Interacts with `MessageBusProxy` to automatically register and unregister with selected strategy (Single, Object, and Recursive). Register `GameObject` with the `OnEnable` callback and uregister `GameObject` with the `OnDisable` callback.

## Dependency injection
`MessageBus` implements the `IMessageBus` interface. I use the `Reflex` framework to inject `MessageBus`.

```csharp
using Reflex.Core;
using UnityEngine;

public class ProjectInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(typeof(MessageBus), typeof(IMessageBus));
    }
}
```

```csharp
using FakeMessageBus;
using Reflex.Core;

public class ExampleObserver : MonoBehaviour
{
    [Inject] private IMessageBus m_MessageBus;

    private void OnEnable()
    {
        m_MessageBus.Register(this);
    }

    private void OnDisable()
    {
        m_MessageBus.Unregister(this);
    }
    
    ...
}
```

## MessageBus Component
### Register(object)
Register an observer with the `MessageBus` if the observer has valid callbacks. A callback contains only one parameter. The observer will not be registered callback if it contains zero or greater than one parameter.
#### Exceptions
`InvalidCallbackException`
Observer has at least one callback is invalid.

### Unregister(object)
Unregister an observer from `MessageBus`.

### Send<T>(T)
Raises an message with the specified arguments.
#### Parameter
`messageArgs` T
The message args to be sended to all registered observer callback.

### Clear()
Removes all callbacks (observers) from the `MessageBus`.

### GetActiveObserverCount<T>()
Gets the number of registered callback for the specified `MessageArgs` type. 
#### Return
int
The number of registered callback
