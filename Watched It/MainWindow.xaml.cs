using GalaSoft.MvvmLight.Command;
using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\WatchedIt\WatchedItData.txt";
        List<Item> ItemList = new List<Item>();
        List<ItemCard> CardList = new List<ItemCard>();
        public MainWindow()
        {
            InitializeComponent();
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\WatchedIt");

            tb.Icon = Watched_It.Properties.Resources.WatchedIt;
            LoadData();

            SetupContextMenu();
             if (!Debugger.IsAttached)
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                string str = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Watched It.exe";
                key.SetValue("WatchedIt", str);
            }
            ItemControl.ItemsSource = CardList;
        }


        public void LoadData()
        {
            if (!File.Exists(filepath))
            {
                Debug.WriteLine("No file found!");
                tb.ShowBalloonTip("We're up!", "Double click the tray icon to open or Right click for more", BalloonIcon.Info);
                SaveData();
                return;
            }

            ItemList = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(filepath));
            SetupCards();
        }

        public void SaveData()
        {
            string serialized = JsonConvert.SerializeObject(ItemList);

            File.WriteAllText(filepath, serialized);
        }

        public void SetupCardImages()
        {
            foreach (ItemCard card in CardList)
            {
                Thread thread = new Thread(card.SetupImage);
                thread.Start();
            }
        }

        public void SetupCards()
        {
            
            foreach (var card in ItemList)
            {
                
                ItemCard newCard = new ItemCard(card);
                //GridList.Children.Add(newCard);
                CardList.Add(newCard);
            }
            CollectionViewSource.GetDefaultView(CardList).Refresh();
            SortCardList("Last Edited", true);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var createNew = new CreateNew();
            createNew.ShowDialog();
            createNew.button.Content = "Create";
        }

        public void CreateNewCard(Type type, string Title,int Season,int Episode, string Progress, bool Completed)
        {
            var item = new Item(type, Title);
            item.season = Season;
            item.episode = Episode;
            item.progress = Progress;
            item.completed = Completed;
            item.LastEdited = (int)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
            ItemList.Add(item);

            ItemCard newCard = new ItemCard(item);
            //GridList.Children.Add(newCard);
            CardList.Add(newCard);
            CollectionViewSource.GetDefaultView(CardList).Refresh();
            Thread thread = new Thread(newCard.SetupImage);
            thread.Start();
            SaveData();
            SetupContextMenu();
        }

        public void Delete(ItemCard card, Item item)
        {
            CardList.Remove(card);
            ItemList.Remove(item);
            CollectionViewSource.GetDefaultView(CardList).Refresh();
            SaveData();
            SetupContextMenu();
        }

        private void SortCardList(string type, bool Ascending)
        {
            switch (type)
            {
                case "Name":
                    if(Ascending)
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c1.item.name.CompareTo(c2.item.name); });
                    else
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c2.item.name.CompareTo(c1.item.name); });
                    break;
                case "Last Edited":
                    if (Ascending)
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c2.item.LastEdited.CompareTo(c1.item.LastEdited); });
                    else
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c1.item.LastEdited.CompareTo(c2.item.LastEdited); });
                    break;
                case "Episodes":
                    if (Ascending)
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c1.item.episode.CompareTo(c2.item.episode); });
                    else
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c2.item.episode.CompareTo(c1.item.episode); });
                    break;
                case "Season":
                    if (Ascending)
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c1.item.season.CompareTo(c2.item.season); });
                    else
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c2.item.season.CompareTo(c1.item.season); });
                    break;
                case "Completed":
                    if (Ascending)
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c1.item.completed.CompareTo(c2.item.completed); });
                    else
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c2.item.completed.CompareTo(c1.item.completed); });
                    break;
                case "Progress":
                    if (Ascending)
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c1.item.progress.CompareTo(c2.item.progress); });
                    else
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c2.item.progress.CompareTo(c1.item.progress); });
                    break;
                case "Type":
                    if (Ascending)
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c1.item.type.CompareTo(c2.item.type); });
                    else
                        CardList.Sort(delegate (ItemCard c1, ItemCard c2) { return c2.item.type.CompareTo(c1.item.type); });
                    break;
            }
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortCardList(((ComboBoxItem)SortBox.SelectedItem).Content.ToString(), OrderButton.Content.ToString() == "⬆");
            CollectionViewSource.GetDefaultView(CardList).Refresh();
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            if(OrderButton.Content.ToString() == "⬆")
                OrderButton.Content = "⬇";
            else
                OrderButton.Content = "⬆";
            SortCardList(((ComboBoxItem)SortBox.SelectedItem).Content.ToString(), OrderButton.Content.ToString() == "⬆");
            CollectionViewSource.GetDefaultView(CardList).Refresh();
        }
    
        private void OpenWindow(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void ExitWindow(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void SetupContextMenu()
        {
            List<ItemCard> cardlist = new List<ItemCard>();
            foreach (var card in CardList)
            {
                cardlist.Add(card);
            }
            cardlist.Sort(delegate (ItemCard c1, ItemCard c2) { return c2.item.LastEdited.CompareTo(c1.item.LastEdited); });
            ContextMenu cm = new ContextMenu();
            MenuItem mi1 = new MenuItem();
            mi1.Header = "Open Window";
            mi1.Click += OpenWindow;
            cm.Items.Add(mi1);
            Separator s = new Separator();
            cm.Items.Add(s);
            for (int i = 0; i < Math.Min(3,cardlist.Count); i++)
            {
                ItemCard card = cardlist[i];
                MenuItem mi = new MenuItem();
                mi.Header = card.item.name;
                MenuItem season = new MenuItem();
                season.Header = "Increase Season by 1";
                season.Click += card.IncreaseSeason;
                MenuItem Episode = new MenuItem();
                Episode.Header = "Increase Episode by 1";
                Episode.Click += card.IncreaseEpisode;
                MenuItem Progress = new MenuItem();
                Progress.Header = "Change Progress";
                Progress.Click += card.ChangeProgress;
                MenuItem Completed = new MenuItem();
                Completed.Header = "Toggle Completed";
                Completed.Click += card.ToggleCompleted;
                MenuItem Update = new MenuItem();
                Update.Header = "Edit Item";
                Update.Click += card.OverlayButton_Click;

                s = new Separator();
                mi.Items.Add(season);
                mi.Items.Add(Episode);
                mi.Items.Add(Progress);
                mi.Items.Add(Completed);
                mi.Items.Add(s);
                mi.Items.Add(Update);
                cm.Items.Add(mi);
            }
            s = new Separator();
            cm.Items.Add(s);
            MenuItem Exit = new MenuItem();
            Exit.Header = "Exit";
            Exit.Click += ExitWindow;
            cm.Items.Add(Exit);
            tb.ContextMenu = cm;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void doubleclicked()
        {
            this.Show();
        }

        public void RefreshSort()
        {
            SortCardList(((ComboBoxItem)SortBox.SelectedItem).Content.ToString(), OrderButton.Content.ToString() == "⬆");
        }
    }
    public class doubleclicc : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.doubleclicked();
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
