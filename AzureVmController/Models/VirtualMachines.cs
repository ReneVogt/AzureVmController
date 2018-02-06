// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace Com.revo.AzureVmController.Models
{
	public class VirtualMachines : IEnumerable<VirtualMachine>
	{
		private readonly Dictionary<string, VirtualMachine> vms = new Dictionary<string, VirtualMachine>();

		private IAzure azure;
		private string _clientID, _authKey, _tenantID, _subscriptionID;

		public async Task ReloadAsync(string clientID, string authKey, string tenantID, string subscriptionID, CancellationToken cancellationToken = default)
		{
			if (azure == null || _clientID != clientID || _authKey != authKey || _tenantID != tenantID || _subscriptionID != subscriptionID)
			{
				_clientID = clientID;
				_authKey = authKey;
				_tenantID = tenantID;
				_subscriptionID = subscriptionID;
				var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(_clientID, _authKey, _tenantID, AzureEnvironment.AzureGlobalCloud).WithDefaultSubscription(_subscriptionID);
				azure = Azure.Configure()
				             .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
				             .Authenticate(credentials)
				             .WithDefaultSubscription();
			}

			var machines = await azure.VirtualMachines.ListAsync(cancellationToken: cancellationToken);
			await Task.Run(() => UpdateVms(machines), cancellationToken);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void UpdateVms(IEnumerable<IVirtualMachine> machines)
		{
			var vmdict = machines.ToDictionary(vm => vm.Id);
			var toRemove = vms.Keys.Where(id => !vmdict.ContainsKey(id)).ToArray();
			foreach (var id in toRemove) vms.Remove(id);
			foreach (var kvp in vmdict)
			{
				if (vms.TryGetValue(kvp.Key, out VirtualMachine machine))
					machine.UpdateFromVm(kvp.Value);
				else
					vms[kvp.Key] = new VirtualMachine(kvp.Value);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerator<VirtualMachine> GetEnumerator() => new ReadOnlyCollection<VirtualMachine>(vms.Values.ToList()).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}