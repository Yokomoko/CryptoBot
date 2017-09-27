using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    /// <summary>    
    /// The result of the /public/getmarketsummaries end point
    /// This contains a summary of the last 24 hours trading for the market
    /// </summary>
    public class MarketSummary
    {
        private string _MarketName;


        public String MarketName
        {
           get { return _MarketName; }
            set
            {
                if (value != _MarketName){
                    _MarketName = value;
                }
            }
        }

        public Decimal High { get; set; }
        public Decimal Low { get; set; }
        public Decimal Volume { get; set; }
        public Decimal Last { get; set; }
        public Decimal BaseVolume { get; set; }
        public DateTime TimeStamp { get; set; }
        public Decimal Bid { get; set; }
        public Decimal Ask { get; set; }
        public int OpenBuyOrders { get; set; }
        public int OpenSellOrders { get; set; }
        public Decimal PrevDay { get; set; }
        public DateTime Created { get; set; }

        public decimal ChangePercentage => decimal.Round((Bid - PrevDay) / PrevDay * 100, 2);
    }
}
