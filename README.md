ReactiveXAML
============

This repository is an experiment in connecting Reactive Programming to WPF.
Instead of creating ViewModels which connect the WPF/Binding world with the ReactiveExtensions-World, as ReactiveUI is doing, my approach is to enable the view to directly bind to observables and observers.
The idea is that what previously was the ViewModel will only be a collection of observables and observers to bind to.
Hopefully the view logic itself will be expressed in XAML, since it is directly dependent on the view itself.

If you want to think in ViewModels, imagine a ViewModel which only exposes Commands and "Inverse Commands" (similar to the InteractionRequests that PRISM uses).
Such a command is only called when the user invokes an action and its parameter contains the entered data.
Properties bound to values no longer exist.

My aim with creating this repository is to evaluate whether this approach is feasible or not instead of to provide a library for use in projects.
If the result of the experiment is such a library, all the better.