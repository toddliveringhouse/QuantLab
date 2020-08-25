using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantLab
{
    public class WriteOutputFile
    {
        public WriteOutputFile(string outputFileName, Dictionary<string, SymbolInfoResult> symbolSummary)
        {
            if (outputFileName == null)
            {
                throw new ArgumentNullException(nameof(outputFileName));
            }

            if (symbolSummary == null)
            {
                throw new ArgumentNullException(nameof(symbolSummary));
            }

            if (File.Exists(outputFileName))
            {
                // Don't write over an existing file.
                string msg = FormattableString.Invariant($"'{outputFileName}' already exists.");
                throw new InvalidOperationException(msg);
            }

            this.OutputFileName = outputFileName;
            this.SymbolSummary = symbolSummary;
        }

        public string OutputFileName { get; private set; }

        public Dictionary<string, SymbolInfoResult> SymbolSummary { get; private set; }

        public void Write()
        {
            var orderedsummaryList = SymbolSummary.Select(x => x.Value).OrderBy(x => x.SymbolName).ToList();

            using (StreamWriter writer = new StreamWriter(new FileStream(this.OutputFileName, FileMode.Create, FileAccess.Write)))
            {
                foreach (var summary in orderedsummaryList)
                {
                    string line = FormattableString.Invariant($"{summary.SymbolName},{summary.MaxTimeGapTicks},{summary.TotalQuanityTraded},{summary.WeightedAveragePrice},{summary.MaxTradePrice}");
                    writer.WriteLine(line);
                }
            }
        }
    }
}
