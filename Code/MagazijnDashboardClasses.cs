using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiCAD.Plugin.BOIKON.Views
{
    [PetaPoco.TableName("MagazijnDashboardLines")]
    [PetaPoco.PrimaryKey("ID")]
    public class WarehouseDashboardLine
    {
        public int ID { get; set; }
        public string Projectnr { get; set; }
        public string OrderLocation { get; set; }
        public string ProjectStorageCar { get; set; }
        public string Responsible { get; set; }
        public DateTime? StartAssembly { get; set; }

        [PetaPoco.ResultColumn]
        public string ProjectDescription { get; set; }

        [PetaPoco.ResultColumn]
        public string Purchasers
        {
            get
            {
                string seperator = ", ";
                string tmp = "";

                foreach (var purchaser in PurchaseOrders.Select(o => o.sentby).Distinct().OrderBy(p => p))
                    tmp += purchaser + seperator;

                tmp = tmp.TrimEnd(seperator.ToCharArray());

                return tmp;
            }
        }

        [PetaPoco.ResultColumn]
        public int ActiveOrders
        {
            get
            {
                return PurchaseOrders.Count;
            }
        }

        [PetaPoco.ResultColumn]
        public DateTime? FirstDeliveryDate
        {
            get
            {
                var sortedList = PurchaseOrders.OrderBy(o => o.FirstDeliveryDate);

                if (sortedList.Count() == 0)
                    return null;

                return sortedList.First().FirstDeliveryDate;
            }
        }

        [PetaPoco.ResultColumn]
        public double? DaysTillFirstDeliveryDate
        {
            get
            {
                if (FirstDeliveryDate == null)
                    return null;

                return ((DateTime)FirstDeliveryDate - DateTime.Now).Days;
            }
        }


        public List<PurchaseOrderData> PurchaseOrders = new List<PurchaseOrderData>();
    }

    public class PurchaseOrderData
    {
        public string bestelnummer { get; set; }
        public string projectnr { get; set; }
        public string sentby { get; set; }
        public string eersteLeverdatum { get; set; }

        public DateTime? FirstDeliveryDate
        {
            get
            {
                if (string.IsNullOrEmpty(eersteLeverdatum))
                    return null;

                return DateTime.Parse(eersteLeverdatum, new System.Globalization.CultureInfo("nl-NL"));
            }
        }
    }

}