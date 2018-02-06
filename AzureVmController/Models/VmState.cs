// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
namespace Com.revo.AzureVmController.Models
{
	public enum VmState
	{
		Unknown,
		Deallocated,
		Deallocating,
		Stopped,
		Stopping,
		Running,
		Starting
	}
}