// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListSampleViewModel.cs" company="Zühlke Engineering GmbH">
//   Zühlke Engineering GmbH
// </copyright>
// <summary>
//   A view model for the synchronized collection sample
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DemoApplication
{
    using System;
    using System.Diagnostics;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using ReactiveXAML;

    public class ListSampleViewModel
    {
        public ListSampleViewModel()
        {
            // Let's prefill our list
            Items = new SynchronizedCollection<string>(new[] { "Some item", "Some other item", "A third item" });

            // Validation of entered value goes here:
            var valueChangeSubject = new Subject<object>();
            var isValueValid = valueChangeSubject.Select(s => !string.IsNullOrWhiteSpace(s as string));
            
            // Is an item selected?
            var listSelection = new Subject<object>();
            var isItemSelected = listSelection.OfType<int>().Select(i => i >= 0);

            // List changes (the code here simulates a web service which applies and reflects the changes)
            var addValue = new Subject<object>();
            var addResult =
                addValue.OfType<string>()
                    .Select(SynchronizedCollection<string>.ChangeEvent.Add);

            // Clear Textbox after adding
            addResult.Subscribe(_ => valueChangeSubject.OnNext(string.Empty));

            var removeValue = new Subject<object>();
            var removeResult =
                removeValue.OfType<int>()
                    .Select(i => SynchronizedCollection<string>.ChangeEvent.RemoveAt(i));

            var editValue = new Subject<object>();
            var editResult =
                editValue.OfType<Tuple<int, string>>()
                    .Select(t => SynchronizedCollection<string>.ChangeEvent.Replace(t.Item1, t.Item2));
            
            var changeResults = new[] { addResult, removeResult, editResult }.Merge();

            // Here our virtual service ends

            // Progress indicator logic
            var showProgress = new[] { addValue, removeValue, editValue }.Merge().Select(x => true);
            var hideProgress = changeResults.Select(x => false);
            
            var canEdit = isValueValid.CombineLatest(isItemSelected, (x, y) => x && y);

            // Set all the properties
            ValueChangeObserver = valueChangeSubject;
            ValidValueObservable = isValueValid;
            AddValueObserver = addValue;
            RemoveValueObserver = removeValue;
            ChangeValueObserver = editValue;
            ListSelectionObserver = listSelection;
            CanDeleteObservable = isItemSelected;
            CanEditObservable = canEdit;
            ShowProgressObservable = new[] { showProgress, hideProgress }.Merge().Select(x => (object)x);

            // And finally connect our list
            changeResults.Subscribe(x => Trace.WriteLine(x));
            changeResults.Subscribe(Items);
        }

        public IObserver<object> ValueChangeObserver { get; private set; }

        public IObservable<bool> ValidValueObservable { get; private set; }

        public IObserver<object> AddValueObserver { get; private set; }

        public IObserver<object> RemoveValueObserver { get; private set; }
        
        public IObserver<object> ChangeValueObserver { get; private set; }

        public IObserver<object> ListSelectionObserver { get; private set; }

        public IObservable<bool> CanDeleteObservable { get; private set; }

        public IObservable<bool> CanEditObservable { get; private set; }

        public IObservable<object> ShowProgressObservable { get; private set; } 

        public SynchronizedCollection<string> Items { get; private set; } 
    }
}