using System;
using System.Collections.Generic;
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
    /// Interaction logic for Update.xaml
    /// </summary>
    public partial class Update : Window
    {
        Item _item;
        ItemCard _card;
        public Update(Item item, ItemCard card)
        {
            InitializeComponent();
            _item = item;
            NameBox.Text = item.name;
            TypeBox.SelectedItem = item.type == Type.Movie ? TypeBox.Items[0] : TypeBox.Items[1];
            SeasonBox.Text = item.season.ToString();
            EpisodeBox.Text = item.episode.ToString();
            ProgressBox.Text = item.progress;
            Completed.IsChecked = item.completed;
            _card = card;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _item.progress = ProgressBox.Text;
            _item.season = Int32.Parse(SeasonBox.Text);
            _item.type = TypeBox.SelectedItem == TypeBox.Items[0] ? Type.Movie : Type.Series;
            _item.episode = Int32.Parse(EpisodeBox.Text);
            _item.name = NameBox.Text;
            _item.completed = (bool)Completed.IsChecked;

            _card.UpdateContent(_item);
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.SaveData();
            }
            this.Close();
        }
        bool clicked = false;
        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (!clicked)
            {
                DeleteButton.Content = "Are you sure?";
                clicked = true;
            }
            else
            {
                MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow != null)
                {
                    mainWindow.Delete(_card,_item);
                }
                this.Close();
            }
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
