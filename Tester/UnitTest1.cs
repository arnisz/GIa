using GI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using zz = GI.Formats.Formats;

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
                var w = gatherer.GetFileInformations(null);
                var i = w.GetAwaiter();
                var g = i.GetResult();

                var actual = g.Find(x => x.Filename == "#actual.sta");
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

        [TestMethod]
        public void DoublesTest()
        {
            double n = zz.GetDouble("0,00314159E3");
            double m = 3.14159;
            double d = 0.01;

            Assert.AreEqual(n, m, d);

            n = zz.GetDouble("3.14159E0");
            m = 3.14159;
            d = 0.01;

            Assert.AreEqual(n, m, d);


        }




    }


}
