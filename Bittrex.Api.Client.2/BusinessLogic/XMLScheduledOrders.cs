using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace BusinessLayer.BusinessLogic
{
    public enum OrderType
    {
        Buy = 0,
        Sell = 1,
    }

    public class ScheduleHandler
    {
        public static Schedule MasterSchedule { get; set; }


        public static void LoadMasterScheduleFromFile()
        {
            //If there is no schedule.xml file. Create one now
            if (!File.Exists("Schedule.xml"))
            {
                using (FileStream outputStream = new FileStream("Schedule.xml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    XmlSerializer s = new XmlSerializer(typeof(Schedule));
                    StreamWriter sw = new StreamWriter(outputStream);
                    s.Serialize(sw, MasterSchedule);
                }
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Schedule));
            try{
                using (FileStream inputStream = new FileStream("Schedule.xml", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)){
                    var sr = new StreamReader(inputStream);
                    MasterSchedule = (Schedule) xmlSerializer.Deserialize(sr);
                }
            }
            catch{
                throw new Exception("XML File is Malformed");
            }

        }

        public static void SaveMasterSchedule()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Schedule));
            //Serialise
            using (FileStream outputStream = new FileStream("Schedule.xml", FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(outputStream);
                xmlSerializer.Serialize(sw, MasterSchedule);
            }
        }

        public static bool AddOrder(string marketName, Order anOrder)
        {
            if (MasterSchedule == null) LoadMasterScheduleFromFile();
            if (MasterSchedule != null && MasterSchedule.Markets.All(d => d.Name != marketName))
            {
                MasterSchedule.Markets.Add(new Market { Name = marketName, Orders = new List<Order>() });
            }
            if (MasterSchedule != null)
            {
                Market market = MasterSchedule.Markets.FirstOrDefault(d => d.Name == marketName);

                market?.Orders.Add(anOrder);
            }

            return true;
        }

        public static void RemoveOrder(string marketName, Order anOrder)
        {
            marketName = marketName.ToUpper().Trim();

            Market market = MasterSchedule.Markets.FirstOrDefault(d => d.Name == marketName);

            if (market != null && market.Orders.Any(id => id.OrderId == anOrder.OrderId))
            {
                market.Orders.Remove(anOrder);
            }
        }

        public static List<Order> GetOrders(string marketName)
        {
            marketName = marketName.ToUpper().Trim();
            var market = MasterSchedule.Markets.Where(d => d.Name == marketName);
            return market.FirstOrDefault()?.Orders;
        }

    }

    [XmlRoot("Schedule")]
    public class Schedule
    {
        [XmlElement(ElementName = "Market")]
        public List<Market> Markets { get; set; }
    }

    public class Market
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement(ElementName = "Orders")]
        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        [XmlAttribute]
        public Guid OrderId => Guid.NewGuid();

        [XmlAttribute]
        public OrderType OrderType { get; set; }

        [XmlElement]
        public decimal Bid { get; set; }

        [XmlElement]
        public decimal Qty { get; set; }

        [XmlElement]
        public DateTime? Expiry { get; set; }

        [XmlElement]
        public DateTime? Sent { get; set; }

        [XmlElement]
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public decimal OrderTotal {
            get { return Qty * Bid; }
            set { }
        }
        public bool IsSent {
            get { return Sent != null; }
            set { }
        }
        [XmlElement]
        public string LastOutcome { get; set; }
    }
}
