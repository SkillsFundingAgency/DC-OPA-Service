using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.OPA.Model;
using ESFA.DC.OPA.Model.Interface;
using ESFA.DC.OPA.Service.Builders;
using ESFA.DC.OPA.Service.Interface.Builders;
using FluentAssertions;
using Oracle.Determinations.Engine;
using Oracle.Determinations.Masquerade.IO;
using Oracle.Determinations.Masquerade.Util;
using Xunit;

namespace ESFA.DC.OPA.Service.Tests.Builders
{
    public class SessionBuilderTests
    {
        #region Create Session Tests

        /// <summary>
        /// Return Session
        /// </summary>
        [Fact(DisplayName = "SessionBuilder - Create Session"), Trait("OPA Session Builder", "Unit")]
        public void CreateOPASession_SessionExists()
        {
            //ARRANGE
            ISessionBuilder sessionBuilder = new SessionBuilder();

            //ACT
            var session = CreateSessionTestSession(sessionBuilder);

            //ASSERT
            session.Should().NotBeNull();
        }

        /// <summary>
        /// Return two unique sessions
        /// </summary>
        [Fact(DisplayName = "SessionBuilder - Create Sessions"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_MapToOPA_InstancesExists()
        {
            //ARRANGE
            ISessionBuilder createSession1 = new SessionBuilder();
            ISessionBuilder createSession2 = new SessionBuilder();

            //ACT
            var session1 = CreateSessionTestSession(createSession1);
            var session2 = CreateSessionTestSession(createSession2);

            //ASSERT
            session1.Should().NotBeNull();
            session2.Should().NotBeNull();

            session2.Should().NotBeSameAs(session1);
        }

        /// <summary>
        /// Return Session and check whether the Rulebase is already initialised
        /// </summary>
        [Fact(DisplayName = "SessionBuilder - RulebaseInitialised false "), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_CreateSession_rulebaseNotInitialised()
        {
            //ARRANGE
            SessionBuilder sessionRBNotInitialised = new SessionBuilder();

            //ACT
            var rbInitPre = sessionRBNotInitialised.RulebaseInitialised;
            CreateSessionTestSession(sessionRBNotInitialised);
            var rbInitPost = sessionRBNotInitialised.RulebaseInitialised;

            //ASSERT
            rbInitPre.Should().BeFalse();
            rbInitPost.Should().BeTrue();
        }

        /// <summary>
        /// Return Session and check whether the Rulebase is already initialised
        /// </summary>
        [Fact(DisplayName = "SessionBuilder - RulebaseInitialised true "), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_CreateSession_rulebaseInitialised()
        {
            //ARRANGE
            SessionBuilder sessionRBInitialised = new SessionBuilder();

            //ACT
            var rbInitPreFirst = sessionRBInitialised.RulebaseInitialised;
            CreateSessionTestSession(sessionRBInitialised);
            var rbInitPostFirst = sessionRBInitialised.RulebaseInitialised;

            var rbInitPreSecond = sessionRBInitialised.RulebaseInitialised;
            CreateSessionTestSession(sessionRBInitialised);
            var rbInitPostSecond = sessionRBInitialised.RulebaseInitialised;

            //ASSERT
            rbInitPreFirst.Should().BeFalse();
            rbInitPostFirst.Should().BeTrue();
            rbInitPreSecond.Should().BeTrue();
            rbInitPostSecond.Should().BeTrue();
        }

        /// <summary>
        /// Return Session and check whether the Rulebase is as expected
        /// </summary>
        [Fact(DisplayName = "SessionBuilder - Rulebase Exists"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_CreateSession_RulebaseExists()
        {
            //ARRANGE
            SessionBuilder sessionRBExists = new SessionBuilder();

            //ACT
            CreateSessionTestSession(sessionRBExists);

            //ASSERT
            sessionRBExists.Rulebase.Should().NotBeNull();
        }

        /// <summary>
        /// Return Session and check whether the Rulebase is as expected
        /// </summary>
        [Fact(DisplayName = "SessionBuilder - Rulebase correct"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_CreateSession_RulebaseCorrect()
        {
            //ARRANGE
            SessionBuilder sessionRBCorrect = new SessionBuilder();

            //ACT
            CreateSessionTestSession(sessionRBCorrect);
            var rulebaseName = sessionRBCorrect.Rulebase.GetBaseFileName();

            //ASSERT
            rulebaseName.Should().BeEquivalentTo("Loans Bursary 17_18");
        }

        /// <summary>
        /// Return Session and check whether the Engine is as expected
        /// </summary>
        [Fact(DisplayName = "SessionBuilder - Engine Exists"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_CreateSession_EngineExists()
        {
            //ARRANGE
            SessionBuilder sessionEngineExists = new SessionBuilder();

            //ACT
            CreateSessionTestSession(sessionEngineExists);

            //ASSERT
            sessionEngineExists.Engine.Should().NotBeNull();
        }

        /// <summary>
        /// Return Session and check whether the Engine is as expected
        /// </summary>
        [Fact(DisplayName = "SessionBuilder - Engine correct"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_CreateSession_EngineCorrect()
        {
            //ARRANGE
            SessionBuilder sessionEngineCorrect = new SessionBuilder();

            //ACT
            CreateSessionTestSession(sessionEngineCorrect);
            var engineVersion = sessionEngineCorrect.Engine.GetVersion();

            //ASSERT
            engineVersion.Should().BeEquivalentTo("10.4.4.21");
        }

        #endregion

        #region MapDataEntityToOPA Tests

        /// <summary>
        /// Return OPA Instance
        /// </summary>
        [Fact(DisplayName = "MapToOPA - Instance exists"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_MapGlobalDataEntityToOpa_InstanceExists()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();
            var session = MapToOPATestSession();

            //ACT
            sessionBuilder.MapGlobalDataEntityToOpa(TestGlobalEntity(), session, session.GetGlobalEntityInstance());

            //ASSERT
            session.Should().NotBeNull();
        }

        /// <summary>
        /// Return OPA Session and check if it has changed
        /// </summary>
        [Fact(DisplayName = "MapToOPA - Global Entity updated"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_MapGlobalDataEntityToOpa_GlobalEntityUpdated()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();
            var sessionPre = MapToOPATestSession();
            var sessionPost = MapToOPATestSession();

            //ACT
            sessionBuilder.MapGlobalDataEntityToOpa(TestGlobalEntity(), sessionPost, sessionPost.GetGlobalEntityInstance());
            var ukprnPre = AttributeValue(sessionPre, "UKPRN");
            var ukprnPost = AttributeValue(sessionPost, "UKPRN");

            //ASSERT
            sessionPost.Should().NotBe(sessionPre);
            ukprnPre.Should().BeNull();
            ukprnPost.Should().Be(12345678);
        }

        /// <summary>
        /// Return OPA Session and check if it has changed
        /// </summary>
        [Fact(DisplayName = "MapToOPA - Global Entity Children updated"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_MapGlobalDataEntityToOpa_GlobalEntityChildrenUpdated()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();
            var sessionPre = MapToOPATestSession();
            var sessionPost = MapToOPATestSession();

            //ACT
            sessionBuilder.MapGlobalDataEntityToOpa(TestGlobalEntity(), sessionPost, sessionPost.GetGlobalEntityInstance());
            var learnerPre = EntityList(sessionPre);
            var learnerPost = EntityList(sessionPost);

            var learnerPrelist = EntityInstanceList(sessionPre, learnerPre);
            var learnerPostlist = EntityInstanceList(sessionPost, learnerPost);

            //ASSERT
            sessionPost.Should().NotBe(sessionPre);
            learnerPrelist.Should().BeNullOrEmpty();
            learnerPostlist.Count.Should().Be(1);

        }

        /// <summary>
        /// Return OPA Session and check if the entity is global or has children
        /// </summary>
        [Fact(DisplayName = "MapToOPA - Map Entity - Entity is Global"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_MapDataEntityToOpa_EntityIsGlobal()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();
            var session = MapToOPATestSession();

            //ACT
            var instance = sessionBuilder.MapDataEntityToOpa(new DataEntity("global"), session, session.GetGlobalEntityInstance());

            //ASSERT
            instance.Should().NotBeNull();
            instance.GetEntity().IsGlobal().Should().BeTrue();
        }

        /// <summary>
        /// Return OPA Session and check if the entity is global or has children
        /// </summary>
        [Fact(DisplayName = "MapToOPA - Map Entity - Entity is not Global"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_MapDataEntityToOpa_EntityIsNotGlobal()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();
            var session = MapToOPATestSession();

            //ACT
            var instance = sessionBuilder.MapDataEntityToOpa(new DataEntity("Learner"), session, session.GetGlobalEntityInstance());

            //ASSERT
            instance.Should().NotBeNull();
            instance.GetEntity().IsGlobal().Should().BeFalse();
        }

        /// <summary>
        /// Return OPA Session and check if it has changed
        /// </summary>
        [Fact(DisplayName = "MapToOPA - Child Entity updated"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_MapDataEntityToOpa_ChildEntityUpdated()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();
            var sessionPre = MapToOPATestSession();
            var sessionPost = MapToOPATestSession();

            //ACT
            sessionBuilder.MapDataEntityToOpa(TestGlobalEntity(), sessionPost, sessionPost.GetGlobalEntityInstance());
            var learnerPre = EntityList(sessionPre);
            var learnerPost = EntityList(sessionPost);

            var learnerPrelist = EntityInstanceList(sessionPre, learnerPre);
            var learnerPostlist = EntityInstanceList(sessionPost, learnerPost);

            //ASSERT
            sessionPost.Should().NotBe(sessionPre);
            learnerPrelist.Should().BeNullOrEmpty();
            learnerPostlist.Count.Should().Be(1);
        }

        #endregion

        #region SetAttribute Tests    

        /// <summary>
        /// Return OPA Session and check if the attributes have mapped as expected
        /// </summary>
        [Fact(DisplayName = "MapToOPA - Set Attribute - Attribute Exists"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_SetAttribute_AttributeExists()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();
            var session = MapToOPATestSession();
            var instance = session.GetGlobalEntityInstance();
            var entity = instance.GetEntity();

            //ACT
            sessionBuilder.SetAttribute(entity, instance, TestAttributeData);

            //ASSERT
            var ukprn = AttributeValue(session, "UKPRN");

            ukprn.Should().NotBeNull();
        }

        /// <summary>
        /// Return OPA Session and check if the attributes have mapped as expected
        /// </summary>
        [Fact(DisplayName = "MapToOPA - Set Attribute - Attribute Correct"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_SetAttribute_AttributeCorrect()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();
            var session = MapToOPATestSession();
            var instance = session.GetGlobalEntityInstance();
            var entity = instance.GetEntity();

            //ACT
            sessionBuilder.SetAttribute(entity, instance, TestAttributeData);

            //ASSERT
            var ukprn = AttributeValue(session, "UKPRN");

            ukprn.Should().BeEquivalentTo(12345678);
        }

        /// <summary>
        /// Return OPA Session and check if the attributes have mapped as expected
        /// </summary>
        [Fact(DisplayName = "MapToOPA - Set Attribute - No attributes or changepoints set"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_SetAttribute_AttributeAndChangePointValuesNull()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();
            var session = MapToOPATestSession();
            var instance = session.GetGlobalEntityInstance();
            var entity = instance.GetEntity();
            var attributeData = new AttributeData("UKPRN", null);

            //ACT
            sessionBuilder.SetAttribute(entity, instance, attributeData);

            //ASSERT         
            var ukprn = AttributeValue(session, "UKPRN");

            ukprn.Should().BeNull();
        }

        /// <summary>
        /// Return OPA Session and check if the attributes have mapped as expected
        /// </summary>
        [Fact(DisplayName = "MapToOPA - Set Attribute - Attribute and changepoints set"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_SetAttribute_ChangePointValuesExist()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();
            var session = MapToOPATestSession();
            var instance = session.GetGlobalEntityInstance();
            var entity = instance.GetEntity();

            var attributeData = TestAttributeData;
            attributeData.AddChangepoints(TestChangePoints);

            //ACT
            sessionBuilder.SetAttribute(entity, instance, attributeData);

            //ASSERT         
            var ukprnChangePoint = entity.GetAttribute("UKPRN").GetValue(instance);

            ukprnChangePoint.Should().NotBeNull();
            ukprnChangePoint.ToString().Should().BeEquivalentTo("{unknown, 100.0 from 2017-08-01, 100.0 from 2017-09-01}");
        }

        #endregion

        #region MapTemporalValue Tests

        /// <summary>
        /// Return OPA Instance
        /// </summary>
        [Fact(DisplayName = "MapToOPA - MapTemporal exists"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_MapTemporal_Exists()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();

            var attributeData = TestAttributeData;
            attributeData.AddChangepoints(TestChangePoints);

            //ACT
            var temporal = sessionBuilder.MapTemporalValue(attributeData.Changepoints);

            //ASSERT         
            temporal.Should().NotBeNull();
        }

        /// <summary>
        /// Return OPA Instance
        /// </summary>
        [Fact(DisplayName = "MapToOPA - MapTemporal Count"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_MapTemporal_Count()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();

            var attributeData = TestAttributeData;
            attributeData.AddChangepoints(TestChangePoints);

            //ACT
            var temporal = sessionBuilder.MapTemporalValue(attributeData.Changepoints);

            //ASSERT         
            temporal.Count.Should().Be(2);
        }

        /// <summary>
        /// Return OPA Instance
        /// </summary>
        [Fact(DisplayName = "MapToOPA - MapTemporal values correct"), Trait("OPA Session Builder", "Unit")]
        public void SessionBuilder_MapTemporal_Correct()
        {
            //ARRANGE
            var sessionBuilder = new SessionBuilder();

            var attributeData = TestAttributeData;
            attributeData.AddChangepoints(TestChangePoints);

            //ACT
            var temporal = sessionBuilder.MapTemporalValue(attributeData.Changepoints);

            //ASSERT         
            temporal.Should().NotBeNull();
            temporal[0].ToString().Should().BeEquivalentTo("100.0@2017-08-01");
            temporal[1].ToString().Should().BeEquivalentTo("100.0@2017-09-01");
        }

        #endregion

        #region Test Helpers

        #region Test Data

        private IDataEntity TestGlobalEntity()
        {
            IDataEntity globalEntity = new DataEntity("global")
            {
                Attributes = new Dictionary<string, IAttributeData>()
                {
                    {"UKPRN", new AttributeData("UKPRN", 12345678)}
                }
            };

            globalEntity.AddChild(
                new DataEntity("Learner")
                {
                    Attributes = new Dictionary<string, IAttributeData>()
                    {
                        {"LearnRefNumber", new AttributeData("LearnRefNumber", "Learner1")}
                    }
                });

            return globalEntity;
        }

        private readonly IAttributeData TestAttributeData = new AttributeData("UKPRN", 12345678);

        private readonly IEnumerable<ITemporalValueItem> TestChangePoints =
            new List<ITemporalValueItem>()
            {
                new TemporalValueItem(DateTime.Parse("2017-08-01"), 100, "currency"),
                new TemporalValueItem(DateTime.Parse("2017-09-01"), 100, "currency")
            };

        #endregion

        #region Create Test Sessions

        private const string RulebaseZipPath = @"ESFA.DC.OPA.Service.Tests.Rulebase.Loans Bursary 17_18.zip";

        private Session CreateSessionTestSession(ISessionBuilder sessionBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Session session;
            using (Stream stream = assembly.GetManifestResourceStream(RulebaseZipPath))
            {
                session = sessionBuilder.CreateOPASession(stream, TestGlobalEntity());
            }

            return session;
        }

        private Session MapToOPATestSession()
        {
            Engine testEngine = Engine.INSTANCE;
            var assembly = Assembly.GetExecutingAssembly();
            Session session;
            using (Stream stream = assembly.GetManifestResourceStream(RulebaseZipPath))
            {
                Rulebase testRulebase = testEngine.GetRulebase(new InputStreamAdapter(stream));
                session = testEngine.CreateSession(testRulebase);
            }

            return session;
        }

        #endregion

        #region Get Session Values

        private object AttributeValue(Session session, string atttributeName)
        {
            var obj = session.GetGlobalEntityInstance().GetEntity()
                .GetAttribute(atttributeName).GetValue(session.GetGlobalEntityInstance());

            return obj;
        }

        private List EntityList(Session session)
        {
            var entities = session.GetGlobalEntityInstance().GetEntity().GetChildEntities();

            return entities;
        }

        private List<EntityInstance> EntityInstanceList(Session session, List entityList)
        {
            List<EntityInstance> entityInstanceList = new List<EntityInstance>();

            foreach (Entity childEntity in entityList)
            {
                var instance = session.GetGlobalEntityInstance().GetChildren(childEntity);
                foreach (EntityInstance entityInstance in instance)
                {
                    entityInstanceList.Add(entityInstance);
                }
            }

            return entityInstanceList;
        }

        #endregion

        #endregion
    }
}
