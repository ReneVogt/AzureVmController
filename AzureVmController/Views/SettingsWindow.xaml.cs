using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Com.revo.AzureVmController.Annotations;
using Com.revo.AzureVmController.ViewModels;

namespace Com.revo.AzureVmController.Views
{
	/// <summary>
	/// Interaktionslogik für SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : INotifyPropertyChanged
	{
		public EditableCredentials Credentials { get; } = new EditableCredentials();

		public event PropertyChangedEventHandler PropertyChanged;

		public SettingsWindow()
		{
			InitializeComponent();
			Credentials.Load();
			DataContext = Credentials;
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Ok_Clicked(object sender, EventArgs e)
		{
			DialogResult = true;
			Credentials.Save();
			Close();
		}
		private void Cancel_Clicked(object sender, EventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
