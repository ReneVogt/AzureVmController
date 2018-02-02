using System;
using System.ComponentModel;
using Com.revo.AzureVmController.Models;

namespace Com.revo.AzureVmController.Views
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private bool userWantsToExit;

		public MainWindow()
		{
			InitializeComponent();
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
		private void Settings_Clicked(object sender, EventArgs e)
		{
			(new SettingsWindow()).ShowDialog();
		}
		private void Exit_Clicked(object sender, EventArgs e)
		{
			userWantsToExit = true;
			Close();
		}
	}
}
