using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using Com.revo.AzureVmController.Annotations;
using Com.revo.AzureVmController.ViewModels;

namespace Com.revo.AzureVmController.Views
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : INotifyPropertyChanged
	{
		private bool userWantsToExit;

		public ObservableCollection<VmListItem> VmItems { get; } = new ObservableCollection<VmListItem>();
		public bool CanEditSettings { get; private set; } = true;
		public bool IsUpdating { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			VmItems.Add(new VmListItem("blöder Name", VmListItem.VmState.Deallocated));
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
		private void Refresh_Clicked(object sender, EventArgs e)
		{
			IsUpdating = !IsUpdating;
			OnPropertyChanged(nameof(CanEditSettings));
			OnPropertyChanged(nameof(IsUpdating));
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
	}
}
