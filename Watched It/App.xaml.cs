using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Watched_It
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // My code goes here, but nothing ever happens.
            MainWindow main = new MainWindow();
            this.MainWindow = main;
            base.OnStartup(e);
        }
    }
}
