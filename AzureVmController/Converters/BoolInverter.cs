using System;
using System.Globalization;
using System.Windows.Data;

namespace Com.revo.AzureVmController.Converters
{
	[ValueConversion(typeof(bool), typeof(bool))]
	public class BoolInverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(value is bool b) || !b;
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(value is bool b) || !b;
	}
}
