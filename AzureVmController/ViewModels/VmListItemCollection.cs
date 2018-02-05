// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
// ReSharper disable InconsistentNaming

namespace Com.revo.AzureVmController.ViewModels
{
	public class VmListItemCollection : ObservableCollection<VmListItem>
	{
		//public class ReloadCommandHandler : ICommand
		//{
		//	private bool busy;
		//	private readonly Action action;

		//	public bool Busy
		//	{
		//		get => busy;
		//		set
		//		{
		//			if (busy == value) return;
		//			busy = value;
		//			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		//		}
		//	}

		//	public bool CanExecute(object parameter) => !busy;
		//	public void Execute(object parameter)
		//	{
		//		action();
		//	}			
		//	public event EventHandler CanExecuteChanged;
		//}

		private IAzure azure;
		private string _clientID, _authKey, _tenantID, _subscriptionID;

		//public ICommand ReloadCommand { get; } = new ReloadCommandHandler();

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

			var vms = (await azure.VirtualMachines.ListAsync(cancellationToken: cancellationToken)).ToDictionary(vm => vm.Id);
			var toRemove = this.Where(item => !vms.ContainsKey(item.VM.Id)).ToArray();
			foreach (var item in toRemove) Remove(item);
			foreach (var vm in vms.Values)
			{
				VmListItem existingItem = this.FirstOrDefault(item => item.VM.Id == vm.Id);
				if (existingItem == null) Add(new VmListItem(vm));
				else existingItem.VM = vm;
			}
		}
	}
}