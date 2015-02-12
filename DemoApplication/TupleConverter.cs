// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TupleConverter.cs" company="Zühlke Engineering GmbH">
//   Zühlke Engineering GmbH
// </copyright>
// <summary>
//   Converts the two information for deletion into a tuple for binding
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DemoApplication
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class TupleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Tuple<int, string>((int)values[0], (string)values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}