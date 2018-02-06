// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Com.revo.AzureVmController.Annotations;
using Com.revo.AzureVmController.Models;

namespace Com.revo.AzureVmController.ViewModels
{
	public class VmListItem : INotifyPropertyChanged
	{
		private readonly VirtualMachine vm;
		private string name;
		private string osType;
		private VmState state;
		private bool busy;

		public event PropertyChangedEventHandler PropertyChanged;

		public string Id { get; private set; }
		public string Name
		{
			get => name;
			set
			{
				if (name == value) return;
				name = value;
				OnPropertyChanged();
			}
		}
		public string OsType
		{
			get => osType;
			set
			{
				if (osType == value) return;
				osType = value;
				OnPropertyChanged();
			}
		}
		public VmState State
		{
			get => state;
			private set
			{
				if (state == value) return;
				state = value;
				OnPropertyChanged();

				StartCommand.Executable = state == VmState.Deallocated || state == VmState.Stopped;
				StopCommand.Executable = state == VmState.Running;
				DeallocateCommand.Executable = state == VmState.Running || state == VmState.Stopped;
				Busy = state == VmState.Deallocating || state == VmState.Starting || state == VmState.Stopping;
			}
		}
		public bool Busy
		{
			get => busy;
			set
			{
				if (busy == value) return;
				busy = value;
				OnPropertyChanged();
			}
		}

		public CustomCommand<VmListItem> StartCommand { get; } = new CustomCommand<VmListItem>(async item => await item.StartAsync());
		public CustomCommand<VmListItem> StopCommand { get; } = new CustomCommand<VmListItem>(async item => await item.StopAsync());
		public CustomCommand<VmListItem> DeallocateCommand { get; } = new CustomCommand<VmListItem>(async item => await item.DeallocateAsync());

		public VmListItem([NotNull] VirtualMachine vm)
		{
			this.vm = vm;
			Refresh();
		}

		public void Refresh()
		{
			Id = vm.Id;
			Name = vm.Name;
			OsType = vm.OS;
			State = vm.State;
		}

		private async Task StartAsync(CancellationToken cancellationToken = default)
		{			
			State = VmState.Starting;
			await vm.StartAsync(cancellationToken);
			Refresh();
		}
		private async Task StopAsync(CancellationToken cancellationToken = default)
		{
			State = VmState.Stopping;
			await vm.StopAsync(cancellationToken);
			Refresh();
		}
		private async Task DeallocateAsync(CancellationToken cancellationToken = default)
		{
			State = VmState.Deallocating;
			await vm.DeallocateAsync(cancellationToken);
			Refresh();
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
