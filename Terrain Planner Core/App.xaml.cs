using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Terrain_Planner_Tool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IDbConnection DbConnection { get; private set; }
        public void Startup_Application(object sender, StartupEventArgs e) {
            var window = new MainWindow();
            window.Show();
        }
    }
}
