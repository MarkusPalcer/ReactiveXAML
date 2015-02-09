// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableBinding.cs" company="Zühlke Engineering GmbH">
//   Zühlke Engineering GmbH
// </copyright>
// <summary>
//   Used to connect a controls property to an observable/observer
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ReactiveXAML
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Subjects;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using ReactiveXAML.Annotations;

    /// <summary>
    /// Used to connect a controls property to an observable/observer
    /// </summary>
    public class ObservableBinding : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty SetterProperty = DependencyProperty.Register("Setter", typeof(IObservable<object>), typeof(ObservableBinding), new PropertyMetadata(default(IObservable<object>), SetterChanged));

        public static readonly DependencyProperty GetterProperty = DependencyProperty.Register("Getter", typeof(IObserver<object>), typeof(ObservableBinding), new PropertyMetadata(default(IObserver<object>)));

        private object value;

        private IDisposable setterSubscription;

        public ObservableBinding()
        {
            this.Setter = new Subject<object>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IObserver<object> Getter
        {
            get
            {
                return (IObserver<object>)GetValue(GetterProperty);
            }

            set
            {
                SetValue(GetterProperty, value);
            }
        }

        public IObservable<object> Setter
        {
            get
            {
                return (IObservable<object>)GetValue(SetterProperty);
            }

            set
            {
                SetValue(SetterProperty, value);
            }
        }

        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (Equals(value, this.value))
                {
                    return;
                }

                this.value = value;

                if (this.Getter != null)
                {
                    this.Getter.OnNext(value);
                }

                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static void SetterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as ObservableBinding;
            if (item == null)
            {
                return;
            }

            if (item.setterSubscription != null)
            {
                item.setterSubscription.Dispose();
                item.setterSubscription = null;
            }

            var newValue = e.NewValue as IObservable<object>;
            if (newValue != null)
            {
                newValue.Subscribe(x => item.Value = x);
            }
        }
    }
}