// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.Windows;
using System.Windows.Forms;
using Com.revo.AzureVmController.Models;
using Com.revo.AzureVmController.Properties;
using Com.revo.AzureVmController.ViewModels;
using MessageBox = System.Windows.MessageBox;

namespace Com.revo.AzureVmController
{
	/// <summary>
	/// Interaktionslogik für "App.xaml"
	/// </summary>
	public partial class App
	{
		private Views.MainWindow mainWindow;
		private NotifyIcon notifyIcon;
		private AzureErrorEventArgs lastError;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			if (!Settings.Default.Upgraded)
			{
				Settings.Default.Upgrade();
				Settings.Default.Upgraded = true;
				Settings.Default.Save();
			}

			mainWindow = new Views.MainWindow();
			mainWindow.VmStateChanged += MainWindow_VmStateChanged;
			mainWindow.ErrorOccured += MainWindow_ErrorOccured;
			notifyIcon = new NotifyIcon
			{
				Text = @"Azure VM controller",
				Icon = AzureVmController.Properties.Resources.cloud,
				Visible = true
			};
			notifyIcon.MouseMove += (_, __) =>
			{
				mainWindow.MouseLeft = DateTime.Now;
				if (mainWindow?.IsVisible != false) return;
				mainWindow.Show();
			};			
			notifyIcon.BalloonTipClosed += (_, __) => lastError = null;
			notifyIcon.BalloonTipClicked += (_, __) =>
			{
				switch (lastError)
				{
					case VmErrorEventArgs vmerror:
						MessageBox.Show($"An error occured at {vmerror.VmName}:{Environment.NewLine}{vmerror.Error.ToString()}", "Azure VM Controller", MessageBoxButton.OK, MessageBoxImage.Error);
						break;
					case AzureErrorEventArgs azureError:
						MessageBox.Show($"An error occured: {Environment.NewLine}{azureError.Error.ToString()}", "Azure VM Controller", MessageBoxButton.OK, MessageBoxImage.Error);
						break;
				}

				lastError = null;
			};
		}
		private void MainWindow_VmStateChanged(object sender, VmStateChangedEventArgs e)
		{			
			if (notifyIcon == null || (e.State != VmState.Deallocated && e.State != VmState.Running && e.State != VmState.Stopped)) return;
			notifyIcon.ShowBalloonTip(10, $"{e.Name} - {e.State}", $"VM {e.Name} changed its state to {e.State}.", ToolTipIcon.Info);
		}
		private void MainWindow_ErrorOccured(object sender, AzureErrorEventArgs e)
		{
			if (notifyIcon == null) return;

			lastError = e;
			switch (e)
			{
				case VmErrorEventArgs vmerror:
					notifyIcon.ShowBalloonTip(int.MaxValue, $"Error at {vmerror.VmName}", vmerror.Error.Message, ToolTipIcon.Error);
					break;
				case AzureErrorEventArgs vmerror:
					notifyIcon.ShowBalloonTip(int.MaxValue, "Error", vmerror.Error.Message, ToolTipIcon.Error);
					break;
			}
		}
		private void Application_Exit(object sender, ExitEventArgs e)
		{
			notifyIcon?.Dispose();
		}
	}
}
