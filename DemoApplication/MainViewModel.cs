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
    using System.ComponentModel;
    using System.Reactive.Subjects;
    using System.Runtime.CompilerServices;

    using ReactiveXAML.Annotations;

    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            TestSubject = new Subject<string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Subject<string> TestSubject { get; private set; }

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