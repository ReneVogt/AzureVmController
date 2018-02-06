// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Com.revo.AzureVmController.Annotations;
using Microsoft.Azure.Management.Compute.Fluent;

namespace Com.revo.AzureVmController.Models
{
	public class VirtualMachine
	{
		private IVirtualMachine vm;
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

		public string Id { get; }
		public VmState State { get; private set; }
		public string Name { get; private set; }
		public string OS { get; private set; }		

		public VirtualMachine([NotNull] IVirtualMachine virtualMachine)
		{
			vm = virtualMachine ?? throw new ArgumentNullException(nameof(virtualMachine));
			Id = vm.Id;
			UpdateFromVm(vm);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateFromVm([NotNull] IVirtualMachine virtualMachine)
		{
			vm = virtualMachine ?? throw new ArgumentNullException(nameof(virtualMachine));

			Name = $"{vm.ResourceGroupName}\\{vm.Name}";
			OS = vm.OSType.ToString();
			State = powerStateToVmState.TryGetValue(vm.PowerState?.Value ?? string.Empty, out VmState s) ? s : VmState.Unknown;
		}
		public Task UpdateFromVmAsync([NotNull] IVirtualMachine virtualMachine)
		{
			if (virtualMachine == null) throw new ArgumentNullException(nameof(virtualMachine));
			return UpdateFromVmAsyncInternal();
			async Task UpdateFromVmAsyncInternal() => await Task.Run(() => UpdateFromVm(virtualMachine));
		}
		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			State = VmState.Starting;
			await vm.StartAsync(cancellationToken);
			await UpdateFromVmAsync(await vm.RefreshAsync(cancellationToken));			
		}
		public async Task StopAsync(CancellationToken cancellationToken = default)
		{
			State = VmState.Stopping;
			await vm.PowerOffAsync(cancellationToken);
			await UpdateFromVmAsync(await vm.RefreshAsync(cancellationToken));
		}
		public async Task DeallocateAsync(CancellationToken cancellationToken = default)
		{
			State = VmState.Deallocating;
			await vm.DeallocateAsync(cancellationToken);
			await UpdateFromVmAsync(await vm.RefreshAsync(cancellationToken));
		}

	}
}
