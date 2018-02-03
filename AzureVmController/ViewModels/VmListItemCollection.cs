// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace Com.revo.AzureVmController.ViewModels
{
	public class VmListItemCollection : ObservableCollection<VmListItem>
	{
		public async Task ReloadAsync(string clientID, string authKey, string tenantID, string subscriptionID, CancellationToken cancellationToken = default(CancellationToken))
		{
			var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientID, authKey, tenantID, AzureEnvironment.AzureGlobalCloud).WithDefaultSubscription(subscriptionID);

			var azure = Azure.Configure()
							 .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
							 .Authenticate(credentials)
							 .WithDefaultSubscription();
			Clear();
			foreach(var item in (await azure.VirtualMachines.ListAsync(cancellationToken: cancellationToken)).Select(vm => new VmListItem(vm)))
				Add(item);
		}
	}
}