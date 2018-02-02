using System.ComponentModel;
using System.Runtime.CompilerServices;
using Com.revo.AzureVmController.Annotations;
using Com.revo.AzureVmController.Models;

namespace Com.revo.AzureVmController.ViewModels
{
	public class EditableCredentials : INotifyPropertyChanged
	{
		private string authKey = string.Empty;
		private string clientID = string.Empty;
		private string tenantID = string.Empty;
		private string subscriptionID = string.Empty;

		public string AuthKey
		{
			get => authKey;
			set
			{
				if (authKey == value) return;
				authKey = value;
				OnPropertyChanged();
			}
		}
		public string ClientID
		{
			get => clientID;
			set
			{
				if (clientID == value) return;
				clientID = value;
				OnPropertyChanged();
			}
		}
		public string TenantID
		{
			get => tenantID;
			set
			{
				if (tenantID == value) return;
				tenantID = value;
				OnPropertyChanged();
			}
		}
		public string SubscriptionID
		{
			get => subscriptionID;
			set
			{
				if (subscriptionID == value) return;
				subscriptionID = value;
				OnPropertyChanged();
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void Load()
		{
			AuthKey = ProtectedSettings.AuthKey;
			ClientID = ProtectedSettings.ClientID;
			TenantID = ProtectedSettings.TenantID;
			SubscriptionID = ProtectedSettings.SubscriptionID;
		}
		public void Save()
		{
			ProtectedSettings.AuthKey = AuthKey;
			ProtectedSettings.ClientID = ClientID;
			ProtectedSettings.TenantID = TenantID;
			ProtectedSettings.SubscriptionID = SubscriptionID;
			ProtectedSettings.Save();
		}
	}
}
