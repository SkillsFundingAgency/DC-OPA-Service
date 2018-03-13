using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ESFA.DC.OPA.Model;
using ESFA.DC.OPA.Model.Interface;
using ESFA.DC.OPA.Service.Builders;
using ESFA.DC.OPA.Service.Interface.Builders;
using FluentAssertions;
using Oracle.Determinations.Engine;
using Xunit;

namespace ESFA.DC.OPA.Service.Tests.Builders
{
    public class OPADataEntityBuilderTests
    {
        #region CreateOPADataEntity Tests

        /// <summary>
        /// Return Data Entity
        /// </summary>
        [Fact(DisplayName = "OPADataEntityBuilder - CreateOPADataEntity"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_CreateDataEntity_Exists()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = GetOutputEntity();

            //ASSERT
            outputEntity.Should().NotBeNull();
        }

        /// <summary>
        /// Return Data Entity
        /// </summary>
        [Fact(DisplayName = "OPADataEntityBuilder - CreateOPADataEntity entities correct"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_CreateDataEntity_EntitiesCorrect()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = GetOutputEntity();

            //ASSERT
            outputEntity.EntityName.Should().Be("global");
            outputEntity.Children.Select(e => e.EntityName).FirstOrDefault().Should().Be("Learner");
        }

        /// <summary>
        /// Return Data Entity
        /// </summary>
        [Fact(DisplayName = "OPADataEntityBuilder - CreateOPADataEntity entities count correct"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_CreateDataEntity_EntitiesCountCorrect()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = GetOutputEntity();

            //ASSERT
            outputEntity.IsGlobal.Should().BeTrue();
            outputEntity.Children.Select(e => e.EntityName).Count().Should().Be(1);
        }

        /// <summary>
        /// Return Data Entity
        /// </summary>
        [Fact(DisplayName = "OPADataEntityBuilder - CreateOPADataEntity Attributes correct"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_CreateDataEntity_AttributesCorrect()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = GetOutputEntity();
            var attributes = outputEntity.Attributes.ToArray();
            var childAtttributes = outputEntity.Children.SelectMany(a => a.Attributes).ToArray();

            //ASSERT
            var ukprn = DecimalStrToInt(GetAttributeValues(attributes, "UKPRN"));
            var larsVersion = GetAttributeValues(attributes, "LARSVersion");
            var learnRefNumber = GetAttributeValues(childAtttributes, "LearnRefNumber");

            ukprn.Should().Be(12345678);
            larsVersion.Should().Be("Version_005");
            learnRefNumber.Should().Be("TestLearner");
        }

        /// <summary>
        /// Return Data Entity
        /// </summary>
        [Fact(DisplayName = "OPADataEntityBuilder - CreateOPADataEntity Attributes count correct"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_CreateDataEntity_AttributesCountCorrect()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = GetOutputEntity();

            //ASSERT
            outputEntity.Attributes.Count.Should().Be(16);
            outputEntity.Children.Select(a => a.Attributes).Count().Should().Be(1);
        }

        #endregion

        #region MapOpaToEntity Tests

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapOpaToEntity - Global Exists"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapOpaToEntity_GlobalExists()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = SetupMapToOpDataEntity();

            //ASSERT
            outputEntity.EntityName.Should().NotBeNull();
        }

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapOpaToEntity - Global Parent Should not exist"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapOpaToEntity_GlobalNoParent()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = SetupMapToOpDataEntity();

            //ASSERT
            outputEntity.Parent.Should().BeNull();
        }

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapOpaToEntity - Global Correct"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapOpaToEntity_GlobalCorrect()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = SetupMapToOpDataEntity();

            //ASSERT
            outputEntity.EntityName.Should().Be("global");
            outputEntity.Attributes.Count().Should().Be(16);
        }

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapOpaToEntity - Child Exists"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapOpaToEntity_ChildExists()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = SetupMapToOpDataEntity();

            //ASSERT
            outputEntity.Children.Select(c => c.EntityName).Should().NotBeNull();
        }

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapOpaToEntity - Child's Parent Exists"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapOpaToEntity_ChildsParentExists()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = SetupMapToOpDataEntity();

            //ASSERT
            outputEntity.Children.Select(c => c.Parent).Should().NotBeNull();
        }

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapOpaToEntity - Child's Parent Correct"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapOpaToEntity_ChildsParentCorrect()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = SetupMapToOpDataEntity();

            //ASSERT
            outputEntity.Children.Select(c => c.Parent.EntityName).Should().BeEquivalentTo("global");
            outputEntity.Children.Select(c => c.Parent.Attributes.Count).Should().BeEquivalentTo(16);
        }

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapOpaToEntity - Child Correct"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapOpaToEntity_ChildCorrect()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var outputEntity = SetupMapToOpDataEntity();

            //ASSERT
            outputEntity.Children.Select(e => e.EntityName).Should().BeEquivalentTo("Learner");
            outputEntity.Children.Select(a => a.Attributes).Count().Should().Be(1);
        }

        #endregion

        #region MapAttributes Tests

        /// <summary>
        /// Return Data Entity and check attributes are as expected
        /// </summary>
        [Fact(DisplayName = "MapAttributes - Attributes Exist"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapAttributes_AttributesExist()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var dataEntity = SetupMapAttributes();

            //ASSERT
            dataEntity.Attributes.Should().NotBeNull();
        }

        /// <summary>
        /// Return Data Entity and check attributes are as expected
        /// </summary>
        [Fact(DisplayName = "MapAttributes - Attributes Correct Count"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapAttributes_AttributesCorrectCount()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var dataEntity = SetupMapAttributes();

            //ASSERT
            dataEntity.Attributes.Count.Should().Be(16);
        }

        /// <summary>
        /// Return Data Entity and check attributes are as expected
        /// </summary>
        [Fact(DisplayName = "MapAttributes - Attributes Correct"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapAttributes_AttributesCorrect()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var dataEntity = SetupMapAttributes();

            //ASSERT
            var attributes = dataEntity.Attributes.ToArray();
            var ukprn = DecimalStrToInt(GetAttributeValues(attributes, "UKPRN"));

            ukprn.Should().Be(12345678);
        }

        #endregion

        #region MapOpaAttributeToDataEntity Tests

        /// <summary>
        /// Return Data Entity and check attributes are as expected
        /// </summary>
        [Fact(DisplayName = "MapOPAAttributesToDataEntity - Attributes Exist"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapOpaAttributesToDataEntity_AttributesExist()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var attributeList = SetupMapOpaAttribute();

            //ASSERT
            attributeList.Should().NotBeNull();

        }

        /// <summary>
        /// Return Data Entity and check attributes are as expected
        /// </summary>
        [Fact(DisplayName = "MapOPAAttributesToDataEntity - Attributes Count"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapOpaAttributesToDataEntity_AttributesCount()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var attributeList = SetupMapOpaAttribute();

            //ASSERT
            attributeList.Count.Should().Be(16);

        }

        /// <summary>
        /// Return Data Entity and check attributes are as expected
        /// </summary>
        [Fact(DisplayName = "MapOPAAttributesToDataEntity - Attributes Correct"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapOpaAttributesToDataEntity_AttributesCorrect()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var attributeList = SetupMapOpaAttribute();

            //ASSERT
            var ukprn = DecimalStrToInt(GetAttributeValues(attributeList, "UKPRN"));
            ukprn.Should().Be(12345678);

        }

        #endregion

        #region MapEntities Tests

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapEntities - Global Exists"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapEntities_GlobalExists()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var dataEntity = SetupMapEntities();

            //ASSERT
            dataEntity.Should().NotBeNull();

        }

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapEntities - Global Correct"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapEntities_GlobalCorrect()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var dataEntity = SetupMapEntities();

            //ASSERT
            dataEntity.EntityName.Should().Be("global");

        }

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapEntities - Global Children Exists"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapEntities_GlobalChildrenExists()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var dataEntity = SetupMapEntities();

            //ASSERT
            dataEntity.Children.Should().NotBeNull();

        }

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapEntities - Global Children Count"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapEntities_GlobalChildrenCount()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var dataEntity = SetupMapEntities();

            //ASSERT
            dataEntity.Children.Count.Should().Be(1);

        }

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapEntities - Global Children Correct"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapEntities_GlobalChildrenCorrect()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var dataEntity = SetupMapEntities();

            //ASSERT
            dataEntity.Children.Select(e => e.EntityName).Should().BeEquivalentTo("Learner");

        }

        /// <summary>
        /// Return Data Entity and check entities are as expected
        /// </summary>
        [Fact(DisplayName = "MapEntities - Global Children Correct Parent"), Trait("OPA To Data Entity Builder", "Unit")]
        public void DataEntityBuilder_MapEntities_GlobalChildrenCorrectParent()
        {
            //ARRANGE
            //Use Test Helpers

            //ACT
            var dataEntity = SetupMapEntities();

            //ASSERT
            dataEntity.Children.Select(p => p.Parent.EntityName).Should().BeEquivalentTo("global");

        }

        #endregion

        #region Test Helpers

        private IDataEntity TestGlobalEntity()
        {
            IDataEntity globalEntity = new DataEntity("global")
            {
                Attributes = new Dictionary<string, IAttributeData>()
                {
                    {"UKPRN", new AttributeData("UKPRN", 12345678)},
                    {"LARSVersion", new AttributeData("LARSVersion", "Version_005")}
                }
            };

            globalEntity.AddChild(
                new DataEntity("Learner")
                {
                    Attributes = new Dictionary<string, IAttributeData>()
                    {
                        {"LearnRefNumber", new AttributeData("LearnRefNumber", "TestLearner")}
                    }
                });

            return globalEntity;
        }

        private const string RulebaseZipPath = @".Rulebase.Loans Bursary 17_18.zip";

        private EntityInstance TestEntityInstance()
        {
            ISessionBuilder sessionBuilder = new SessionBuilder();
            Session session;
            var assembly = Assembly.GetCallingAssembly();
            var rulebaseLocation = assembly.GetName().Name + RulebaseZipPath;

            using (Stream stream = assembly.GetManifestResourceStream(rulebaseLocation))
            {
                session = sessionBuilder.CreateOPASession(stream, TestGlobalEntity());
            }

            session.Think();

            return session.GetGlobalEntityInstance();
        }

        private IDataEntity GetOutputEntity()
        {
            IOPADataEntityBuilder createDataEntity = new OPADataEntityBuilder();
            EntityInstance entityInstance = TestEntityInstance();
            IDataEntity dataEntity = null;

            return createDataEntity.CreateOPADataEntity(entityInstance, dataEntity);
        }

        private IDataEntity SetupMapToOpDataEntity()
        {
            var mapToDataEntity = new OPADataEntityBuilder();
            EntityInstance entityInstance = TestEntityInstance();
            IDataEntity dataEntity = null;

            return mapToDataEntity.MapOpaToEntity(entityInstance, dataEntity);
        }

        private List<IAttributeData> SetupMapOpaAttribute()
        {
            var builder = new OPADataEntityBuilder();
            var instance = TestEntityInstance();
            IDictionary<string, IAttributeData> attributeDictionary = new Dictionary<string, IAttributeData>();
            var rbAttributes = instance.GetEntity().GetAttributes();

            foreach (RBAttr r in rbAttributes)
            {
                var attData = builder.MapOpaAttributeToDataEntity(instance, r);
                attributeDictionary.Add(attData.Name, attData);
            }

            return attributeDictionary.Values.ToList();
        }

        private IDataEntity SetupMapAttributes()
        {
            var mapAttributes = new OPADataEntityBuilder();
            EntityInstance entityInstance = TestEntityInstance();
            IDataEntity dataEntity = new DataEntity(entityInstance.GetEntity().GetName());

            mapAttributes.MapAttributes(entityInstance, dataEntity);

            return dataEntity;
        }

        private IDataEntity SetupMapEntities()
        {
            var mapEntities = new OPADataEntityBuilder();
            var instance = TestEntityInstance();
            var childEntities = instance.GetEntity().GetChildEntities();
            var dataEntity = new DataEntity(instance.GetEntity().GetName());

            mapEntities.MapEntities(instance, childEntities, dataEntity);

            return dataEntity;
        }

        private string GetAttributeValues(KeyValuePair<string, IAttributeData>[] attributes, string attributeName)
        {
            var attributeValue = attributes.Where(a => a.Key == attributeName)
                .Select(v => v.Value.Value).FirstOrDefault().ToString();

            return attributeValue;
        }

        private string GetAttributeValues(List<IAttributeData> attributes, string attributeName)
        {
            var attributeValue = attributes.Where(n => n.Name == attributeName)
                .Select(v => v.Value).FirstOrDefault().ToString();

            return attributeValue;
        }

        private int DecimalStrToInt(string value)
        {
            var valueInt = value.Substring(0, value.IndexOf('.', 0));
            return Int32.Parse(valueInt);
        }

        #endregion
    }
}