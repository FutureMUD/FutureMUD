using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MudSharp_Watcher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ProcessWatcher _watcher = new ProcessWatcher();
		
		public MainWindow()
		{
			InitializeComponent();
		}

		#region Overrides of FrameworkElement

		/// <inheritdoc />
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			DataContext = new ViewModel();
			using var fs = new FileStream("LauncherBootInfo.data", FileMode.OpenOrCreate);
			using var reader = new StreamReader(fs);
			((ViewModel)DataContext).DatabaseString = reader.ReadLine() ?? "server=localhost;port=3306;database=yourdbo;uid=account;password=password;Default Command Timeout=300000";
		}

		#endregion

		private void LaunchMUDButton_OnClick(object sender, RoutedEventArgs e)
		{

			_watcher.DataReceived -= WatcherOnDataReceived;
			_watcher.DataReceived += WatcherOnDataReceived;
			_watcher.MudStoppedBooting -= WatcherOnMudStoppedBooting;
			_watcher.MudStoppedBooting += WatcherOnMudStoppedBooting;
			_watcher.MudStarted -= WatcherOnMudStarted;
			_watcher.MudStarted += WatcherOnMudStarted;
			_watcher.SetDatbaseString(((ViewModel)DataContext).DatabaseString);
			_watcher.StartMud();

			((ViewModel)DataContext).LaunchButtonEnabled = false;
		}

		private void WatcherOnMudStarted(object? sender, EventArgs e)
		{
			this.Dispatcher.Invoke(() =>
			{
				((ViewModel)DataContext).GameLaunched = true;
			});
		}

		private void WatcherOnMudStoppedBooting(object? sender, EventArgs e)
		{
			this.Dispatcher.Invoke(() =>
			{
				((ViewModel)DataContext).LaunchButtonEnabled = true;
				((ViewModel)DataContext).GameLaunched = false;
			});
		}

		private void WatcherOnDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Data))
			{
				return;
			}
		}
	}
}
