using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace Com.revo.AzureVmController.ViewModels {
	public class VmListItemCollection : ObservableCollection<VmListItem>
	{
		public async Task ReloadAsync(string clientID, string authKey, string tenantID, string subscriptionID)
		{
			var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientID, authKey, tenantID, AzureEnvironment.AzureGlobalCloud).WithDefaultSubscription(subscriptionID);

			var azure = Azure.Configure()
							 .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
							 .Authenticate(credentials)
							 .WithDefaultSubscription();
			Clear();
			foreach(var item in (await azure.VirtualMachines.ListAsync()).Select(vm => new VmListItem(vm)))
				Add(item);
		}
	}
}