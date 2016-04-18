using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSpec;
using NSpec.Runner;

namespace UnitTestProject1
{
    [TestClass]
    public class MSTest1 : nspec
    {
        private string something = String.Empty;

        [TestMethod, TestSpecification]
        public void When_Writing_A_Test_Spec()
        {
            string theTruth = String.Empty;

            context["and when dealing with other people,"] = () =>
            {
                before = () => theTruth = "true";
                it["it will be honest."] = () => Assert.AreEqual(theTruth, "true");
                it["it will open doors for old ladies."] = () => Assert.AreEqual(theTruth, "true");

                context["and when dealing with old dudes that are grumpy,"] = () =>
                {
                    before = () => theTruth = "false";

                    it["it will tell a knock knock joke."] = () => Assert.AreEqual(theTruth, "false");
                };

                context["and when dealing with old dudes that are not grumpy,"] = () =>
                {
                    it["it will give them a high five."] = () => Assert.AreEqual(theTruth, "true");
                };
            };
        }
    }
}
