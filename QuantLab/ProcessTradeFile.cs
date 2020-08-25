using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantLab
{
    public class ProcessTradeFile
    {
        public ProcessTradeFile(string inputFileName, int maxQueueSize, int takeTimeout, string outputFileName)
        {
            if (maxQueueSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxQueueSize));
            }

            if (takeTimeout <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(takeTimeout));
            }

            this.InputFileName = inputFileName;
            this.MaxQueueSize = maxQueueSize;
            this.tradeQueue = new BlockingCollection<Trade>(maxQueueSize);

            this.symbolSummary = new Dictionary<string, SymbolInfoResult>();
            this.allInputsProcessed = false;
            this.TakeTimeout = takeTimeout;
            this.OutputFileName = outputFileName;
        }

        public string InputFileName { get; private set; }

        public string OutputFileName { get; private set; }

        public int MaxQueueSize { get; private set; }

        public int TakeTimeout { get; private set; }

        private BlockingCollection<Trade> tradeQueue;

        private Dictionary<string, SymbolInfoResult> symbolSummary;

        private bool allInputsProcessed;

        public void Process()
        {
            var addTradeTask = Task.Run(() => AddAllTradeRecordsToQueue());

            var addResultsTask = Task.Run(() => AddTradeRecordsToResult());

            Task.WaitAll(addTradeTask);
            Task.WaitAll(addResultsTask);

            // Now output file.
            var writeOutputFile = new WriteOutputFile(this.OutputFileName, symbolSummary);
            writeOutputFile.Write();
        }


        public void AddAllTradeRecordsToQueue()
        {
            var trades = ReadTrades.ReadTradesFromFile(this.InputFileName);
            foreach (var trade in trades)
            {
                this.tradeQueue.Add(trade);
            }

            this.allInputsProcessed = true;
        }


        public void AddTradeRecordsToResult()
        {
            while (!this.allInputsProcessed || this.tradeQueue.Count > 0)
            {
                bool taken = this.tradeQueue.TryTake(out Trade trade, this.TakeTimeout);
                if (taken)
                {
                    bool found = symbolSummary.ContainsKey(trade.Symbol);
                    SymbolInfoResult result;
                    if (!found)
                    {
                        result = new SymbolInfoResult(trade.Symbol);
                        symbolSummary.Add(result.SymbolName, result);
                    }
                    else
                    {
                        result = symbolSummary[trade.Symbol];
                    }

                    result.AddTradeToSummaryInfo(trade);
                }
            }
        }
    }
}
