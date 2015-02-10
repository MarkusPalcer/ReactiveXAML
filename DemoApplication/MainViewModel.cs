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
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
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

            TransformingSubject = new Subject<object>();
            TransformationResult = TransformingSubject.OfType<string>().Select(x => string.Concat(x.Reverse()));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Subject<object> TestSubject { get; private set; }

        public Subject<object> TransformingSubject { get; private set; }

        public IObservable<object> TransformationResult { get; private set; }
        
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