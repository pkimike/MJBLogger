using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MJBLogger;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        readonly IMJBLog _log = new MJBLog(@"Test", @"P:\_temp");
        [TestMethod]
        public void TestCache()
        {
            var Log = new MJBLog()
            {
                LogName = @"TestCache",
                CachedMode = true
            };
            for (int x=0; x<=100; x++)
            {
                Log.Info($"Test message #{x}");
            }

            Log.CachedMode = false;
        }

        [TestMethod]
        public void TestGE()
        {
            var Log = new MJBLog(@"Test", @"P:\_temp");

            if (Log.Level.GE(LogLevel.Verbose))
            {
                Log.Echo(@"Got here!");
            }
        }

        [TestMethod]
        public void TestEcho()
        {
            _log.Echo(@"Testing");
        }
    }
}
