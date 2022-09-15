using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GI;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Tester
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void Ganterer_start()
        {
            GIGatherer gatherer = GIGatherer.Instance;
            Assert.IsNotNull(gatherer);
        }

        [TestMethod]
        public async Task ReadAllFilesbyFTP()
        {
            GIGatherer gatherer = GIGatherer.Instance;
            gatherer.SetIP("192.168.17.99");
            Task.Run(async () =>
            {
                var w =  gatherer.GetFileInformations(null);
                var i = w.GetAwaiter();
                var g = i.GetResult();

                var actual =  g.Find(x => x.Filename == "#actual.sta");
                if (actual != null)
                {
                    Assert.IsTrue(actual.Content.Contains("CONFIGURATION STABLE"));
                }
                else
                {
                    Assert.Fail();
                }
            }).GetAwaiter().GetResult();
        }
    
    
    }


}
