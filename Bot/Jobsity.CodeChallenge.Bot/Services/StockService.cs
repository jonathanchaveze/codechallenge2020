using Jobsity.CodeChallenge.Bot.Models;
using Jobsity.CodeChallenge.Bot.Utilities;
using System;
using System.Net;

namespace Jobsity.CodeChallenge.Bot.Services
{
    public class StockService
    {
        private readonly string _endpoint;

        public StockService(string endpoint)
        {
            _endpoint = endpoint;
        }

        public Stock GetStockByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException($"You must specify a stock code in the following format: /stock=code");
            }

            var stock = new Stock { Code = code?.ToUpper() };

            using (var client = new WebClient())
            {
                byte[] data = new byte[0];

                try
                {
                    data = client.DownloadData($"{_endpoint}&s={stock.Code}");
                }
                catch (Exception e)
                {
                    throw new Exception($"The stock service is unavailable at this momment. Please try again later: {e.Message}", e);
                }

                var lines = new CsvParser().Parse(data);

                if (lines.Count < 2)
                {
                    throw new Exception("The stock service is unavailable at this momment. Please try again later: Cannot parse response data");
                }
                
                var fields = lines[1];

                if (fields[6] == "N/D")
                {
                    throw new ArgumentException($"Stock code {stock.Code} is invalid. Please review your stock code and send the command again.");
                }

                stock.Price = Decimal.Parse(fields[6]);                
            }

            return stock;
        }
    }
}