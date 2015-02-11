namespace ReactiveXAML.MarkupExtensions
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    public class ReactiveCommandExtension : MarkupExtension
    {
        public ReactiveCommandExtension(string path)
        {
            Path = path;
        }

        public string Path { get; set; }

        public object Source { get; set; }

        public string CanExecutePath { get; set; }

        public Binding CanExecuteBinding { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var valueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            var target = valueTarget.TargetObject as FrameworkElement;

            var source = Source;
            var prependDataContext = false;

            if (source == null)
            {
                if (target == null)
                {
                    throw new InvalidOperationException("Either Source must be set or this MarkupExtenstion must be used on a FrameworkElement.");
                }

                source = target;
                prependDataContext = true;
            }

            var binding = new Binding((prependDataContext ? "DataContext." : string.Empty) + Path) { Source = source };

            var command = new ReactiveCommand();
            BindingOperations.SetBinding(command, ReactiveCommand.CalledObserverProperty, binding);

            if (CanExecuteBinding != null)
            {
                if (CanExecuteBinding.Source == null)
                {
                    throw new InvalidOperationException("When using a binding for CanExecute, the source must be set. ElementName is not supported.");
                }

                BindingOperations.SetBinding(command, ReactiveCommand.CanExecuteProperty, CanExecuteBinding);
            }
            else if (CanExecutePath != null)
            {
                binding = new Binding((prependDataContext ? "DataContext." : string.Empty) + CanExecutePath) { Source = source };
                BindingOperations.SetBinding(command, ReactiveCommand.CanExecuteProperty, binding);
            }

            return command;
        }
    }
}