namespace ReactiveXAML
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Used to connect the command of a control to an observable
    /// </summary>
    public class ObservableCommand : DependencyObject, ICommand
    {
        public static readonly DependencyProperty ExecuteObserverProperty = DependencyProperty.Register("ExecuteObserver", typeof(IObserver<object>), typeof(ObservableCommand), new PropertyMetadata(default(IObserver<object>)));

        public event EventHandler CanExecuteChanged;

        public IObservable<object> CanExecuteObservable
        {
            get
            {
                return (IObservable<object>)GetValue(CanExecuteObservableProperty);
            }

            set
            {
                SetValue(CanExecuteObservableProperty, value);
            }
        }

        public IObserver<object> ExecuteObserver
        {
            get
            {
                return (IObserver<object>)GetValue(ExecuteObserverProperty);
            }

            set
            {
                SetValue(ExecuteObserverProperty, value);
            }
        }

        public bool CanExecuteDefault { get; set; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (ExecuteObserver != null)
            {
                ExecuteObserver.OnNext(parameter);
            }
        }
    }
}