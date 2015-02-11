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
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;

    using Console = System.Console;

    public class MainViewModel
    {
        public MainViewModel()
        {
            var scheduler = new TaskPoolScheduler(Task.Factory);

            TestSubject = new Subject<object>();
            TestSubject.Subscribe(x => Console.WriteLine(@"Value changed to {0}", new[] { x }));

            TransformingSubject = new Subject<object>();
            
            // Let's fake a service which takes a while to work on each item and then feeds it into its output 
            TransformationResult = TransformingSubject.OfType<string>().ObserveOn(scheduler).Select(
                x =>
                    {
                        Thread.Sleep(500);
                        return string.Concat(x.Reverse());
                    });

            ValidationPreview = new Subject<object>();
            ValidationResult = ValidationPreview.OfType<string>().Select(ValidateInput);
        }

        // A simple dummy for binding against. It simply echoes the values back to the UI.
        public Subject<object> TestSubject { get; private set; }

        // Everything that gets sent to this subject is (virtually) pushed to a service
        public Subject<object> TransformingSubject { get; private set; }

        // The services responses are received in this Observable
        public IObservable<object> TransformationResult { get; private set; }

        // This time we do not go to a service, but we do validation
        public Subject<object> ValidationPreview { get; private set; }

        // The pattern stays the same however
        public IObservable<bool> ValidationResult { get; private set; }

        // Our validation logic
        private static bool ValidateInput(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }
    }
}