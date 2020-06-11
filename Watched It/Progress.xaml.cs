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
using System.Windows.Shapes;

namespace Watched_It
{
    /// <summary>
    /// Interaction logic for Progress.xaml
    /// </summary>
    public partial class Progress : Window
    {
        Item _item;
        ItemCard _card;
        public Progress(Item item, ItemCard card)
        {
            InitializeComponent();
            ProgressBox.Text = item.progress;
            _item = item;
            _card = card;
            ProgressBox.Focus();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _item.progress = ProgressBox.Text;
            _item.LastEdited = (int)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
            _card.UpdateContent(_item);

            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.SaveData();
                mainWindow.SetupContextMenu();
                mainWindow.RefreshSort();
            }
            this.Close();
        }
    }
}
