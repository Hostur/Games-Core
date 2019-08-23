# Games core

This project take advantage of one of the most popular DI frameworks used in business - Autofac.
With some additional logic dedicated for Unity you can easy register and inject your dependencies.

## Registration dependencies 

For desktop - registration look like this:
```
[CoreRegister(true)]
public sealed class CoreMainThreadActionsQueue
{
  private readonly Queue<Action> _threadedActions;
  private readonly Queue<IEnumerator> _coroutines;

  public CoreMainThreadActionsQueue()
  {
    _threadedActions = new Queue<Action>(48);
    _coroutines = new Queue<IEnumerator>(24);
  }
...
```
Boolean parameter for registration its a value indicating whether given type should be threated like singletone or not.
You can extend this functionality - for example by registration dependency for particular DI sub-scope.

Desktop version care about registration as implemented interfaces, disposable and keyed registration that helps later with injecting types into GameObjects.

This automated resolve is implemented <a href="https://github.com/Hostur/Games-Core/blob/master/Assets/_Scripts/Core/DI/RegisterAssemblyModule.cs" target="_blank">`here`</a>

As you can see, mobile registration is not fully automated. Thats because of dynamic compilation used by Autofac.

## Injecting

You can resolve your dependencies in couple diffrent ways:
First of all by placing your dependencies in constructors.

```
[CoreRegister(false)]
public class ExampleClass
{
  private readonly CoreMainThreadActionsQueue _coreMainThreadActionsQueue;
  public ExampleClass(CoreMainThreadActionsQueue coreMainThreadActionsQueue)
  {
    _coreMainThreadActionsQueue = coreMainThreadActionsQueue;
  }
}
```

On classed derived from CoreBehaviour you can inject dependencies through inject attribute:

```
[CoreRegister(false)]
public class ExampleClass : CoreBehaviour
{
  [CoreInject]
  private CoreMainThreadActionsQueue _coreMainThreadActionsQueue;
}
```

Injection happening inside <a href="https://github.com/Hostur/Games-Core/blob/master/Assets/_Scripts/Core/CoreBehaviour.cs" target="_blank">`CoreBehaviour`</a> Awake function
by calling static extension from <a href="https://github.com/Hostur/Games-Core/blob/master/Assets/_Scripts/Core/DI/RunetimeDependencyProvider.cs" target="_blank">`RunetimeDependencyProvider`</a>.

You can call it in any other place to resolve all the fields marked by [CoreInject] attribute.

The last opction for injection is by touching DI container directly. It is not appropriate way but sometimes you need lazy dependency because the default one may 
cause circular dependency problem when class A need dependency to class B and class B require dependency to class C which depends on cass A.
You can resolve such scenario by hand made registration the Lazy<T> of your class in container, and injecting this lazy dependency that will be resolved dynamicly when
you touch this class. If you are scary of such architecture you can resolve dependency in runetime from container:

```
private void SomeFunction()
{
  var dependency = God.PrayFor<CustomDependency>();
  dependency.DoSomething();
  ...
}
```
 
## Internal communication - events

Each other functionality is built at the top of this DI.
You can use internal communication provided by <a href="https://github.com/Hostur/Games-Core/blob/master/Assets/_Scripts/Core/InternalCommunication/CoreGameEventsManager.cs" target="_blank">`CoreGameEventsManager`</a>.
You can subscribe for certain types of events that you will create by yourself:

```
public class SetGameLanguageEvent : CoreGameEvent
{
  public GameLanguage Language { get; }
  public SetGameLanguageEvent(GameLanguage language)
  {
	Language = language;
  } 
}
```

Subscription can be automated.
In CoreBehaviour by EventHandler registration:
```
CoreRegisterEventHandler(typeof(SetGameLanguageEvent))]
private void OnSetGameLanguageEvent(object sender, EventArgs arg)
{
  SetGameLanguageEvent e = arg as SetGameLanguageEvent;
  PlayerPrefs.SetInt(KEY_GAME_LANGUAGE, (int)e.Language);
  PlayerPrefs.Save();
  Publish(new AfterLanguageChangedEvent(e.Language));
}
```

CoreBehaviour also take advantage of unsubscribing these handlers when your game object will be destroyed.

Like in case of DI you can use backend methods directly by calling:
```
this.SubscribeMyEventHandlers();
```
and 
```
this.UnSubscribeMyEventHandlers();
```
It is automated in CoreBehaviours.

You can subscribe your handler directly through CoreGameEventsManager:
```
CoreGameEventsManager.Subscribe<LanguagesManager.AfterLanguageChangedEvent>(OnReloadLanguageEvent);
```

## Resources

WIP

## MVVM

WIP

## Workbooks and Scriptables

WIP
 
