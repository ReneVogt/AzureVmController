// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Com.revo.AzureVmController.Annotations;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;

namespace Com.revo.AzureVmController.ViewModels
{
	public class VmListItem : INotifyPropertyChanged
	{
		public enum VmState
		{
			Unknown,
			Deallocated,
			Deallocating,
			Stopped,
			Stopping,
			Running,
			Starting
		}
		private IVirtualMachine vm;
		private VmState state;
		public event PropertyChangedEventHandler PropertyChanged;
		public string Name => $"{vm.ResourceGroupName}\\{vm.Name}";
		public OperatingSystemTypes OsType => vm.OSType;
		public VmState State
		{
			get => state;
			private set
			{
				if (state == value) return;
				bool busy = Busy;
				state = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CanStart));
				OnPropertyChanged(nameof(CanStop));
				OnPropertyChanged(nameof(CanDeallocate));
				if (busy != Busy) OnPropertyChanged(nameof(Busy));					
			}
		}
		public bool CanStart => State == VmState.Deallocated || State == VmState.Stopped;
		public bool CanStop => State == VmState.Running;
		public bool CanDeallocate => State == VmState.Running || State == VmState.Stopped;
		public bool Busy => State == VmState.Deallocating || State == VmState.Starting || State == VmState.Stopping;

		public VmListItem([NotNull] IVirtualMachine vm)
		{
			this.vm = vm ?? throw new ArgumentNullException(nameof(vm));
			UpdateVmState();			
		}

		public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			State = VmState.Starting;
			await vm.StartAsync(cancellationToken);
			vm = await vm.RefreshAsync(cancellationToken);
			UpdateVmState();
		}
		public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			State = VmState.Stopping;
			await vm.PowerOffAsync(cancellationToken);
			vm = await vm.RefreshAsync(cancellationToken);
			UpdateVmState();
		}
		public async Task DeallocateAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			State = VmState.Deallocating;
			await vm.DeallocateAsync(cancellationToken);
			vm = await vm.RefreshAsync(cancellationToken);
			UpdateVmState();
		}
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void UpdateVmState()
		{
			if (vm.PowerState == PowerState.Deallocated) State = VmState.Deallocated;
			else if (vm.PowerState == PowerState.Deallocating) State = VmState.Deallocating;
			else if (vm.PowerState == PowerState.Stopped) State = VmState.Stopped;
			else if (vm.PowerState == PowerState.Stopping) State = VmState.Stopping;
			else if (vm.PowerState == PowerState.Running) State = VmState.Running;
			else if (vm.PowerState == PowerState.Starting) State = VmState.Starting;
			else State = VmState.Unknown;
		}
	}
}
