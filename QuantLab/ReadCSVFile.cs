using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace QuantLab
{
    public static class ReadTrades
    {       
        public static IEnumerable<Trade> ReadTradesFromFile(string fileName)
        {
            string fileLine;
            int lineNumber = 1;          
            using (var reader = File.OpenText(fileName))
            {
                while((fileLine = reader.ReadLine()) != null)
                {
                    // Use yield and IEnumerable since file may contain too many records to return in one list.
                    Trade trade = ParseLineToCreateTrade(fileLine, lineNumber);

                    lineNumber++;
                    yield return trade;
                }
            }          
        }

        public static Trade ParseLineToCreateTrade(string lineToParse, int lineNumber)
        {
            Trade trade = null;
            try
            {
                // line should be of format: <TimeStamp>,<Symbol>,<Quantity>,<Price>
                var rows = lineToParse.Split(',');
                if (rows.Length != 4)
                {
                    string msg = FormattableString.Invariant($"Line should contain 4 items but it contained {rows.Length}");
                    throw new Exception(msg);
                }

                bool parsed = long.TryParse(rows[0], out long timeStamp);
                if (!parsed)
                {
                    throw new Exception("unable to parse time stamp");
                }

                string symbol = rows[1];

                parsed = ulong.TryParse(rows[2], out ulong quantity);
                if (!parsed)
                {
                    throw new Exception("unable to parse quantity");
                }

                parsed = ulong.TryParse(rows[3], out ulong price);
                if (!parsed)
                {
                    throw new Exception("unable to parse price");
                }

                trade = new Trade(timeStamp, symbol, quantity, price);
            }
            catch(Exception e)
            {
                string msg = FormattableString.Invariant($"Line '{lineToParse}', line number {lineNumber}, unable to parse.");
                throw new Exception(msg, e);
            }

            return trade;
        }
    }
}
