using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Com.revo.AzureVmController.Annotations;
using Com.revo.AzureVmController.Models;
using Com.revo.AzureVmController.ViewModels;

namespace Com.revo.AzureVmController.Views
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : INotifyPropertyChanged
	{
		private bool userWantsToExit;
		private bool isUpdating;

		public VmListItemCollection VmItems { get; } = new VmListItemCollection();
		public bool CanEditSettings => !(IsUpdating || VmItems.Any(item => item.Busy));
		public bool IsUpdating {
			get => isUpdating;
			private set
			{
				if (isUpdating == value) return;
				isUpdating = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CanEditSettings));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
		}
		private void MainWindow_OnActivated(object sender, EventArgs e)
		{
			var workingArea = Screen.PrimaryScreen.WorkingArea;
			Top = workingArea.Top + workingArea.Height - Height - 10;
			Left = workingArea.Left + workingArea.Width - Width - 10;
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
		private async void Refresh_Clicked(object sender, EventArgs e)
		{
			await ReloadAsync();
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
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private async Task ReloadAsync()
		{
			IsUpdating = true;
			await VmItems.ReloadAsync(
				clientID: ProtectedSettings.ClientID, 
				authKey: ProtectedSettings.AuthKey, 
				tenantID: ProtectedSettings.TenantID, 
				subscriptionID: ProtectedSettings.SubscriptionID);
			IsUpdating = false;
		}
	}
}
