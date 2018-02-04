// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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

		private readonly Dictionary<string, VmState> powerStateToVmState = new Dictionary<string, VmState>
		{
			[PowerState.Unknown.Value] = VmState.Unknown,
			[PowerState.Deallocated.Value] = VmState.Deallocated,
			[PowerState.Deallocating.Value] = VmState.Deallocating,
			[PowerState.Stopped.Value] = VmState.Stopped,
			[PowerState.Stopping.Value] = VmState.Stopping,
			[PowerState.Running.Value] = VmState.Running,
			[PowerState.Starting.Value] = VmState.Starting
		};

		private IVirtualMachine vm;
		private string name;
		private string osType;
		private VmState state;
		private bool canStart;
		private bool canStop;
		private bool canDeallocate;
		private bool busy;

		public event PropertyChangedEventHandler PropertyChanged;

		public IVirtualMachine VM
		{
			get => vm;
			set
			{
				vm = value;
				UpdateVmValues();
			}
		}
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

				CanStart = state == VmState.Deallocated || state == VmState.Stopped;
				CanStop = state == VmState.Running;
				CanDeallocate = state == VmState.Running || state == VmState.Stopped;
				Busy = state == VmState.Deallocating || state == VmState.Starting || state == VmState.Stopping;
			}
		}
		public bool CanStart
		{
			get => canStart;
			set
			{
				if (canStart == value) return;
				canStart = value;
				OnPropertyChanged();
			}
		}
		public bool CanStop
		{
			get => canStop;
			set
			{
				if (canStop == value) return;
				canStop = value;
				OnPropertyChanged();
			}
		}
		public bool CanDeallocate
		{
			get => canDeallocate;
			set
			{
				if (canDeallocate == value) return;
				canDeallocate = value;
				OnPropertyChanged();
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

		public VmListItem([NotNull] IVirtualMachine vm)
		{
			this.vm = vm ?? throw new ArgumentNullException(nameof(vm));
			UpdateVmValues();
		}

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			State = VmState.Starting;
			await vm.StartAsync(cancellationToken);
			vm = await vm.RefreshAsync(cancellationToken);
			UpdateVmState();
		}
		public async Task StopAsync(CancellationToken cancellationToken = default)
		{
			State = VmState.Stopping;
			await vm.PowerOffAsync(cancellationToken);
			vm = await vm.RefreshAsync(cancellationToken);
			UpdateVmState();
		}
		public async Task DeallocateAsync(CancellationToken cancellationToken = default)
		{
			State = VmState.Deallocating;
			await vm.DeallocateAsync(cancellationToken);
			vm = await vm.RefreshAsync(cancellationToken);
			UpdateVmState();
		}


		private void UpdateVmValues()
		{
			Name = $"{vm.ResourceGroupName}\\{vm.Name}";
			OsType = vm.OSType.ToString();
			UpdateVmState();
		}
		private void UpdateVmState()
		{
			State = powerStateToVmState.TryGetValue(vm.PowerState?.Value ?? string.Empty, out VmState s) ? s : VmState.Unknown;
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
