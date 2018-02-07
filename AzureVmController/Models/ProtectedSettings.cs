// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.Security.Cryptography;
using System.Text;
using Com.revo.AzureVmController.Properties;

namespace Com.revo.AzureVmController.Models
{

	public static class ProtectedSettings
	{
		public static string AuthKey
		{
			get => Decode(Settings.Default.Access_AuthKey);
			set => Settings.Default.Access_AuthKey = Encode(value);
		}
		public static string ClientID
		{
			get => Decode(Settings.Default.Access_ClientID);
			set => Settings.Default.Access_ClientID = Encode(value);
		}
		public static string TenantID
		{
			get => Decode(Settings.Default.Access_TenantID);
			set => Settings.Default.Access_TenantID = Encode(value);
		}
		public static string SubscriptionID
		{
			get => Decode(Settings.Default.Access_SubscriptionID);
			set => Settings.Default.Access_SubscriptionID = Encode(value);
		}
		public static bool AreValid => !(string.IsNullOrWhiteSpace(AuthKey) || string.IsNullOrWhiteSpace(ClientID) || string.IsNullOrWhiteSpace(TenantID) || string.IsNullOrWhiteSpace(SubscriptionID));

		public static void Save()
		{
			Settings.Default.Save();
		}
		private static string Encode(string s)
		{
			return Convert.ToBase64String(
			                              ProtectedData.Protect(
			                                                    Encoding.Default.GetBytes(s),
			                                                    new byte[] { 23, 42 },
			                                                    DataProtectionScope.CurrentUser));
		}
		private static string Decode(string s)
		{
			try
			{
				return Encoding.Default.GetString(ProtectedData.Unprotect(
				                                                          Convert.FromBase64String(s),
				                                                          new byte[] {23, 42}, DataProtectionScope.CurrentUser));
			}
			catch (CryptographicException)
			{
				return string.Empty;
			}
		}
	}
}