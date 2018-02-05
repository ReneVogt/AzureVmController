// Copyright 2018 René Vogt. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Com.revo.AzureVmController.Annotations;

namespace Com.revo.AzureVmController.ViewModels
{
	public class AsyncCommand : ICommand
	{
		private bool canExecute;
		private readonly Func<Task> action;

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

		public AsyncCommand([NotNull] Func<Task> action, bool canExecute = true)
		{
			this.action = action ?? throw new ArgumentNullException(nameof(action));
			this.canExecute = canExecute;
		}

		public bool CanExecute(object parameter) => canExecute;
		public async void Execute(object parameter)
		{
			await action();
		}
		public event EventHandler CanExecuteChanged;
	}
	public class AsyncCommand<T> : ICommand
	{
		private bool canExecute;
		private readonly Func<T, Task> action;

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

		public AsyncCommand([NotNull] Func<T, Task> action, bool canExecute = true)
		{
			this.action = action ?? throw new ArgumentNullException(nameof(action));
			this.canExecute = canExecute;
		}

		public bool CanExecute(object parameter) => canExecute;
		public async void Execute(object parameter)
		{
			await action((T)parameter);
		}
		public event EventHandler CanExecuteChanged;
	}
}
