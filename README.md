ReactiveXAML
============

This repository is an experiment in connecting Reactive Programming (specifically ReactiveExtensions) to WPF.
The aim of ReactiveXAML is not to replace [ReactiveUI] but to try a different approach of linking the reactive world with the WPF world.
While [ReactiveUI] uses helpers to create ViewModels which connect the reactive with the WPF world by converting them into Properties and implementing `INotifyPropertyChanged`, ReactiveXAML aims to enable "direct" binding to `IObservable`s and `IObserver`s.
ReactiveXAML provides helper classes and markup extensions which enable these kind of bindings.

This repository is not meant to provide a fully functional framework, but to provide a space for experiments with the idea of moving the bridge between reactive and WPF from the ViewModel to the binding itself.
Results from these experiments will be briefly documented here.

Usage
-----

* Use ObservableBindingExtension to bind to Observers and Observables
* One ObservableBinding can have an Observer and an Observable  
  If only one property is given, the type is used to determine binding direction
* BindingMode between proxy and target property can be set
* UpdateSourceTrigger supported
* Use ReactiveCommandExtension to bind a command to an Observer
* CommandParameter is passed as value
* Use SynchronizedCollection to create a collection which is updated via Update-Events
* SynchronizedCollection contains ChangeEvent with factory methods


Restrictions / Issues
---------------------

* Currently `ObservableBinding`s are only possible to `IObservable<object>` and `IObserver<object>`.  
  Typing is only available for the CanExecute-Binding of the commands, which needs to bind to `IObservable<bool>`.
* The `ObservableBinding` are implemented using a proxy-object, which is instantiated for each binding.
* No ValueConverter support yet
* SynchronizedCollection sends an event to the UI for each changed item (especially for ReplaceAll changes)  
  Bulk-Change behavior not implemented yet
* SynchronizedCollection does not yet use [RX] schedulers but captures the dispatcher it was created on


Comparison to WPF
-----------------

* Rather similar (an event controls updates of the property)
* WPF uses one event for all properties (INotifyPropertyChanged) <-> ReactiveXAML uses one event for each property
* ReactiveXAML uses schedulers <-> WPF uses Tasks and Dispatcher

Comparison to ObservableViews
-----------------------------

* No virtual dom as in [ReactJS], but WPF has virtualization  
  -> Is this comparable?
* No exchangeable renderer as in [cycle]. WPF has a builtin renderer.


Conclusion
----------

* General idea seems okay
* Proxy object for binding might cause performance/memory issues
* View logic can be expressed in LINQ with the help of [RX]
* "ViewModel" can use schedulers provided by [RX], ensuring thread safety


[ReactiveUI]: http://reactiveui.net/
[RX]: https://msdn.microsoft.com/en-us/data/gg577609
[cycle]: https://github.com/staltz/cycle
[ReactJS]: http://facebook.github.io/react/