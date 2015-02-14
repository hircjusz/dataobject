using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SoftwareMind.SimpleWorkflow.Tests.InegrationTests
{
    [TestFixture]
    [Category("integration")]
    public class WFVariableCollectionTest
    {
        [Test]
        public void VariableClocectionSimpleOperation()
        {
            var vc = new WFVariableDefCollection();
            vc.Add(new WFVariableDef("a", 1));
            vc.Add(new WFVariableDef("b", 2));

            Assert.AreEqual(vc["a"].Value, 1);
            Assert.AreEqual(vc["b"].Value, 2);

            vc["a"].Value = 3;
            vc["b"].Value = 4;

            Assert.AreEqual(vc["a"].Value, 3);
            Assert.AreEqual(vc["b"].Value, 4);

            Assert.AreEqual(vc.Count, 2);

            vc.Remove("b");

            Assert.AreEqual(vc.Count, 1);

            vc.Remove("a");

            Assert.AreEqual(vc.Count, 0);
        }

        [Test]
        public void VariableClocectionSimpleOperationWithUsingIDictionaryInterface()
        {
            IDictionary<string, object> dic = new WFVariableDefCollection();
            dic["a"] = 1;
            dic["b"] = 2;

            Assert.AreEqual(dic["a"], 1);
            Assert.AreEqual(dic["b"], 2);

            dic["a"] = 3;
            dic["b"] = 4;

            Assert.AreEqual(dic.Count, 2);

            dic.Remove("b");

            Assert.AreEqual(dic.Count, 1);

            dic.Remove("a");

            Assert.AreEqual(dic.Count, 0);
        }

        [Test]
        public void VariableNameMonitoring()
        {
            WFVariableCollection vc = new WFVariableDefCollection();

            WFVariableDef a = new WFVariable("a", 1);
            WFVariableDef b = new WFVariable("b", 1);

            vc.Add(a);
            vc["b"] = b;

            Assert.True(vc.Contains(a));
            Assert.True(vc.ContainsKey(a.Name));

            Assert.True(vc.Contains(b));
            Assert.True(vc.ContainsKey(b.Name));

            b.Name = "c";

            Assert.True(vc.Contains(b));
            Assert.True(vc.ContainsKey(b.Name));

            Assert.Throws<ArgumentException>(new TestDelegate(() => b.Name = "a"));
            Assert.Throws<ArgumentException>(new TestDelegate(() => vc.Add(new WFVariable("a", 1))));

            vc.Remove(b);
            b.Name = "a";
        }

        [Test]
        public void VariableNameMonitoringWithUsingIDictionaryInterface()
        {
            WFVariableDefCollection vc = new WFVariableDefCollection();
            IDictionary<string, object> dic = vc;

            dic["a"] = 1;
            dic["b"] = 2;

            Assert.True(dic.Contains(new KeyValuePair<string, object>("a", 1)));
            Assert.True(dic.ContainsKey("a"));

            Assert.True(dic.Contains(new KeyValuePair<string, object>("b", 2)));
            Assert.True(dic.ContainsKey("b"));

            vc["b"].Name = "c";

            Assert.True(dic.Contains(new KeyValuePair<string, object>("c", 2)));
            Assert.True(dic.ContainsKey("c"));

            Assert.Throws<ArgumentException>(new TestDelegate(() => vc["c"].Name = "a"));
            Assert.Throws<ArgumentException>(new TestDelegate(() => vc.Add(new WFVariable("a", 1))));
        }
    }
}
