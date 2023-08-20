using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace AtomicCore.Tests
{
    [TestClass()]
    public class ExpressionCalculaterTests
    {
        [TestMethod()]
        public void GetValueTest()
        {
            //Func<string, string> fun1 = d => d;
            //var res1 = fun1.Invoke("123");

            //Delegate showTiwice = fun1;
            //var res2 = showTiwice.DynamicInvoke("123");

            Expression<Func<string, string, string>> func = (x, y) => y;
            var obj = ExpressionCalculater.GetValue(func, new object[] { null });

            Assert.IsTrue(null != obj);
        }
    }
}