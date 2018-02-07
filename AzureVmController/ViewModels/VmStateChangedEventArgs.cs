// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using Com.revo.AzureVmController.Annotations;
using Com.revo.AzureVmController.Models;

namespace Com.revo.AzureVmController.ViewModels {
	public class VmStateChangedEventArgs : EventArgs
	{
		[NotNull]
		public string Name { get; }
		public VmState State { get; }
		public VmStateChangedEventArgs([NotNull] string name, VmState state)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			State = state;
		}
	}
}