using Jobsity.CodeChallenge.Bot.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Jobsity.CodeChallenge.Testing
{
    [TestClass]
    public class StockServiceTesting
    {
        private readonly string _stockEndpoint = "https://stooq.com/q/l/?f=sd2t2ohlcv&h&e=csv";
        private StockService _stockService;

        public StockServiceTesting()
        {
            _stockService = new StockService(_stockEndpoint);
        }

        [TestMethod]
        public void QueryEmptyStockCode()
        {
            Assert.ThrowsException<ArgumentException>(() => _stockService.GetStockByCode(""));
        }

        [TestMethod]
        public void GetStockInfo()
        {
            var stock = _stockService.GetStockByCode("aapl.us");

            Assert.AreNotEqual(0, stock.Price);
        }

        [TestMethod]
        public void QueryInvalidStockCode()
        {
            Assert.ThrowsException<ArgumentException>(() => _stockService.GetStockByCode("xxxxxx"));
        }
    }
}
