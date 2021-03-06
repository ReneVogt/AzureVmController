﻿// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
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
				OnPropertyChanged(nameof(AreValid));
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
				OnPropertyChanged(nameof(AreValid));
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
				OnPropertyChanged(nameof(AreValid));
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
				OnPropertyChanged(nameof(AreValid));
			}
		}
		public bool AreValid => !(string.IsNullOrWhiteSpace(authKey) || string.IsNullOrWhiteSpace(clientID) || string.IsNullOrWhiteSpace(tenantID) || string.IsNullOrWhiteSpace(subscriptionID));

		public event PropertyChangedEventHandler PropertyChanged;

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

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
