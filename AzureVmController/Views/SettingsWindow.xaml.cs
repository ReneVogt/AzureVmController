// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using Com.revo.AzureVmController.ViewModels;

namespace Com.revo.AzureVmController.Views
{
	/// <summary>
	/// Interaktionslogik für SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow
	{
		public EditableCredentials Credentials { get; } = new EditableCredentials();

		public SettingsWindow()
		{
			InitializeComponent();
			Credentials.Load();
		}

		private void Ok_Clicked(object sender, EventArgs e)
		{
			DialogResult = true;
			Credentials.Save();
			Close();
		}
		private void Cancel_Clicked(object sender, EventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
