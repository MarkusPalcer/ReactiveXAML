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

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var valueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            var source = Source;
            var path = Path;

            if (source == null)
            {
                var target = valueTarget.TargetObject as FrameworkElement;
                if (target == null)
                {
                    throw new InvalidOperationException("Either Source must be set or this MarkupExtenstion must be used on a FrameworkElement.");
                }

                source = target;

                path = "DataContext." + path;
            }

            var binding = new Binding(path) { Source = source };

            var command = new ReactiveCommand();
            command.SetBinding(ReactiveCommand.CalledObserverProperty, binding);

            return command;
        }
    }
}