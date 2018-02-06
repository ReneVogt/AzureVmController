// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Com.revo.AzureVmController.Annotations;

namespace Com.revo.AzureVmController.ViewModels
{
	public class MainWindowModel : INotifyPropertyChanged
	{
		private bool isUpdating;

		public CustomCommand ReloadCommand { get; }

		public VmListItemCollection VmItems { get; } = new VmListItemCollection();
		public bool IsUpdating
		{
			get => isUpdating;
			private set
			{
				if (isUpdating == value) return;
				ReloadCommand.Executable = !value;
				isUpdating = value;
				OnPropertyChanged();
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public MainWindowModel()
		{
			ReloadCommand = new CustomCommand(async () => await ReloadAsync());
		}

		private async Task ReloadAsync()
		{
			IsUpdating = true;
			await VmItems.ReloadAsync();
			IsUpdating = false;
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
