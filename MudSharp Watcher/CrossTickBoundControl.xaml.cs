using System;
using System.Collections.Generic;
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
	/// Interaction logic for CrossTickBoundControl.xaml
	/// </summary>
	public partial class CrossTickBoundControl : UserControl
	{
		public bool Status
		{
			get { return (bool)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}
		public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(bool), typeof(CrossTickBoundControl), new UIPropertyMetadata(false));

		public CrossTickBoundControl()
		{
			InitializeComponent();
		}
	}
}
