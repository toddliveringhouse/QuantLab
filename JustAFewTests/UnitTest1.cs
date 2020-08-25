using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantLab;

namespace JustAFewTests
{
    [TestClass]
    public class UnitTest1
    {

        //  ****** Note many more unit tests for production code ******************

        [TestMethod]
        public void EnsureInputFileRunsWithNoExceptions()
        {
            string fileName = @"C:\Temp\input.csv";
            int maxQueueSize = 1000;
            int timoutMs = 50;
            string outputFileName = @"C:\Temp\output.csv";
            var processTradeFile = new ProcessTradeFile(fileName, maxQueueSize, timoutMs, outputFileName);
            processTradeFile.Process();
        }

        [TestMethod]
        public void EnsureSymbolNameTooLongThrowsException()
        {
            string symbolNameThatIsTooLong = "AAA445TTT";
            bool exceptionCaught = false;
            try
            {
                SymbolInfoResult symbolInfoResult = new SymbolInfoResult(symbolNameThatIsTooLong);
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);
        }

        [TestMethod]
        public void EnsureInvalidQueueSizeThrowsException()
        {
            string fileName = @"C:\Temp\input.csv";
            int maxQueueSizeThatMakesNoSense = -2;
            int timoutMs = 50;
            string outputFileName = @"C:\Temp\output.csv";

            bool exceptionCaught = false;
            try
            {
                var processTradeFile = new ProcessTradeFile(fileName, maxQueueSizeThatMakesNoSense, timoutMs, outputFileName);
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);
        }

    }
}
