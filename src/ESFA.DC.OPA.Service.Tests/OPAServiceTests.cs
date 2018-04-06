using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ESFA.DC.OPA.Model;
using ESFA.DC.OPA.Model.Interface;
using ESFA.DC.OPA.Service.Builders;
using ESFA.DC.OPA.Service.Interface;
using ESFA.DC.OPA.Service.Interface.Rulebase;
using ESFA.DC.OPA.Service.Rulebase;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.OPA.Service.Tests
{
    public class OPAServiceTests
    {
        #region OPA Service Consructor Tests

        /// <summary>
        /// Return OPA Service
        /// </summary>
        [Fact(DisplayName = "OPA Service - Initiate"), Trait("OPA Service", "Unit")]
        public void OPAService_Initiate()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.Should().NotBeNull();
        }

        /// <summary>
        /// Return OPA Service
        /// </summary>
        [Fact(DisplayName = "OPA Service - Initiate and check entity name"), Trait("OPA Service", "Unit")]
        public void OPAService_InitiateAndCheckEntityName()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.EntityName.Should().BeEquivalentTo("Global");
        }

        /// <summary>
        /// Return OPA Service
        /// </summary>
        [Fact(DisplayName = "OPA Service - Initiate and check child entity name"), Trait("OPA Service", "Unit")]
        public void OPAService_InitiateAndCheckChildEntityName()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.Children.Single().EntityName.Should().BeEquivalentTo("Learner");
        }

        #endregion

        #region OPA Entity Structure Output Tests

        /// <summary>
        /// Return Global Entity and check Attributes
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global Attributes Exist"), Trait("OPA Service", "Unit")]
        public void OPAService_Global_Attributes_Exist()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.Attributes.Should().NotBeNull();
        }

        /// <summary>
        /// Return Global Entity and Count Attributes
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global Attributes Exist"), Trait("OPA Service", "Unit")]
        public void OPAService_Global_Attributes_Count()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.Attributes.Count.Should().Be(16);
        }

        /// <summary>
        /// Return OPA Service and check for Global entity
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global - isGlobal True"), Trait("OPA Service", "Unit")]
        public void OPAService_Global_isGlobal()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.IsGlobal.Should().BeTrue();
        }

        /// <summary>
        /// Return OPA Service and check Global entity name
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global EntityName Exist"), Trait("OPA Service", "Unit")]
        public void OPAService_Global_EntityName_Exists()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.EntityName.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Return OPA Service and check Global entity name
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global EntityName Exist"), Trait("OPA Service", "Unit")]
        public void OPAService_Global_EntityName_Correct()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.EntityName.Should().Be("global");
        }

        /// <summary>
        /// Return OPA Service and check Global Entity children
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global Children Exist"), Trait("OPA Service", "Unit")]
        public void OPAService_Global_Children_Exist()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.Children.Should().NotBeNull();
        }

        /// <summary>
        /// Return OPA Service and count Global Entity children
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global Children Count"), Trait("OPA Service", "Unit")]
        public void OPAService_Global_Children_Count()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.Children.Count.Should().Be(1);
        }

        /// <summary>
        /// Return OPA Service and check Global Entity children
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global Children EntityName Correct"), Trait("OPA Service", "Unit")]
        public void OPAService_Global_Children_EntityNameCorrect()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.Children.Select(e => e.EntityName).Should().BeEquivalentTo("Learner");
        }

        /// <summary>
        /// Return OPA Service and check Global Entity children
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global Children Attributes Exist"), Trait("OPA Service", "Unit")]
        public void OPAService_Global_Children_Attributes_Exist()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.Children.Select(a => a.Attributes).Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Return OPA Service and check Global Entity children
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global Children Attributes Count"), Trait("OPA Service", "Unit")]
        public void OPAService_Global_Children_AttributesCount()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.Children.Select(a => a.Attributes.Count).Should().BeEquivalentTo(7);
        }

        #endregion

        #region OPA Entity Data Output Tests

        /// <summary>
        /// Return OPA Service and check Global Entity attributes
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global Attribute UKPRN Exists"), Trait("OPA Service", "Unit")]
        public void OPAService_Data_Global_UKPRN_Exists()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            AttributeValue(result, "UKPRN").Should().NotBeNull();
        }

        /// <summary>
        /// Return OPA Service and check Global Entity attributes
        /// </summary>
        [Fact(DisplayName = "OPA Service - Global Attribute UKPRN Correct"), Trait("OPA Service", "Unit")]
        public void OPAService_Data_Global_UKPRN_Correct()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            AttributeValue(result, "UKPRN").Should().Be(12345678);
        }

        /// <summary>
        /// Return OPA Service and check Global Entity children attributes
        /// </summary>
        [Fact(DisplayName = "OPA Service - Learner Attribute LearnRefNumber Exists"), Trait("OPA Service", "Unit")]
        public void OPAService_Data_Learner_LearnRefNumber_Exists()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.Children.Select(l => l.LearnRefNumber).Should().NotBeNull();
        }

        /// <summary>
        /// Return OPA Service and check Global Entity children attributes
        /// </summary>
        [Fact(DisplayName = "OPA Service - Learner Attribute LearnRefNumber Correct"), Trait("OPA Service", "Unit")]
        public void OPAService_Data_Learner_LearnRefNumber_Correct()
        {
            // ARRANGE
            // Use Test Helpers

            // ACT
            var result = MockOPAService(TestDataEntity());

            // ASSERT
            result.Children.Select(l => l.LearnRefNumber).Should().BeEquivalentTo("Learner1");
        }

        #endregion

        #region Test Helpers

        #region Test Data

        private IDataEntity TestDataEntity()
        {
            IDataEntity globalEntity = new DataEntity("Global")
            {
                Attributes = new Dictionary<string, IAttributeData>()
                {
                    { "UKPRN", new AttributeData("UKPRN", 12345678) }
                }
            };

            globalEntity.AddChild(
                new DataEntity("Learner")
                {
                    Attributes = new Dictionary<string, IAttributeData>()
                    {
                        { "LearnRefNumber", new AttributeData("LearnRefNumber", "Learner1") }
                    }
                });

            return globalEntity;
        }

        #endregion

        #region Create Test Service

        private IRulebaseProvider RulebaseProviderMock()
        {
            var rulebaseZipPath =
                Assembly.GetCallingAssembly().GetManifestResourceNames()
                .Where(n => n.Contains("Rulebase"))
                .Select(r => r).SingleOrDefault();

            return new RulebaseProvider(rulebaseZipPath);
        }

        private IRulebaseProviderFactory MockRulebaseProviderFactory()
        {
            var mock = new Mock<IRulebaseProviderFactory>();

            mock.Setup(m => m.Build()).Returns(RulebaseProviderMock());

            return mock.Object;
        }

        private IOPAService MockTestObject()
        {
            return new OPAService(new SessionBuilder(), new OPADataEntityBuilder(new DateTime(2017, 8, 1)), MockRulebaseProviderFactory());
        }

        private IDataEntity MockOPAService(IDataEntity dataEntity)
        {
            var serviceMock = new Mock<IOPAService>();

            serviceMock.Setup(sm => sm.ExecuteSession(dataEntity)).Returns(TestDataEntity);
            var mockData = MockTestObject();

            return mockData.ExecuteSession(dataEntity);
        }

        #endregion

        #region Get Session Values

        private object AttributeValue(IDataEntity dataEntity, string attributeName)
        {
            return dataEntity.Attributes.Where(k => k.Key == attributeName).Select(v => v.Value.Value).Single();
        }

        #endregion

        #endregion

    }
}
