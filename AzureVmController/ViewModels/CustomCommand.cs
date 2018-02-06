// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.Windows.Input;
using Com.revo.AzureVmController.Annotations;

namespace Com.revo.AzureVmController.ViewModels
{
	public class CustomCommand : ICommand
	{
		private bool canExecute;
		private readonly Action action;

		public bool Executable
		{
			get => canExecute;
			set
			{
				if (canExecute == value) return;
				canExecute = value;
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public CustomCommand([NotNull] Action action, bool canExecute = true)
		{
			this.action = action ?? throw new ArgumentNullException(nameof(action));
			this.canExecute = canExecute;
		}

		public bool CanExecute(object parameter) => canExecute;
		public void Execute(object parameter) => action();
		public event EventHandler CanExecuteChanged;
	}
	public class CustomCommand<T> : ICommand
	{
		private bool canExecute;
		private readonly Action<T> action;

		public bool Executable
		{
			get => canExecute;
			set
			{
				if (canExecute == value) return;
				canExecute = value;
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public CustomCommand([NotNull] Action<T> action, bool canExecute = true)
		{
			this.action = action ?? throw new ArgumentNullException(nameof(action));
			this.canExecute = canExecute;
		}

		public bool CanExecute(object parameter) => canExecute;
		public void Execute(object parameter) => action((T)parameter);		
		public event EventHandler CanExecuteChanged;
	}
}
