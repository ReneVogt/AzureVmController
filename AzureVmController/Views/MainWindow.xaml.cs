// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
		public bool IsUpdating {
			get => isUpdating;
			private set
			{
				if (isUpdating == value) return;
				isUpdating = value;
				OnPropertyChanged();
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
			AdjustLocation(RenderSize);
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
		private async void Refresh_Clicked(object sender, RoutedEventArgs e)
		{
			await ReloadAsync();
		}
		private async void Settings_Clicked(object sender, RoutedEventArgs e)
		{
			if ((new SettingsWindow()).ShowDialog() == true)
				await ReloadAsync();
		}
		private void Exit_Clicked(object sender, RoutedEventArgs e)
		{
			userWantsToExit = true;
			Close();
		}
		private void Start_Clicked(object sender, RoutedEventArgs e) => ((sender as Button)?.DataContext as VmListItem)?.StartAsync();
		private void Stop_Clicked(object sender, RoutedEventArgs e) => ((sender as Button)?.DataContext as VmListItem)?.StopAsync();
		private void Deallocate_Clicked(object sender, RoutedEventArgs e) => ((sender as Button)?.DataContext as VmListItem)?.DeallocateAsync();
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private void AdjustLocation(Size size)
		{
			var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
			Top = workingArea.Top + workingArea.Height - size.Height - 10;
			Left = workingArea.Left + workingArea.Width - size.Width - 10;
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
