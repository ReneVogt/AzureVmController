// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Com.revo.AzureVmController.Models;

namespace Com.revo.AzureVmController.ViewModels
{
	public class VmListItemCollection : ObservableCollection<VmListItem>
	{
		private readonly VirtualMachines vms = new VirtualMachines();
		private readonly PropertyChangedEventHandler itemsPropertyChanged;
		private readonly EventHandler<VmErrorEventArgs> itemErrorOccured; 

		public event EventHandler<VmStateChangedEventArgs> VmStateChanged;
		public event EventHandler<AzureErrorEventArgs> ErrorOccured;

		public VmListItemCollection()
		{
			itemsPropertyChanged = OnItemPropertyChanged;
			itemErrorOccured = OnItemErrorOccured;
		}

		public async Task ReloadAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				if (!ProtectedSettings.AreValid)
					throw new InvalidCredentialException("You did not yet specify valid credentials!");

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
			catch (Exception e)
			{
				ErrorOccured?.Invoke(this, new AzureErrorEventArgs(e));
			}
		}

		protected override void ClearItems()
		{
			foreach (var item in this) SetItemEventHandlers(item, false);
			base.ClearItems();
		}
		protected override void InsertItem(int index, VmListItem item)
		{
			if (vms.All(vm => vm.Id != item.Id)) throw new InvalidOperationException("It's not possible to add arbitrary items to this collection!");
			SetItemEventHandlers(item, true);
			base.InsertItem(index, item);
		}
		protected override void RemoveItem(int index)
		{
			SetItemEventHandlers(this[index], false);
			base.RemoveItem(index);
		}
		protected override void SetItem(int index, VmListItem item)
		{
			SetItemEventHandlers(this[index], false);
			SetItemEventHandlers(item, true);
			base.SetItem(index, item);
		}

		private void SetItemEventHandlers(VmListItem item, bool set)
		{
			if (set)
			{
				item.PropertyChanged += itemsPropertyChanged;
				item.ErrorOccured += itemErrorOccured;
			}
			else
			{
				item.PropertyChanged -= itemsPropertyChanged;
				item.ErrorOccured -= itemErrorOccured;
			}
		}
		private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(VmListItem.State) && sender is VmListItem item)
				VmStateChanged?.Invoke(this, new VmStateChangedEventArgs(item.Name, item.State)); 
		}
		private void OnItemErrorOccured(object sender, VmErrorEventArgs e)
		{
			ErrorOccured?.Invoke(sender, e);
		}
	}
}