using Daylily.Bot.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class CommandTester
    {
        private const string FullCmd = "/test -switch1 -arg \"123\" \"freeArg is here\" -switch2";
        private const int Count = 100000;
        //[TestMethod]
        public void TestParamDividerV1()
        {
            for (int i = 0; i < Count; i++)
            {
                CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV1());
                ca.Analyze(FullCmd);
            }
        }

        [TestMethod]
        public void TestParamDividerV2()
        {
            for (int i = 0; i < Count; i++)
            {
                CommandAnalyzer ca = new CommandAnalyzer(new ParamDividerV2());
                ca.Analyze(FullCmd);
            }
        }

        [TestMethod]
        public void TestStreamParamDivider()
        {
            for (int i = 0; i < Count; i++)
            {
                CommandAnalyzer ca = new CommandAnalyzer(new StreamParamDivider());
                ca.Analyze(FullCmd);
            }
        }

        //[DataTestMethod]
        //[DataRow(1, 1, 2)]
        //public void DataTestMethod1(int a, int b, int result)
        //{
        //    Assert.AreEqual(result, a + b);
        //}
    }
}
