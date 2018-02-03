// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System.Windows;
using System.Windows.Forms;

namespace Com.revo.AzureVmController
{
	/// <summary>
	/// Interaktionslogik für "App.xaml"
	/// </summary>
	public partial class App
	{
		private Views.MainWindow mainWindow;
		private NotifyIcon notifyIcon;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			mainWindow = new Views.MainWindow();
			notifyIcon = new NotifyIcon
			{
				Text = @"Azure VM controller",
				Icon = AzureVmController.Properties.Resources.cloud,
				Visible = true

			};
			notifyIcon.MouseMove += (_, __) =>
			{
				if (mainWindow?.IsVisible == false)
					mainWindow.Show();
			};
		}
		private void Application_Exit(object sender, ExitEventArgs e)
		{
			notifyIcon?.Dispose();
		}
	}
}
