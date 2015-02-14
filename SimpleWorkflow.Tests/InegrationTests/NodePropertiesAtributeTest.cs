using System;
using System.Linq;
using NUnit.Framework;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Tests.InegrationTests
{
    [TestFixture]
    public class NodePropertiesAttributeTest
    {
        [Test, Category("integration")]
        public void CheckAllActivitesHaveCorrectAtributes()
        {
            Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).
                Where(x => typeof(WFActivityBase).IsAssignableFrom(x) && !x.IsAbstract && x.IsPublic).ToArray();

            foreach(Type  type in types)
            {
                NodePropertiesAttribute attr = type.GetCustomAttributes(false).OfType<NodePropertiesAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    Assert.IsNotNull(attr.DefaultName);
                    Assert.IsNotNull(attr.LargeImage);
                    Assert.IsNotNull(attr.SmallImage);
                }
            }
        }
    }
}
