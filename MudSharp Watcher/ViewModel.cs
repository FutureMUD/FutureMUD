using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MudSharp_Watcher.Annotations;

namespace MudSharp_Watcher
{
	public class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private bool _launchButtonEnabled = true;

		public bool LaunchButtonEnabled
		{
			get => _launchButtonEnabled;
			set
			{
				if (value == _launchButtonEnabled) return;
				_launchButtonEnabled = value;
				OnPropertyChanged(nameof(LaunchButtonEnabled));
			}
		}

		private bool _gameLaunched;
		public bool GameLaunched
		{
			get
			{
				return _gameLaunched;
			}
			set
			{
				_gameLaunched = value;
				OnPropertyChanged(nameof(GameLaunched));
			}
		}

		private bool _updateAvailable;

		public bool UpdateAvailable
		{
			get => _updateAvailable;
			set
			{
				if (value == _updateAvailable) return;
				_updateAvailable = value;
				OnPropertyChanged(nameof(UpdateAvailable));
			}
		}

		private string _databaseString;

		public string DatabaseString
		{
			get => _databaseString;
			set
			{
				_databaseString = value; 
				OnPropertyChanged(nameof(DatabaseString));
			}
		}
	}
}
