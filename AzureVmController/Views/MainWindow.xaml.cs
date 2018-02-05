// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Com.revo.AzureVmController.ViewModels;


namespace Com.revo.AzureVmController.Views
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private bool userWantsToExit;
		public DateTime MouseLeft { get; set; }

		public MainWindowModel ViewModel { get; } = new MainWindowModel();

		public MainWindow()
		{
			InitializeComponent();
			DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100), IsEnabled = true};
			timer.Tick += Timer_Tick;
		}
		private void MainWindow_OnActivated(object sender, EventArgs e)
		{
			AdjustLocation(RenderSize);
			MouseLeft = DateTime.Now;
		}
		private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			AdjustLocation(e.NewSize);
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (userWantsToExit) return;
			Hide();
			e.Cancel = true;
		}
		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);
			Hide();
		}
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			MouseLeft = DateTime.MaxValue;
		}
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			MouseLeft = DateTime.Now;
		}
		private void Timer_Tick(object sender, EventArgs e)
		{
			if (DateTime.Now - MouseLeft > TimeSpan.FromMilliseconds(1500))
				Hide();
		}
		private async void Settings_Clicked(object sender, RoutedEventArgs e)
		{
			if ((new SettingsWindow()).ShowDialog() == true)
				await ViewModel.ReloadAsync();
		}
		private void Exit_Clicked(object sender, RoutedEventArgs e)
		{
			userWantsToExit = true;
			Close();
		}
		private void AdjustLocation(Size size)
		{
			var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
			Top = workingArea.Top + workingArea.Height - size.Height - 10;
			Left = workingArea.Left + workingArea.Width - size.Width - 10;
		}
	}
}
