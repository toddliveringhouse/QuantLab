using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantLab
{
    class Program
    {
        static void Main(string[] args)
        {
            // Note: Try different input files.  The output file cannot already exist!

            string fileName = @"C:\Temp\input.csv";
        //  string fileName = @"C:\Temp\input1.txt";
            int maxQueueSize = 1000;
            int timoutMs = 50;
            string outputFileName = @"C:\Temp\output.csv";
            var processTradeFile = new ProcessTradeFile(fileName, maxQueueSize, timoutMs, outputFileName);
            processTradeFile.Process();
        }
    }
}
