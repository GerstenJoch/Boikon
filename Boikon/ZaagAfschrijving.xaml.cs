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
    public sealed class ButtonChrome : System.Windows.Controls.Decorator { }
    /// <summary>
    /// Interaction logic for ZaagAfschrijving.xaml
    /// </summary>
    /// 

    //CSV -> Boikon -> Tigerstop -> Zaag
    // -> Boikon voor afschrijving
    public partial class ZaagAfschrijving : UserControl
    {
        OpenFileDialog fotoSticker = new OpenFileDialog();
        public ZaagAfschrijving()
        {
            InitializeComponent();
            Init_Artikelen_En_Bewerkers();
        }

        private void BTN_Send_Click(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(BTN_Send))         //Zaag
            {
                int i;
                if (String.IsNullOrEmpty(tbZaagAantal.Text) || !int.TryParse(tbZaagAantal.Text, out i))
                {
                    lblZaagAantal.Foreground = Brushes.Red;
                    lblZaagError.Visibility = Visibility.Visible;
                }
                else
                {
                    lblZaagAantal.Foreground = Brushes.Black;
                }
                if (CB_Artikel.SelectedIndex == -1)
                {
                    lblZaagArtikel.Foreground = Brushes.Red;
                    lblZaagError.Visibility = Visibility.Visible;
                }
                else
                {
                    lblZaagArtikel.Foreground = Brushes.Black;
                }
                if (CB_Bewerker.SelectedIndex == -1)
                {
                    lblGegevensBewerker.Foreground = Brushes.Red;
                    lblZaagError.Visibility = Visibility.Visible;
                }
                else
                {
                    lblGegevensBewerker.Foreground = Brushes.Black;
                }
                if (!(String.IsNullOrEmpty(tbZaagAantal.Text) || !int.TryParse(tbZaagAantal.Text, out i) || CB_Artikel.SelectedIndex == -1 || CB_Bewerker.SelectedIndex == -1))
                {
                    lblZaagError.Visibility = Visibility.Collapsed;
                }
            }

            if (sender.Equals(BTN_Push))
            {        //Afschrijving
                int i;
                if (String.IsNullOrEmpty(tbAfschrijvingActueelGebruik.Text) || !int.TryParse(tbAfschrijvingActueelGebruik.Text, out i))
                {
                    lblAfschrijvingActueelGebruik.Foreground = Brushes.Red;
                    lblAfschrijvingError.Visibility = Visibility.Visible;
                }
                else
                {
                    lblAfschrijvingActueelGebruik.Foreground = Brushes.Black;
                }
                if (String.IsNullOrEmpty(tbAfschrijvingReden.Text) && tbAfschrijvingVerwachtGebruik.Text.Trim() != tbAfschrijvingActueelGebruik.Text.Trim())
                {
                    lblAfschrijvingReden.Foreground = Brushes.Red;
                    lblAfschrijvingError.Visibility = Visibility.Visible;
                }
                else
                {
                    lblAfschrijvingReden.Foreground = Brushes.Black;
                }
                if (!(fotoSticker.FileNames.Length > 0))
                {
                    lblAfschrijvingUploadSticker.Foreground = Brushes.Red;
                    lblAfschrijvingError.Visibility = Visibility.Visible;
                }
                if (!(String.IsNullOrEmpty(tbAfschrijvingActueelGebruik.Text) || !int.TryParse(tbAfschrijvingActueelGebruik.Text, out i) || (String.IsNullOrEmpty(tbAfschrijvingReden.Text) && tbAfschrijvingVerwachtGebruik.Text.Trim() != tbAfschrijvingActueelGebruik.Text.Trim()) || !(fotoSticker.FileNames.Length > 0)))
                {
                    lblAfschrijvingError.Visibility = Visibility.Collapsed;
                    Write_Log();
                }
            }
        }
        private void BTN_Upload_Click(object sender, RoutedEventArgs e)
        {


            fotoSticker.Multiselect = true;
            fotoSticker.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            fotoSticker.DefaultExt = ".png";
            Nullable<bool> dialogOK = fotoSticker.ShowDialog();
            if (dialogOK == true)
            {
                if (fotoSticker.FileNames.Length > 1)
                {
                    lblAfschrijvingStickerConfirmation.Content = "U mag niet meer dan 1 foto uploaden";
                    lblAfschrijvingStickerConfirmation.Visibility = Visibility.Visible;
                    lblAfschrijvingStickerConfirmation.Foreground = Brushes.Red;
                }
                else
                {
                    string sFileName = fotoSticker.FileName;
                    string[] UploadName = sFileName.Split('\\');
                    lblAfschrijvingStickerConfirmation.Content = UploadName.Last();
                    lblAfschrijvingStickerConfirmation.Visibility = Visibility.Visible;
                    lblAfschrijvingUploadSticker.Foreground = Brushes.Black;
                    lblAfschrijvingStickerConfirmation.Foreground = Brushes.Black;
                    using (StreamWriter writer = new StreamWriter("C:/Users/sande/Downloads/Boikoncode/Images.txt"))
                    {
                        writer.WriteLine(sFileName);
                    }
                }
            }
        }
        private void Init_Artikelen_En_Bewerkers()
        {
            CB_Artikel.Items.Add("100, 40x40L");
            CB_Artikel.Items.Add("101, 40x40L");
            CB_Artikel.Items.Add("102, 40x40L-1N");
            CB_Artikel.Items.Add("103, 40x40L-1N");
            CB_Artikel.Items.Add("104, 40x40L-1N");
            CB_Artikel.Items.Add("105, 40x40L-1N");
            CB_Artikel.Items.Add("106, 40x40L-1N");
            CB_Artikel.Items.Add("107, 40x40L-2N90");

            CB_Bewerker.Items.Add("Alexander Mosselaar");
            CB_Bewerker.Items.Add("Guido Ruijs");
            CB_Bewerker.Items.Add("Sander Reinders");
            CB_Bewerker.Items.Add("Job van der Mark");
            CB_Bewerker.Items.Add("Jurre ten Brink");
            CB_Bewerker.Items.Add("Anne-Maaike Hendriks");
            CB_Bewerker.Items.Add("Brand Petersen");
        }
        private void Write_Log()
        {
            using (StreamWriter writer = new StreamWriter("C:/Users/sande/Downloads/Boikoncode/Log.txt"))
            {
                int lengte = 50;
                writer.WriteLine("Projectnummer: " + tbGegevensProjectnr.Text);
                writer.WriteLine("Projectleider: " + tbGegevensProjectleider.Text);
                writer.WriteLine("Projectnaam: " + tbGegevensProjectnaam.Text);
                writer.WriteLine("Bewerker: " + CB_Bewerker.SelectedItem);
                writer.WriteLine("Artikel: " + CB_Artikel.SelectedItem);
                writer.WriteLine("Verwacht gebruik: " + tbAfschrijvingVerwachtGebruik.Text);
                writer.WriteLine("Actueel gebruik: " + tbAfschrijvingActueelGebruik.Text);
                writer.WriteLine("Huidige voorraad: " + (lengte - int.Parse(tbAfschrijvingActueelGebruik.Text)));
                writer.WriteLine("Reden: " + tbAfschrijvingReden.Text);
                writer.WriteLine("Sticker: " + fotoSticker.FileName);

            }
        }
    }
}



