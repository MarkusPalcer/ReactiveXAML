// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Zühlke Engineering GmbH">
//   Zühlke Engineering GmbH
// </copyright>
// <summary>
//   The main view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DemoApplication
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Subjects;
    using System.Runtime.CompilerServices;

    using ReactiveXAML.Annotations;

    using Console = System.Console;

    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            TestSubject = new Subject<object>();
            TestSubject.Subscribe(x => Console.WriteLine(@"Value changed to {0}", new[] { x }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Subject<object> TestSubject { get; private set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}