using System;
using System.Linq;
using System.Reflection;
using ESFA.DC.OPA.Service.Interface.Rulebase;
using ESFA.DC.OPA.Service.Rulebase;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.OPA.Service.Tests.Rulebase
{
    public class RulebaseProviderTests
    {
        /// <summary>
        /// Return RulebaseProvider
        /// </summary>
        [Fact(DisplayName = "RulebaseProvider - Instance Exists"), Trait("RulebaseProvider", "Unit")]
        public void RulebaseProvider_Exists()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var rulebaseProvider = TestRuleBaseProvider();

            // ASSERT
            rulebaseProvider.Should().NotBeNull();
        }

        /// <summary>
        /// Return RulebaseProvider
        /// </summary>
        [Fact(DisplayName = "RulebaseProvider - GetStream Exists"), Trait("RulebaseProvider", "Unit")]
        public void RulebaseProviderGetStream_Exists()
        {
            // ARRANGE
            var rulebaseProvider = TestRuleBaseProvider();

            // ACT
            var stream = rulebaseProvider.GetStream(Assembly.GetExecutingAssembly());

            // ASSERT
            stream.Should().NotBeNull();
        }

        /// <summary>
        /// Return RulebaseProviderFactory
        /// </summary>
        [Fact(DisplayName = "RulebaseProviderFactory - Build Successful"), Trait("RulebaseProviderFactory", "Unit")]
        public void RulebaseProviderFactory_BuildSuccess()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var rulebaseProvider = RulebaseProviderFactoryMock().Build();

            // ASSERT
            rulebaseProvider.Should().NotBeNull();
        }

        /// <summary>
        /// Return RulebaseProviderFactory
        /// </summary>
        [Fact(DisplayName = "RulebaseProviderFactory - Build Values Not Null"), Trait("RulebaseProviderFactory", "Unit")]
        public void RulebaseProviderFactory_BuildValues()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var rulebaseProvider = RulebaseProviderFactoryMock().Build();

            // ASSERT
            rulebaseProvider.GetStream(Assembly.GetExecutingAssembly()).Should().NotBeNull();
        }

        #region Test Helpers

        private IRulebaseProviderFactory RulebaseProviderFactoryMock()
        {
            Mock<IRulebaseProviderFactory> rulebaseProviderFactoryMock = new Mock<IRulebaseProviderFactory>();

            rulebaseProviderFactoryMock.Setup(rm => rm.Build()).Returns(TestRuleBaseProvider());

            return  rulebaseProviderFactoryMock.Object;
        }

        private IRulebaseProvider TestRuleBaseProvider()
        {
            var rulebaseZipPath =
               Assembly.GetExecutingAssembly().GetManifestResourceNames()
               .Where(n => n.Contains("Rulebase"))
               .Select(r => r).SingleOrDefault();

            return new RulebaseProvider(rulebaseZipPath);
        }
        
        #endregion
    }
}
