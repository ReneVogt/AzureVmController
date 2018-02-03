using System;
using Com.revo.AzureVmController.ViewModels;

namespace Com.revo.AzureVmController.Views
{
	/// <summary>
	/// Interaktionslogik für SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow
	{
		private readonly EditableCredentials credentials = new EditableCredentials();

		public SettingsWindow()
		{
			InitializeComponent();
			credentials.Load();
			DataContext = credentials;
		}

		private void Ok_Clicked(object sender, EventArgs e)
		{
			DialogResult = true;
			credentials.Save();
			Close();
		}
		private void Cancel_Clicked(object sender, EventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
