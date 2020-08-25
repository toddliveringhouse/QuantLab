using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantLab
{
    public class SymbolInfoResult
    {

        public const int SymbolNameLength = 3;

        public SymbolInfoResult(string symbolName)
        {
            if (symbolName == null)
            {
                throw new ArgumentNullException(nameof(symbolName));
            }

            if (symbolName.Length != SymbolNameLength)
            {
                string msg = FormattableString.Invariant($"{nameof(symbolName)} must be {SymbolNameLength} characters, but the name is '{symbolName}'.");
                throw new ArgumentOutOfRangeException(msg);
            }

            this.SymbolName = symbolName;
        }

        public void AddTradeToSummaryInfo(Trade trade)
        {
            if (trade == null)
            {
                throw new ArgumentNullException(nameof(trade));
            }

            if (trade.Symbol != this.SymbolName)
            {
                string msg = FormattableString.Invariant($"The trade is for '{nameof(trade.Symbol)}' but the summary info is for '{this.SymbolName}'.");
            }

            // Compute MaxTimeGapTicks
            if (PreviousTradeTick == null)
            {
                this.MaxTimeGapTicks = 0;
            }
            else
            {
                long timeGapTicks = trade.TimeStamp - this.PreviousTradeTick.Value;
                this.MaxTimeGapTicks = timeGapTicks > this.MaxTimeGapTicks ? timeGapTicks : this.MaxTimeGapTicks;
            }

            // Compute MaxPrice
            this.MaxTradePrice = trade.Price > this.MaxTradePrice ? trade.Price : this.MaxTradePrice;

            // Must be computed before averages.
            this.TotalQuanityTraded = this.TotalQuanityTraded + trade.Quantity;

            // Compute the averages          
            this.RunningWeightedAveragePrice = ComputeRunningWeightedAveragePriceFromTrade(trade);
            this.RunningQuantityInWeightedAveragePrice = this.TotalQuanityTraded;

            // Cast because we want WeightedAveragePrice to be truncated.
            this.WeightedAveragePrice = (ulong)this.RunningWeightedAveragePrice;

            PreviousTradeTick = trade.TimeStamp;
        }

        private double ComputeRunningWeightedAveragePriceFromTrade(Trade trade)
        {
            double average = 0;
            try
            {
                average = ((RunningQuantityInWeightedAveragePrice * RunningWeightedAveragePrice) + (trade.Quantity * trade.Price)) / this.TotalQuanityTraded;          
            }
            catch (Exception e)
            {
                // Add values to exception that causes the computation to fail.
                string msg = FormattableString.Invariant($"{nameof(RunningQuantityInWeightedAveragePrice)} {RunningQuantityInWeightedAveragePrice}, {nameof(RunningWeightedAveragePrice)} {RunningWeightedAveragePrice}, {nameof(trade.Quantity)} {trade.Quantity}, {nameof(trade.Price)} {trade.Price}");
                throw new Exception (msg, e);
            }

            return average;
        }

        // Should be 3 characters
        public string SymbolName { get; private set; }

        public long MaxTimeGapTicks { get; private set; }

        // Note as trades are added it is assumed that the timestamp tick increases
        public long? PreviousTradeTick { get; private set; }

        public ulong MaxTradePrice { get; private set; }

        public ulong TotalQuanityTraded { get; private set; }

        // Weighted average is a rounded down value of the RunningWeightedAveragePrice
        public ulong WeightedAveragePrice { get; private set; }

        // Running is used to keep a decimal portion of the average so that rounding
        // error is not introducted on every updated for a trade.
        public double RunningWeightedAveragePrice { get; private set; }

        public ulong RunningQuantityInWeightedAveragePrice { get; private set; }

    }
}
