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
using Microsoft.Win32;
namespace Boikon
{
    /// <summary>
    /// Interaction logic for ZaagAfschrijving.xaml
    /// </summary>
    public partial class ZaagAfschrijving : UserControl
    {
        public ZaagAfschrijving()
        {
            InitializeComponent();
        }

        private void BTN_SaveSelectedItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BTN_ResetSelectedItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LV_WarehouseDashboard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void LV_WarehouseDashboard_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
        }
        private void LV_WarehouseDashboardClicked(ListView listView, GridViewColumnHeader headerClicked)
        {
        }
        private void SortListView(string sortBy, ItemCollection items)
        {
        }
        private void BTN_Send_Click(object sender, RoutedEventArgs e)
        {

        }
        //
        private void BTN_Upload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fotoSticker = new OpenFileDialog();
            fotoSticker.Multiselect = true;
            fotoSticker.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            fotoSticker.DefaultExt = ".png";
            Nullable<bool> dialogOK = fotoSticker.ShowDialog();
            if (dialogOK == true)
            {
                string sFilenames = "";
                foreach (string sFilename in fotoSticker.FileNames)
                {
                    sFilenames += ";" + sFilename;
                }
                sFilenames = sFilenames.Substring(1);
                using (StreamWriter writer = new StreamWriter("C:/Users/sande/Downloads/Boikoncode/Images.txt"))
                {
                    writer.WriteLine(sFilenames);
                }
                // Read a file  
                string readText = File.ReadAllText("C:/Users/sande/Downloads/Boikoncode/Images.txt");
                Console.WriteLine(sFilenames);
            }
        }

        private void BTN_Upload_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}



