// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using Com.revo.AzureVmController.Annotations;

namespace Com.revo.AzureVmController.ViewModels {
	public class VmErrorEventArgs : AzureErrorEventArgs
	{
		public string VmName { get; }

		public VmErrorEventArgs(string vmName, [NotNull] Exception error)
			: base(error)
		{
			VmName = vmName;
		}
	}
}