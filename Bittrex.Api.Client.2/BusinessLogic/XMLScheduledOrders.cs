using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace BusinessLayer.BusinessLogic
{
    public enum OrderType
    {
        Buy = 0,
        Sell = 1,
    }

    public class ScheduleHandler
    {
        public static bool AddOrder(string marketName, Order anOrder)
        {
            Schedule schedule;
            marketName = marketName.ToUpper().Trim();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Schedule));
            using (FileStream inputStream = new FileStream("Schedule.xml", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var sr = new StreamReader(inputStream);
                schedule = (Schedule)xmlSerializer.Deserialize(sr);
            }
            if (!schedule.Markets.Any(d => d.Name == marketName))
            {
                schedule.Markets.Add(new Market { Name = marketName, Orders = new List<Order>() });
            }
            Market market = schedule.Markets.FirstOrDefault(d => d.Name == marketName);

            //RemoveOrder(marketName, anOrder);
            market.Orders.Add(anOrder);

            //Serialise
            using (FileStream outputStream = new FileStream("Schedule.xml", FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(outputStream);
                xmlSerializer.Serialize(sw, schedule);
            }

            return true;
        }

        public static void RemoveOrder(string marketName, Order anOrder)
        {
            Schedule schedule;
            marketName = marketName.ToUpper().Trim();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Schedule));
            using (Stream inputStream = File.OpenRead("Schedule.xml"))
            {
                schedule = (Schedule)xmlSerializer.Deserialize(inputStream);
            }
            Market market = schedule.Markets.FirstOrDefault(d => d.Name == marketName);

            if (market != null && market.Orders.Any(id => id.OrderId == anOrder.OrderId))
            {
                market.Orders.Remove(anOrder);
            }
            //Serialise
            //todo turn this into filestream if required to be used at a later date
            using (Stream outputStream = File.OpenWrite("Schedule.xml"))
            {
                xmlSerializer.Serialize(outputStream, schedule);
            }
        }

        public static List<Order> GetOrders(string marketName)
        {
            Schedule schedule;
            marketName = marketName.ToUpper().Trim();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Schedule));

            using (FileStream inputStream = new FileStream("Schedule.xml", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var sr = new StreamReader(inputStream);
                schedule = (Schedule)xmlSerializer.Deserialize(sr);
            }
            var market = schedule.Markets.Where(d => d.Name == marketName);
            return market.FirstOrDefault().Orders;
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
        public decimal OrderTotal
        {
            get { return Qty * Bid; }
            set { }
        }
        public bool IsSent
        {
            get { return Sent != null; }
            set { }
        }
        [XmlElement]
        public string LastOutcome { get; set; }
    }
}
