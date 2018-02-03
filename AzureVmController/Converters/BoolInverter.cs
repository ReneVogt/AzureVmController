// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
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
