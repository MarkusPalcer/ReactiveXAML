namespace ReactiveXAML.MarkupExtensions
{
    using System;
    using System.Windows.Data;
    using System.Windows.Markup;

    public class ObservableBindingExtension : MarkupExtension
    {
        public Binding SetterBinding { get; set; }

        public Binding GetterBinding { get; set; }

        public BindingMode? Mode { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var observableBinding = new ObservableBinding();

            if (GetterBinding == null && SetterBinding == null)
            {
                throw new InvalidOperationException("There must be at least a getter or setter present");
            }

            if (GetterBinding != null)
            {
                BindingOperations.SetBinding(observableBinding, ObservableBinding.GetterProperty, GetterBinding);
            }

            if (SetterBinding != null)
            {
                BindingOperations.SetBinding(observableBinding, ObservableBinding.SetterProperty, SetterBinding);
            }

            var bindingMode = BindingMode.TwoWay;

            if (!this.Mode.HasValue)
            {
                if (SetterBinding == null)
                {
                    bindingMode = BindingMode.OneWay;
                }
                else if (GetterBinding == null)
                {
                    bindingMode = BindingMode.OneWayToSource;
                }
            }
            else
            {
                bindingMode = this.Mode.Value;
            }

            var binding = new Binding("Value") { Mode = bindingMode, Source = observableBinding };

            return binding.ProvideValue(serviceProvider);
        }
    }
}