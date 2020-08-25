using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantLab
{
    public class Trade
    {      

        public const int SymbolNameLength = 3;

        public Trade(long timeStamp, string symbol, ulong quanity, ulong price)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (symbol.Length != SymbolNameLength)
            {
                string msg = FormattableString.Invariant($"{nameof(symbol)} must be {SymbolNameLength} characters, but the name is '{symbol}'.");
                throw new ArgumentOutOfRangeException(msg);
            }

            if (quanity == 0)
            {
                string msg = FormattableString.Invariant($"{nameof(quanity)} must be greater than zero.");
                throw new ArgumentOutOfRangeException();
            }

            if (price == 0)
            {
                string msg = FormattableString.Invariant($"{nameof(price)} must be greater than zero.");
                throw new ArgumentOutOfRangeException();
            }

            this.TimeStamp = timeStamp;
            this.Symbol = symbol;
            this.Quantity = quanity;
            this.Price = price;
        }

        public long TimeStamp   { get; private set; }

        public string Symbol  { get; private set; }

        public ulong Quantity { get; private set; }

        public ulong Price { get; private set; }
    }
}
