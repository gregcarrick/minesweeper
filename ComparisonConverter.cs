using System;
using System.Globalization;
using System.Windows.Data;

namespace Minesweeper
{
    /// <summary>
    /// Provides a way to bind objects with non-bool values to boolean
    /// properties of controls, such as a radio button's IsChecked.
    /// </summary>
    public class ComparisonConverter : IValueConverter
    {
        /// <summary>
        /// Returns true when <code>value?.Equals(parameter)</code> returns true,
        /// false otherwise.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="targetType">Required by <see cref="IValueConverter"/> but unused.</param>
        /// <param name="parameter">The parameter to compare against.</param>
        /// <param name="cultureInfo">Required by <see cref="IValueConverter"/> but unused.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return value?.Equals(parameter);
        }

        /// <summary>
        /// Inverts <see cref="Convert(object, Type, object, CultureInfo)"/> by
        /// returning <paramref name="parameter"/> when <paramref name="value"/>
        /// is a bool with value true, <see cref="Binding.DoNothing"/> otherwise.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType">Required by <see cref="IValueConverter"/> but unused.</param>
        /// <param name="parameter"></param>
        /// <param name="culture">Required by <see cref="IValueConverter"/> but unused.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value?.Equals(true) == true) ? parameter : Binding.DoNothing;
        }
    }
}
