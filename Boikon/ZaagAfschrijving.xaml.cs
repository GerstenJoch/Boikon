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
using Boikon;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using MySql.Data.MySqlClient;

namespace Boikon
{
    /// <summary>
    /// Interaction logic for ZaagAfschrijving.xaml
    /// </summary>
    /// 
    public partial class ZaagAfschrijving : UserControl
    {
        
        OpenFileDialog fotoSticker = new OpenFileDialog();
        OpenFileDialog csv = new OpenFileDialog();
        List<string> artikelen = new List<string>();
        List<string> profielen = new List<string>();
        IDictionary<string, string> CSVfiles = new Dictionary<string, string>();
        string profiel, lengte, selected_artikel, huidige_voorraad;
        
        public ZaagAfschrijving()
        {
            InitializeComponent();
        }
        private void BTN_CSVUpload_Click(object sender, RoutedEventArgs e)
        {
            csv.Multiselect = true;
            Nullable<bool> dialogOK = csv.ShowDialog();
            if (dialogOK == true)
            {
                Init_Bewerkers();
                Retrieve_CSV();
                Project_Select.Visibility = Visibility.Collapsed;
                ZaagInterface.Visibility = Visibility.Visible;
            }
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
                if (CB_Profiel.SelectedIndex == -1)
                {
                    lblZaagProfiel.Foreground = Brushes.Red;
                    lblZaagError.Visibility = Visibility.Visible;
                }
                else
                {
                    lblZaagProfiel.Foreground = Brushes.Black;
                }
                if (!(String.IsNullOrEmpty(tbZaagAantal.Text) || !int.TryParse(tbZaagAantal.Text, out i) || CB_Artikel.SelectedIndex == -1 || CB_Bewerker.SelectedIndex == -1 || CB_Profiel.SelectedIndex == -1))
                {
                    lblZaagError.Visibility = Visibility.Collapsed;
                    Zaag_Log();
                    int verwachtGebruik = Int32.Parse(tbZaagAantal.Text) * Int32.Parse(lengte);
                    string msgtext = "Gelukt! Uw taak is correct verstuurd.";
                    string txt = "Versturen Succesvol";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxResult result = MessageBox.Show(msgtext, txt, button);
                    tbZaagAantal.IsEnabled = false;
                    CB_Artikel.IsEnabled = false;
                    CB_Bewerker.IsEnabled = false;
                    CB_Profiel.IsEnabled = false;
                    CB_GegevensProjectnaam.IsEnabled = false;
                    CB_GegevensProjectnr.IsEnabled = false;
                    tbAfschrijvingVerwachtGebruik.Text = verwachtGebruik.ToString();

                }
            }

            if (sender.Equals(BTN_Push)) {        //Afschrijving
                double i;
                if (String.IsNullOrEmpty(tbAfschrijvingActueelGebruik.Text) || !Double.TryParse(tbAfschrijvingActueelGebruik.Text, out i))
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
                if (!(String.IsNullOrEmpty(tbAfschrijvingActueelGebruik.Text) || !Double.TryParse(tbAfschrijvingActueelGebruik.Text, out i) || (String.IsNullOrEmpty(tbAfschrijvingReden.Text) && tbAfschrijvingVerwachtGebruik.Text.Trim() != tbAfschrijvingActueelGebruik.Text.Trim()) || !(fotoSticker.FileNames.Length > 0)))
                {
                    lblAfschrijvingError.Visibility = Visibility.Collapsed;
                    Afschrijving_Log();
                    string msgtext = "Gelukt! Uw afschrijving correct verstuurd.";
                    string txt = "Afschrijving Succesvol";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxResult result = MessageBox.Show(msgtext, txt, button);
                    //Clear Recent Input
                    tbAfschrijvingReden.Clear(); tbAfschrijvingActueelGebruik.Clear(); tbAfschrijvingVerwachtGebruik.Clear(); tbZaagAantal.Clear();
                    lblAfschrijvingStickerConfirmation.Visibility = Visibility.Collapsed; tbZaagLengte.Clear(); tbGegevensProjectleider.Clear(); 
                    CB_Artikel.Items.Clear(); CB_Bewerker.Items.Clear(); CB_GegevensProjectnr.Items.Clear(); CB_GegevensProjectnaam.Items.Clear(); CB_Profiel.Items.Clear();
                    Project_Select.Visibility = Visibility.Visible;
                    ZaagInterface.Visibility = Visibility.Collapsed;
                    artikelen.Clear(); profielen.Clear(); CSVfiles.Clear();
                    tbZaagAantal.IsEnabled = true;
                    CB_Artikel.IsEnabled = true;
                    CB_Bewerker.IsEnabled = true;
                    CB_Profiel.IsEnabled = true;
                    CB_GegevensProjectnaam.IsEnabled = true;
                    CB_GegevensProjectnr.IsEnabled = true;
                }
            }
        }
        private void CB_GegevensProjectnr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CB_GegevensProjectnr.SelectedIndex != -1)
            {
                CB_Profiel.Items.Clear(); CB_Artikel.Items.Clear(); artikelen.Clear(); profielen.Clear(); tbZaagLengte.Clear(); tbZaagAantal.Clear(); CB_Bewerker.SelectedIndex = -1;
                using (var reader = new StreamReader(CSVfiles[CB_GegevensProjectnr.SelectedItem.ToString()]))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        line = line.Replace(";;;;;;;;;;;;", "");
                        string[] delimiter = { ";," };
                        var values = line.Split(delimiter, System.StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i].StartsWith("Ingevuld door"))
                            {
                                var items = values[i].Split(':');
                                tbGegevensProjectleider.Text = items[1].Trim(';');
                            }
                            string[] artikelSplit = values[i].Split(';');
                            if (artikelSplit.Length > 2)
                            {
                                for (int j = 0; j < artikelSplit.Length; j++)
                                {
                                    int k;
                                    if (!String.IsNullOrEmpty(artikelSplit[j]) && int.TryParse(artikelSplit[j], out k) && !artikelSplit.First().StartsWith("Project"))
                                    {
                                        artikelen.Add(values[i]);
                                        break;
                                    }
                                }
                            }
                            if (values[i].StartsWith(";;"))
                            {
                                var profielLijst = values[i].Remove(0, 2);
                                if (profielLijst.First() != ';' && !(profielLijst.StartsWith("Profiellijst")))
                                {
                                    var curProfiel = profielLijst.Split(';');
                                    if (!profielen.Contains(curProfiel[0]))
                                    {
                                        profielen.Add(profielLijst);
                                        CB_Profiel.Items.Add(curProfiel[0]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void CB_Artikel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CB_Artikel.SelectedIndex != -1)
            {
                var content = CB_Artikel.SelectedItem.ToString();
                string[] delimiter = { ": " };
                content = (content.Split(delimiter, StringSplitOptions.None))[1];
                foreach (var artikel in artikelen)
                {
                    var artikelSplit = artikel.Split(';');

                    if (artikelSplit[1].StartsWith(content))
                    {
                        selected_artikel = artikel;
                        Console.WriteLine(selected_artikel);
                    }
                }
            }
        }
        private void CB_Profiel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CB_Artikel.Items.Clear();
            if (CB_Profiel.SelectedIndex != -1)
            {
                var content = CB_Profiel.SelectedItem.ToString();
                string[] delimiter = { ", " };
                content = (content.Split(delimiter, StringSplitOptions.RemoveEmptyEntries))[0];
                foreach (var artikel in artikelen)
                {
                    var artikelSplit = artikel.Split(';');

                    if (artikelSplit[2].StartsWith(content))
                    {
                        CB_Artikel.Items.Add(artikelSplit[0] + ": " + artikelSplit[1] + ": " + artikelSplit[2] + ": " + artikelSplit[3] + ": " + artikelSplit[4] + ": " + artikelSplit[5] + ": " + artikelSplit[6] + ": " + artikelSplit[7]);
                        profiel = artikelSplit[2];
                    }
                }
                foreach (var profile in profielen)
                {
                    var profielSplit = profile.Split(';');
                    var profileContent = (profielSplit[0].Split(delimiter, StringSplitOptions.RemoveEmptyEntries))[0];
                    if (profiel.StartsWith(profileContent) || profiel.Equals(profileContent))
                    {
                        if (profielSplit.Length > 1)
                        {
                            lengte = (profielSplit[1].Split(' '))[0];
                        }
                    }
                }
                tbZaagLengte.Text = lengte + " m/stuk";
            }
        }
        private void BTN_Upload_Click(object sender, RoutedEventArgs e)
        {
            fotoSticker.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            fotoSticker.DefaultExt = ".png";
            Nullable<bool> dialogOK = fotoSticker.ShowDialog();
            if (dialogOK == true)
            {
                    string sFileName = fotoSticker.FileName;
                    string[] UploadName = sFileName.Split('\\');
                    lblAfschrijvingStickerConfirmation.Content = UploadName.Last();
                    lblAfschrijvingStickerConfirmation.Visibility = Visibility.Visible;
                    lblAfschrijvingUploadSticker.Foreground = Brushes.Black;
                    lblAfschrijvingStickerConfirmation.Foreground = Brushes.Black;
            }
        }
        private void Init_Bewerkers()
        {
            using (var conn = new MySqlConnection(SQLData.connstr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT first_name, last_name FROM bewerkers ORDER BY first_name", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var fname = reader.GetString(0);
                            var lname = reader.GetString(1);
                            CB_Bewerker.Items.Add(fname + " " + lname);
                        }
                    }
                }
            }

        }
        private void Retrieve_CSV()
        {
            foreach (var file in csv.FileNames)
            {
                string[] FileName = file.Split('\\');
                var projectFileNr = (FileName.Last().Split(' '))[0];
                CB_GegevensProjectnr.Items.Add(projectFileNr); ;
                CSVfiles.Add(projectFileNr, file);
            }

        }
        private void Zaag_Log()
        {
            MySqlConnection conn = new MySqlConnection(SQLData.connstr);
            string insertQuery = "INSERT INTO boikon.zaag_log(project_nummer,project_leider,project_naam,bewerker,profiel,artikel,tijd) VALUES('" + CB_GegevensProjectnr.SelectedItem + "','"
                + tbGegevensProjectleider.Text + "','" + CB_GegevensProjectnaam.SelectedItem + "','" + CB_Bewerker.SelectedItem + "','"
                + CB_Profiel.SelectedItem + "','" + CB_Artikel.SelectedItem + "','" + DateTime.Now + "')";
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
            try
            {
                if (cmd.ExecuteNonQuery() == 1)
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            conn.Close();
        }
        private void Afschrijving_Log()
        {
            Update_Voorraad();
            MySqlConnection conn = new MySqlConnection(SQLData.connstr);
            string insertQuery = "INSERT INTO boikon.afschrijving_log(project_nummer,project_leider,project_naam,bewerker,artikel,verwacht_gebruik,actueel_gebruik,huidige_voorraad,reden,sticker,tijd) VALUES('" + CB_GegevensProjectnr.SelectedItem + "','"
                + tbGegevensProjectleider.Text + "','" + CB_GegevensProjectnaam.SelectedItem + "','" + CB_Bewerker.SelectedItem + "','" + CB_Artikel.SelectedItem + "','" + tbAfschrijvingVerwachtGebruik.Text + "','" + tbAfschrijvingActueelGebruik.Text + "','"
                + huidige_voorraad + "','" + tbAfschrijvingReden.Text + "','" + fotoSticker.FileName + "','" + DateTime.Now + "')";
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
            try
            {
                if (cmd.ExecuteNonQuery() == 1)
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            conn.Close();
        }
        private void Update_Voorraad()
        {
            string path = CSVfiles[CB_GegevensProjectnr.SelectedItem.ToString()];
            List<String> lines = new List<String>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] delimiter = { ";" };
                    string[] split = line.Split(delimiter, System.StringSplitOptions.None);
                    if (line.StartsWith(selected_artikel))
                    {
                        split[4] = (Convert.ToDouble(split[4].Replace(',', '.')) - Convert.ToDouble(tbAfschrijvingActueelGebruik.Text.Replace(',', '.'))).ToString().Replace('.', ',');
                        huidige_voorraad = split[4];
                        line = String.Join(";", split);
                    }
                    lines.Add(line);
                }
            }
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                foreach (String line in lines)
                    writer.WriteLine(line);
            }
        }
    }
}



