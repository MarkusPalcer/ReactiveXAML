namespace ReactiveXAML.MarkupExtensions
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    public class ObservableBindingExtension : MarkupExtension
    {
        public ObservableBindingExtension()
        {
            this.Mode = BindingMode.Default;
        }

        public ObservableBindingExtension(string path) : this()
        {
            this.SetterBinding = new Binding(path);
            this.GetterBinding = new Binding(path);
        }

        public Binding SetterBinding { get; set; }

        public Binding GetterBinding { get; set; }

        public BindingMode Mode { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var valueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            var target = valueTarget.TargetObject as FrameworkElement;

            var proxy = new ObservableBindingProxy();

            if (this.SetterBinding == null && this.GetterBinding == null)
            {
                throw new InvalidOperationException("There must be at least one setter or getter binding provided");
            }

            if (this.SetterBinding != null)
            {
                if (this.SetterBinding.Source == null)
                {
                    this.SetterBinding.Source = target;
                    this.SetterBinding.Path.Path = "DataContext." + this.SetterBinding.Path.Path;
                }

                BindingOperations.SetBinding(proxy, ObservableBindingProxy.SetterProperty, this.SetterBinding);
            }

            if (this.GetterBinding != null)
            {
                if (this.GetterBinding.Source == null)
                {
                    this.GetterBinding.Source = target;
                    this.GetterBinding.Path.Path = "DataContext." + this.GetterBinding.Path.Path;
                }

                BindingOperations.SetBinding(proxy, ObservableBindingProxy.GetterProperty, this.GetterBinding);
            }

            var result = new Binding("Value")
                             {
                                 Source = proxy,
                                 Mode = Mode
                             };

            return result.ProvideValue(serviceProvider);
        }
    }
}