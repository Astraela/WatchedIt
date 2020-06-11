using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Watched_It
{
    /// <summary>
    /// Interaction logic for CreateNew.xaml
    /// </summary>
    public partial class CreateNew : Window
    {
        public CreateNew()
        {
            InitializeComponent();
            TypeBox.SelectedItem = TypeBox.Items[0];
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            button.Content = "Creating...";
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                if (NameBox.Text == "")
                    return;

                Type type = TypeBox.SelectedItem == TypeBox.Items[0] ? Type.Movie : Type.Series;

                mainWindow.CreateNewCard(type, NameBox.Text,Int32.Parse(SeasonBox.Text),Int32.Parse(EpisodeBox.Text),ProgressBox.Text,(bool)Completed.IsChecked);
            }
            this.Hide();
        }

        private void SeasonBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SeasonBox.Text =
                Regex.Replace(SeasonBox.Text, "[^0-9]", "");
        }

        private void EpisodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EpisodeBox.Text =
                Regex.Replace(EpisodeBox.Text, "[^0-9]", "");
        }
    }
}
