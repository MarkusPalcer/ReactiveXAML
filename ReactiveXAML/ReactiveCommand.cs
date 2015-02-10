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

        private static bool ValidateCanExecuteProperty(object value)
        {
            return value == null || value is bool || value is Func<object, bool>;
        }

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
            var callbackValue = CanExecute as Func<object, bool>;

            if (directValue.HasValue)
            {
                return directValue.Value;
            }
            else if (callbackValue != null)
            {
                return callbackValue(parameter);
            }
            else
            {
                return true;
            }
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
    }
}