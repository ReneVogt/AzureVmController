// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Com.revo.AzureVmController.Models;

namespace Com.revo.AzureVmController.ViewModels
{
	public class VmListItemCollection : ObservableCollection<VmListItem>
	{
		private readonly VirtualMachines vms = new VirtualMachines();

		public async Task ReloadAsync(CancellationToken cancellationToken = default)
		{
			await vms.ReloadAsync(
			                      clientID: ProtectedSettings.ClientID, 
			                      authKey: ProtectedSettings.AuthKey, 
			                      tenantID: ProtectedSettings.TenantID,
								  subscriptionID: ProtectedSettings.SubscriptionID,
								  cancellationToken: cancellationToken);

			var currentVms = vms.ToList();
			var toRemove = this.Where(vm => currentVms.All(cvm => cvm.Id != vm.Id)).ToArray();
			foreach (var vm in toRemove) Remove(vm);

			foreach (var cvm in currentVms)
			{
				var vm = this.FirstOrDefault(e => e.Id == cvm.Id);
				if (vm == null) Add(new VmListItem(cvm));
				else vm.Refresh();
			}
		}
	}
}