using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Com.revo.AzureVmController.Annotations;

namespace Com.revo.AzureVmController.ViewModels
{
	public class VmListItem : INotifyPropertyChanged
	{		
		public enum VmState
		{
			Running,
			Starting,
			Stopped,
			Stopping,
			Deallocated,
			Deallocating
		}
		public string Name { get; set; }
		public VmState State { get; set; }
		public bool CanStart => State == VmState.Deallocated || State == VmState.Stopped;
		public bool CanStop => State == VmState.Running;
		public bool CanDeallocate => State == VmState.Running || State == VmState.Stopped;
		public bool Busy => State == VmState.Deallocating || State == VmState.Starting || State == VmState.Stopping;

		public VmListItem() { }
		public VmListItem(string name, VmState state)
		{
			Name = name;
			State = state;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
