using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace HiCAD.Plugin.BOIKON.Views
{
    /// <summary>
    /// Interaction logic for MagazijnDashboardControl.xaml
    /// </summary>
    public partial class MagazijnDashboardControl : UserControl
    {
        DBConnection dbconnhc;


        ObservableCollection<WarehouseDashboardLine> ActiveProjects = new ObservableCollection<WarehouseDashboardLine>();
        WarehouseDashboardLine selectedItem = null;



        public MagazijnDashboardControl()
        {
            InitializeComponent();

            dispatcherTimer.Tick += dispatcherTimer_Tick;

            GlobalVar.magazijnDashboardControl = this;

            dbconnhc = new DBConnection(GlobalConstVariables.dbhc, GlobalConstVariables.sqlserver, GlobalConstVariables.sqluser, GlobalConstVariables.sqlpass);


            LV_WarehouseDashboard.ItemsSource = ActiveProjects;

            foreach (var user in GlobalVar.GebruikersNamen)
            {
                CB_Responsible.Items.Add(user);
                CB_FilterInkoper.Items.Add(user);
            }

            LaadInterneAfleverlocaties();
        }

        private void LaadInterneAfleverlocaties()
        {
            if (File.Exists(@"\\192.168.0.3\boikon_tool_templates\Bestelling\Interne afleverlocaties.txt"))
            {
                foreach (string line in System.IO.File.ReadLines(@"\\192.168.0.3\boikon_tool_templates\Bestelling\Interne afleverlocaties.txt"))
                {
                    CB_OrderLocation.Items.Add(line);
                }
            }
        }


        #region Error label
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            errorLabel.Visibility = System.Windows.Visibility.Hidden;
        }
        private void showLabel(string text, bool iserror)
        {
            errorLabel.Text = text;
            errorLabel.Visibility = System.Windows.Visibility.Visible;

            if (iserror)
            {
                errorLabel.Background = Brushes.DarkRed;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 8);
            }
            else
            {
                errorLabel.Background = Brushes.DarkGreen;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            }

            dispatcherTimer.Start();
        }
        #endregion

        #region Listview sorteren

        ListSortDirection _lastDirection_LV_ProjectOrders = ListSortDirection.Ascending;


        private void LV_WarehouseDashboard_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            LV_WarehouseDashboardClicked(LV_WarehouseDashboard, headerClicked, ref _lastDirection_LV_ProjectOrders);
        }

        private void LV_WarehouseDashboardClicked(ListView listView, GridViewColumnHeader headerClicked, ref ListSortDirection _lastDirection)
        {
            try
            {
                ListSortDirection direction;

                if (headerClicked != null)
                {
                    if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        { direction = ListSortDirection.Descending; }
                        else
                        { direction = ListSortDirection.Ascending; }
                        string header = null;
                        if (headerClicked.Column.DisplayMemberBinding != null)
                        { header = ((Binding)headerClicked.Column.DisplayMemberBinding).Path.Path as string; }
                        else
                        {
                            switch (headerClicked.Column.Header.ToString())
                            {
                                case ("Projectnr"):
                                    header = "Projectnr";
                                    break;
                                case ("Projectomschrijving"):
                                    header = "ProjectDescription";
                                    break;
                                case ("Projectkar"):
                                    header = "ProjectStorageCar";
                                    break;
                                case ("Locatie"):
                                    header = "OrderLocation";
                                    break;
                                case ("Aanspreekpunt"):
                                    header = "Responsible";
                                    break;
                                case ("Inkoper(s)"):
                                    header = "Purchasers";
                                    break;
                                case ("Start montage"):
                                    header = "StartAssembly";
                                    break;
                                case ("Openstaande bestellingen"):
                                    header = "ActiveOrders";
                                    break;
                                case ("Eerstvolgende leverdatum"):
                                    header = "FirstDeliveryDate";
                                    break;


                                default:
                                    break;
                            }
                        }
                        SortListView(header, direction, listView.Items);
                        _lastDirection = direction;
                    }
                }
            }
            catch
            {
                showLabel("Er is iets mis gegaan met sorteren", true);
            }
        }

        private void SortListView(string sortBy, ListSortDirection direction, ItemCollection items)
        {
            try
            {
                ICollectionView dataView = CollectionViewSource.GetDefaultView(items);
                dataView.SortDescriptions.Clear();
                SortDescription sd = new SortDescription(sortBy, direction);
                dataView.SortDescriptions.Add(sd);
                dataView.Refresh();
            }
            catch
            {
                showLabel("Er is iets mis gegaan met sorteren", true);
            }
        }

        #endregion



        private void BTN_Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                CreateNotCreatedWarehouseDashboardProjects();
                GetActiveProjects();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BTN_ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            tbFilterProjectnr.Text = null;
            tbFilterProjectkar.Text = null;
            CB_FilterInkoper.SelectedIndex = -1;
        }





        private List<string> GetNotCreatedWarehouseDashboardProjects()
        {
            List<string> tmpList = new List<string>();

            try
            {
                var sql = PetaPoco.Sql.Builder;
                sql.Select("DISTINCT projectnr");
                sql.From("_ActieveProjecten");
                sql.Where("(projectnr) NOT IN(SELECT projectnr FROM MagazijnDashboardLines)");
                sql.OrderBy("projectnr");

                // Database verbinding maken
                PetaPoco.Database db = new PetaPoco.Database(dbconnhc.connectionstring, "System.Data.SqlClient");

                // Query uitvoeren en als lijst terug geven
                return db.Query<string>(sql).ToList();
            }
            catch (Exception ex)
            {
                showLabel("Fout bij ophalen van data: " + ex.Message, true);
            }

            return tmpList;
        }

        private void GetOrderData(WarehouseDashboardLine warehouseDashboardLine)
        {
            try
            {
                // Database verbinding maken
                PetaPoco.Database db = new PetaPoco.Database(dbconnhc.connectionstring, "System.Data.SqlClient");

                foreach (var purchaseorder in db.Query<PurchaseOrderData>(SQLActiveOrders(warehouseDashboardLine.Projectnr)))
                    warehouseDashboardLine.PurchaseOrders.Add(purchaseorder);

            }
            catch (Exception ex)
            {
                showLabel("Fout bij ophalen van data: " + ex.Message, true);
            }
        }

        private void CreateNotCreatedWarehouseDashboardProjects()
        {
            try
            {
                // Database verbinding maken
                PetaPoco.Database db = new PetaPoco.Database(dbconnhc.connectionstring, "System.Data.SqlClient");


                foreach (var projectnr in GetNotCreatedWarehouseDashboardProjects())
                {
                    WarehouseDashboardLine warehouseDashboardLine = new WarehouseDashboardLine()
                    {
                        Projectnr = projectnr
                    };

                    // Controle of de regel ondertussen al bestaat
                    int count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM MagazijnDashboardLines WHERE projectnr = @0", new string[] { projectnr });

                    // Alleen toevoegen als hij nog niet bestaat
                    if (count == 0)
                    {
                        db.Insert(warehouseDashboardLine);
                    }
                }
            }
            catch (Exception ex)
            {
                showLabel("Fout bij ophalen van data: " + ex.Message, true);
            }
        }

        private void GetActiveProjects()
        {
            try
            {
                lblActiveOrders.Text = "";
                ActiveProjects.Clear();

                List<WarehouseDashboardLine> tmpList = new List<WarehouseDashboardLine>();

                string FilterInkoper = CB_FilterInkoper.SelectedItem != null ? CB_FilterInkoper.SelectedItem.ToString() : null;
                string FilterProjectkar = tbFilterProjectkar.Text;

                // Database verbinding maken
                PetaPoco.Database db = new PetaPoco.Database(dbconnhc.connectionstring, "System.Data.SqlClient");

                var sync = new object();

                Parallel.ForEach(db.Query<WarehouseDashboardLine>(SQLActiveProjects()), warehouseDashboardLine =>
                {
                    GetOrderData(warehouseDashboardLine);

                    bool skip = false;

                    // filters toepassen
                    if (!string.IsNullOrEmpty(FilterInkoper))
                    {
                        // skippe als er niets is ingevuld
                        if (string.IsNullOrEmpty(warehouseDashboardLine.Purchasers))
                            skip = true;

                        // controle
                        if (!string.IsNullOrEmpty(warehouseDashboardLine.Purchasers) && !warehouseDashboardLine.Purchasers.Contains(FilterInkoper))
                            skip = true;                            
                    }

                    if (!string.IsNullOrEmpty(FilterProjectkar))
                    {
                        // skippe als er niets is ingevuld
                        if (string.IsNullOrEmpty(warehouseDashboardLine.ProjectStorageCar))
                            skip = true;

                        // controle
                        if (!string.IsNullOrEmpty(warehouseDashboardLine.ProjectStorageCar) && !warehouseDashboardLine.ProjectStorageCar.Contains(FilterProjectkar))
                            skip = true;
                    }


                    // Toevoegen aan de collection
                    if (!skip)
                    {
                        lock (sync)
                        {
                            tmpList.Add(warehouseDashboardLine);
                        }
                    }
                });


                // Lijst overzetten naar de observable collection (die kan niet gevuld worden vanuit een thread
                foreach (var warehouseDashboardLine in tmpList.OrderBy(o => o.Projectnr))
                    ActiveProjects.Add(warehouseDashboardLine);

                lblActiveOrders.Text = ActiveProjects.Count().ToString();
            }
            catch (Exception ex)
            {
                showLabel("Fout bij ophalen van data: " + ex.Message, true);
            }
        }

        private PetaPoco.Sql SQLActiveProjects(int ID = -1)
        {
            string filterProjectnr = string.IsNullOrEmpty(tbFilterProjectnr.Text) ? null : tbFilterProjectnr.Text;

            var sql = PetaPoco.Sql.Builder;
            
            sql.Select(
                new string[] {
                    "MagazijnDashboardLines.ID as ID",
                    "MagazijnDashboardLines.Projectnr as Projectnr",
                    "MagazijnDashboardLines.OrderLocation as OrderLocation",
                    "MagazijnDashboardLines.ProjectStorageCar as ProjectStorageCar",
                    "MagazijnDashboardLines.Responsible as Responsible",
                    "MagazijnDashboardLines.StartAssembly as StartAssembly",

                    "[001].dbo._HicadProjecten.Projectomschrijving as ProjectDescription",
                });

            sql.From("MagazijnDashboardLines");
            sql.InnerJoin("[001].dbo._HicadProjecten ON [HiCADPlugin].dbo.MagazijnDashboardLines.Projectnr COLLATE DATABASE_DEFAULT = [001].dbo._HicadProjecten.Projectnr COLLATE DATABASE_DEFAULT ");

            sql.Where(
                "(MagazijnDashboardLines.Projectnr IN (SELECT projectnr FROM _ActieveProjecten))" +
                (ID != -1 ? $"AND MagazijnDashboardLines.ID = {ID}" : "") +
                (filterProjectnr != null ? $"AND MagazijnDashboardLines.Projectnr LIKE '%{filterProjectnr}%'" : ""));

            sql.OrderBy("Projectnr");

            return sql;
        }

        private PetaPoco.Sql SQLActiveOrders(string projnr)
        {
            var sql = PetaPoco.Sql.Builder;

            sql.Select("*");
            sql.From("_ActieveBestellingen");
            sql.Where($"projectnr = '{projnr}'");

            return sql;
        }

        private void SaveSelectedItems()
        {
            try
            {
                if (selectedItem == null)
                    return;

                // Database verbinding maken
                PetaPoco.Database db = new PetaPoco.Database(dbconnhc.connectionstring, "System.Data.SqlClient");

                // 1 enkele updaten
                if (LV_WarehouseDashboard.SelectedItems.Count == 1)
                    db.Update(selectedItem);

                // Meerdere items updaten
                if (LV_WarehouseDashboard.SelectedItems.Count > 1)
                {
                    foreach (var item in LV_WarehouseDashboard.SelectedItems)
                    {
                        WarehouseDashboardLine warehouseDashboardLine = item as WarehouseDashboardLine;

                        warehouseDashboardLine.OrderLocation = selectedItem.OrderLocation;
                        warehouseDashboardLine.ProjectStorageCar = selectedItem.ProjectStorageCar;
                        warehouseDashboardLine.Responsible = selectedItem.Responsible;
                        warehouseDashboardLine.StartAssembly = selectedItem.StartAssembly;

                        db.Update(warehouseDashboardLine);
                    }

                    // Refreshen, anders wordt de listview niet geupdate
                    LV_WarehouseDashboard.Items.Refresh();
                }
                    
            }
            catch (Exception ex)
            {
                showLabel("Foutje: " + ex.Message, true);
            }
        }

        private void ResetSelectedItem()
        {
            try
            {
                if (selectedItem == null)
                    return;

                // Database verbinding maken
                PetaPoco.Database db = new PetaPoco.Database(dbconnhc.connectionstring, "System.Data.SqlClient");

                //To query a scalar
                foreach (var item in db.Query<WarehouseDashboardLine>(SQLActiveProjects(selectedItem.ID)))
                {
                    ActiveProjects[ActiveProjects.IndexOf(selectedItem)] = item;
                    selectedItem = item;
                    LV_WarehouseDashboard.SelectedItem = item;
                }

            }
            catch (Exception ex)
            {
                showLabel("Foutje: " + ex.Message, true);
            }
        }

        private void LV_WarehouseDashboard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LV_WarehouseDashboard.SelectedItem == null)
                selectedItem = null;

            if (LV_WarehouseDashboard.SelectedItems.Count == 1)
                selectedItem = LV_WarehouseDashboard.SelectedItem as WarehouseDashboardLine;

            if (LV_WarehouseDashboard.SelectedItems.Count > 1)
            {
                selectedItem = new WarehouseDashboardLine()
                {
                    ID = -1,
                    Projectnr = "meerdere geselecteerd",
                    ProjectDescription = "meerdere geselecteerd"                    
                };
            }

            GRD_SelectedWarehouseDashboardItem.DataContext = selectedItem;
        }



        private void BTN_SaveSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            SaveSelectedItems();
        }

        private void BTN_ResetSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            ResetSelectedItem();
        }
    }
}
