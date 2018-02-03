// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Com.revo.AzureVmController.Annotations;
using Microsoft.Azure.Management.Compute.Fluent;

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
		private readonly IVirtualMachine vm;
		public string Name => $"{vm.ResourceGroupName}\\{vm.Name}";
		public VmState State { get; private set; }
		public bool CanStart => State == VmState.Deallocated || State == VmState.Stopped;
		public bool CanStop => State == VmState.Running;
		public bool CanDeallocate => State == VmState.Running || State == VmState.Stopped;
		public bool Busy => State == VmState.Deallocating || State == VmState.Starting || State == VmState.Stopping;

		public VmListItem([NotNull] IVirtualMachine vm)
		{
			this.vm = vm ?? throw new ArgumentNullException(nameof(vm));
			UpdateVmState();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void UpdateVmState()
		{
			VmState state = VmState.Unknown;
			if (vm.PowerState == PowerState.Deallocated) state = VmState.Deallocated;
			if (vm.PowerState == PowerState.Deallocating) state = VmState.Deallocating;
			if (vm.PowerState == PowerState.Stopped) state = VmState.Stopped;
			if (vm.PowerState == PowerState.Stopping) state = VmState.Stopping;
			if (vm.PowerState == PowerState.Running) state = VmState.Running;
			if (vm.PowerState == PowerState.Starting) state = VmState.Starting;
			if (state == State) return;
			State = state;
			OnPropertyChanged(nameof(State));
		}
	}
}
