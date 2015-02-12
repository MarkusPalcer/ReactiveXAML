// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReactiveCommand.cs" company="Zühlke Engineering GmbH">
//   Zühlke Engineering GmbH
// </copyright>
// <summary>
//   An implementation of ICommand which binds directly to an Observer
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ReactiveXAML
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    public class ReactiveCommand : DependencyObject, ICommand
    {
        public static readonly DependencyProperty CalledObserverProperty = DependencyProperty.Register("CalledObserver", typeof(object), typeof(ReactiveCommand), new PropertyMetadata(null, CalledObserverPropertyChanged));

        public static readonly DependencyProperty CanExecuteProperty = DependencyProperty.Register("CanExecute", typeof(object), typeof(ReactiveCommand), new PropertyMetadata(true, CanExecutePropertyChanged), ValidateCanExecuteProperty);

        private bool lastObservedCanExecuteValue;

        private IDisposable canExecuteSubscription;

        public event EventHandler CanExecuteChanged;

        public object CalledObserver
        {
            get
            {
                return this.GetValue(CalledObserverProperty);
            }

            set
            {
                SetValue(CalledObserverProperty, value);
            }
        }

        [DefaultValue(true)]
        public object CanExecute
        {
            get
            {
                return this.GetValue(CanExecuteProperty);
            }

            set
            {
                SetValue(CanExecuteProperty, value);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (CalledObserver == null)
            {
                return false;
            }

            var directValue = CanExecute as bool?;
            var observableValue = CanExecute as IObservable<bool>;

            if (directValue.HasValue)
            {
                return directValue.Value;
            }
            
            if (observableValue != null)
            {
                return this.lastObservedCanExecuteValue;
            }
            
            return true;
        }

        public void Execute(object parameter)
        {
            ((IObserver<object>)CalledObserver).OnNext(parameter);
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                Dispatcher.Invoke(() => handler(this, EventArgs.Empty));
            }
        }

        private static void CanExecutePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as ReactiveCommand;
            if (item == null)
            {
                return;
            }

            if (item.canExecuteSubscription != null)
            {
                item.canExecuteSubscription.Dispose();
            }

            var observableValue = e.NewValue as IObservable<bool>;
            if (observableValue != null)
            {
                item.canExecuteSubscription =
                    observableValue.Subscribe(newValue => ObservedCanExecuteValueChanged(item, newValue), ex => Console.WriteLine("CanExecute onerror: {0}", ex));
            }

            item.OnCanExecuteChanged();
        }

        private static void ObservedCanExecuteValueChanged(ReactiveCommand item, bool newValue)
        {
            item.lastObservedCanExecuteValue = newValue;
            item.OnCanExecuteChanged();
        }

        private static void CalledObserverPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as ReactiveCommand;
            if (item == null)
            {
                return;
            }

            item.OnCanExecuteChanged();
        }

        private static bool ValidateCanExecuteProperty(object value)
        {
            return value == null || value is bool || value is IObservable<bool>;
        }
    }
}