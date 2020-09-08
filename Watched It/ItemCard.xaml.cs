using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace Watched_It
{
    /// <summary>
    /// Interaction logic for ItemCard.xaml
    /// </summary>
    public partial class ItemCard : UserControl
    {
        public Item item;

        public ItemCard(Item _item)
        {
            InitializeComponent();
            UpdateContent(_item,false);
        }

        public void SetupImage()
        {
            // make an HTTP Get request
            string data = "";
            if (item == null || item.name == "")
                return;
            var request = (HttpWebRequest)WebRequest.Create("https://www.google.com/search?q=" + item.name + " new " + item.type.ToString() + " poster" + "&tbm=isch");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.94 Safari/537.36";
            using (var webResponse = (HttpWebResponse)request.GetResponse())
            {
                using (Stream dataStream = webResponse.GetResponseStream())
                {
                    if (dataStream == null)
                    {
                        data = "";
                        return;
                    }
                    using (var sr = new StreamReader(dataStream))
                    {
                        data = sr.ReadToEnd();
                    }
                }
            }
            if (data == "") return;


            int urlStart = data.IndexOf("[\"http", StringComparison.Ordinal);
            int imageLocation = data.IndexOf(".jpg",urlStart, StringComparison.Ordinal);


            if (imageLocation == 0||urlStart == 0)
                return;

            while (true)
            {
                int newUrl = data.IndexOf("[\"http", urlStart + 5, StringComparison.Ordinal);
                if (newUrl > imageLocation)
                    break;
                if (newUrl == 0)
                    return;
                urlStart = newUrl;
            }

            string url = data.Substring(urlStart+2, imageLocation - urlStart + 2);
            image.Dispatcher.Invoke((Action)(() =>
            {
                image.Source = new BitmapImage(new Uri(url));
            }));

        }

        public void OverlayButton_Click(object sender, RoutedEventArgs e)
        {
            var update = new Update(item,this);
            update.ShowDialog();
        }

        public void UpdateContent(Item _item, bool isUpdate = true)
        {
            if ((string)Title.Content != _item.name)
            {
                Title.Content = _item.name;
                Thread thread = new Thread(SetupImage);
                thread.Start();
            }
            item = _item;
            if (!_item.completed)
            {
                MovieCompleted.Content = "Not completed";
                SeriesCompleted.Content = "Not completed";
            }
            else
            {
                MovieCompleted.Content = "Completed";
                SeriesCompleted.Content = "Completed";
            }
            if (_item.type == Type.Movie)
            {
                MovieCompleted.Visibility = Visibility.Visible;
                SeriesCompleted.Visibility = Visibility.Hidden;
                Season.Visibility = Visibility.Hidden;
            }
            else if (_item.type == Type.Series)
            {
                MovieCompleted.Visibility = Visibility.Hidden;
                SeriesCompleted.Visibility = Visibility.Visible;
                Season.Visibility = Visibility.Visible;
                Season.Content = "Season " + _item.season + " Episode " + _item.episode;
            }
            if (_item.progress != "")
            {
                Progress.Content = _item.progress;
                Progress.Visibility = Visibility.Visible;
            }
            else
            {
                Progress.Visibility = Visibility.Hidden;
            }
            Debug.WriteLine(item.LastEdited);
            if (isUpdate)
            {
                item.LastEdited = (int)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
                MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow != null)
                {
                    mainWindow.SetupContextMenu();
                    mainWindow.RefreshSort();
                }
            }
        }

        public void IncreaseSeason(object sender, RoutedEventArgs e)
        {
            item.season++;
            item.episode = 0;
            Season.Content = "Season " + item.season + " Episode " + item.episode;
            item.LastEdited = (int)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.SaveData();
                mainWindow.SetupContextMenu();
                mainWindow.RefreshSort();
            }
        }

        public void IncreaseEpisode(object sender, RoutedEventArgs e)
        {
            item.episode++;
            Season.Content = "Season " + item.season + " Episode " + item.episode;
            item.LastEdited = (int)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.SaveData();
                mainWindow.SetupContextMenu();
                mainWindow.RefreshSort();
            }
        }

        public void ToggleCompleted(object sender, RoutedEventArgs e)
        {
            item.completed = !item.completed;
            SeriesCompleted.Content = item.completed ? "Completed" : "Not completed";
            MovieCompleted.Content = item.completed ? "Completed" : "Not completed";
            item.LastEdited = (int)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.SaveData();
                mainWindow.SetupContextMenu();
                mainWindow.RefreshSort();
            }
        }

        public void ChangeProgress(object sender, RoutedEventArgs e)
        {
            var progress = new Progress(item, this);
            progress.ShowDialog();
        }
    }
}
