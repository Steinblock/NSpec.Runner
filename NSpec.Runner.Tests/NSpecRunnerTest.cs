using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSpec;
using NSpec.Runner;

namespace NSpec.Runner.Tests
{
    [TestClass]
    public class NSpecRunnerTest : nspec
    {

        [TestMethod, TestSpecification]
        public void When_Writing_A_Test_Spec()
        {
            bool theTruth = false;

            context["and when dealing with other people,"] = () =>
            {
                before = () => theTruth = true;
                it["it will be honest."] = () => theTruth.should_be(true);
                it["it will open doors for old ladies."] = () => theTruth.should_be(true);

                context["and when dealing with old dudes that are grumpy,"] = () =>
                {
                    before = () => theTruth = false;

                    it["it will tell a knock knock joke."] = () => theTruth.should_be(false);
                };

                context["and when dealing with old dudes that are not grumpy,"] = () =>
                {
                    it["it will give them a high five."] = () => theTruth.should_be(true);
                };
            };
        }

        [TestMethod, TestSpecification]
        public void When_Writing_A_Test_Spec_With_Failues()
        {
            bool theTruth = false;

            context["and when dealing with other people,"] = () =>
            {
                before = () => theTruth = false;
                it["it will be honest."] = () => theTruth.should_be(true);
                it["it will open doors for old ladies."] = () => theTruth.should_be(true);

                context["and when dealing with old dudes that are grumpy,"] = () =>
                {
                    before = () => theTruth = true;

                    it["it will tell a knock knock joke."] = () => theTruth.should_be(false);
                };

                context["and when dealing with old dudes that are not grumpy,"] = () =>
                {
                    it["it will give them a high five."] = () => theTruth.should_be(true);
                };
            };
        }

        [TestMethod, TestSpecification]
        public void When_Writing_A_Test_Spec_With_Pendings()
        {
            context["and you haven't finished the specs"] = () =>
            {
                it["you should declare them as TODO"] = todo;
                it["and the test result should be inconclusiv"] = () => { };
                it["and the output should contain Pending = 1"] = () => { };
            };
        }

        [TestMethod, TestSpecification, Ignore]
        public void When_Writing_A_Test_Spec_With_Ingnore_Attribute()
        {
            it["it should be ignored"] = () => { Assert.Fail("it should be ignored"); };
        }

        [TestMethod, TestSpecification(FailFast=true)]
        public void When_Writing_A_Test_Spec_With_FailFast()
        {
            it["it should fail on first assertation"] = () => { 1.is_greater_than(2); };
            it["and not here"] = () => { 2.is_less_than(1); Assert.Fail("this should not be called"); };
        }

    }
}
