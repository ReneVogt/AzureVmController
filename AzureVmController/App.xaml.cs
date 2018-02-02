﻿using System.Windows;
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
			var workingArea = Screen.PrimaryScreen.WorkingArea;
			mainWindow.Top = workingArea.Top + workingArea.Height - mainWindow.Height - 10;
			mainWindow.Left = workingArea.Left + workingArea.Width - mainWindow.Width - 10;
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
