using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Com.revo.AzureVmController.Annotations;
using Com.revo.AzureVmController.Models;

namespace Com.revo.AzureVmController.ViewModels
{
	public class MainWindowModel : INotifyPropertyChanged
	{
		private bool isUpdating;

		public VmListItemCollection VmItems { get; } = new VmListItemCollection();
		public bool IsUpdating
		{
			get => isUpdating;
			private set
			{
				if (isUpdating == value) return;
				isUpdating = value;
				OnPropertyChanged();
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public async Task ReloadAsync()
		{
			IsUpdating = true;
			await VmItems.ReloadAsync(
			                          clientID: ProtectedSettings.ClientID,
			                          authKey: ProtectedSettings.AuthKey,
			                          tenantID: ProtectedSettings.TenantID,
			                          subscriptionID: ProtectedSettings.SubscriptionID);
			IsUpdating = false;
		}
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
