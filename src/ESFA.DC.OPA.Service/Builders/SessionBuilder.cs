using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ESFA.DC.OPA.Model.Interface;
using ESFA.DC.OPA.Service.Interface.Builders;
using Oracle.Determinations.Engine;
using Oracle.Determinations.Engine.Local.Temporal;
using Oracle.Determinations.Masquerade.IO;
using Oracle.Determinations.Masquerade.Util;

namespace ESFA.DC.OPA.Service.Builders
{
    public class SessionBuilder : ISessionBuilder
    {
        private readonly Engine _engine = Engine.INSTANCE;

        internal bool RulebaseInitialised { get; set; }

        internal Oracle.Determinations.Engine.Rulebase Rulebase { get; set; }

        internal Engine Engine => _engine;

        public Session CreateOPASession(Stream rulebaseStream, IDataEntity globalEntity)
        {
            if (!RulebaseInitialised)
            {
                InputStream stream = new InputStreamAdapter(rulebaseStream);
                Rulebase = Engine.GetRulebase(stream);
                RulebaseInitialised = true;
            }

            Session session = Engine.CreateSession(Rulebase);

            var inputGlobalInstance = session.GetGlobalEntityInstance();

            MapGlobalDataEntityToOpa(globalEntity, session, inputGlobalInstance);

            return session;
        }

        #region MapEntityToOpaSession

        protected internal void MapGlobalDataEntityToOpa(IDataEntity dataEntity, Session session, EntityInstance parentEntityInstance)
        {
            Entity globalEntity = parentEntityInstance.GetEntity();
            foreach (var attribute in dataEntity.Attributes)
            {
                SetAttribute(globalEntity, parentEntityInstance, attribute.Value);
            }

            foreach (var childDataEntity in dataEntity.Children)
            {
                var childInstance = this.MapDataEntityToOpa(childDataEntity, session, parentEntityInstance);
                if (!childDataEntity.IsGlobal)
                {
                    parentEntityInstance.MarkContainmentComplete(true, childInstance.GetEntity());
                }
            }
        }

        protected internal EntityInstance MapDataEntityToOpa(IDataEntity dataEntity, Session session, EntityInstance parentEntityInstance)
        {
            Entity entity = null;
            EntityInstance targetInstance = null;

            if (dataEntity.IsGlobal)
            {
                entity = parentEntityInstance.GetEntity();
                targetInstance = parentEntityInstance;
            }
            else
            {
                entity = session.GetRulebase().GetEntity(dataEntity.EntityName);
                targetInstance = session.CreateEntityInstance(entity, parentEntityInstance);
            }

            foreach (var attribute in dataEntity.Attributes)
            {
                SetAttribute(entity, targetInstance, attribute.Value);
            }

            var opaChildEntities = entity.GetChildEntities();

            foreach (Entity opaChildEntity in opaChildEntities)
            {
                if (!dataEntity.Children.Select(de => de.EntityName).Contains(opaChildEntity.GetName()))
                {
                    targetInstance.MarkContainmentComplete(true, opaChildEntity);
                }
            }

            foreach (var childDataEntity in dataEntity.Children)
            {
                var childInstance = this.MapDataEntityToOpa(childDataEntity, session, targetInstance);
                if (!dataEntity.IsGlobal)
                {
                    targetInstance.MarkContainmentComplete(true, childInstance.GetEntity());
                }
            }

            return targetInstance;
        }

        protected internal void SetAttribute(Entity entity, EntityInstance targetInstance, IAttributeData attributeData)
        {
            if (attributeData.Value == null && attributeData.Changepoints.Count == 0)
            {
                return;
            }

            RBAttr attribute = entity.GetAttribute(attributeData.Name);
            if (attribute != null)
            {
                if (attributeData.Changepoints.Count > 0)
                {
                    var changePoints = this.MapTemporalValue(attributeData.Changepoints);
                    attribute.SetValue(targetInstance, new TemporalValue(null, changePoints));
                }
                else if (attribute.GetValueType() == 16)
                {
                    attribute.SetValue(targetInstance, new Date(DateTime.Parse(attributeData.Value.ToString())));
                }
                else if (attribute.GetValueType() == 4)
                {
                    attribute.SetValue(targetInstance, new Oracle.Determinations.Masquerade.Lang.Double(attributeData.Value.ToString()));
                }
                else if (attribute.GetValueType() == 8)
                {
                    attribute.SetValue(targetInstance, new Oracle.Determinations.Masquerade.Lang.Double(attributeData.Value.ToString()));
                }
                else if (attribute.GetValueType() == 2)
                {
                    attribute.SetValue(targetInstance, attributeData.Value.ToString().Trim());
                }
                else
                {
                    attribute.SetValue(targetInstance, bool.Parse(attributeData.Value.ToString()));
                }
            }
            else
            {
                // TODO: Log something
            }
        }

        protected internal ArrayList MapTemporalValue(IList<ITemporalValueItem> valueList)
        {
            ArrayList changepoints = new ArrayList();
            foreach (var temporalValueItem in valueList)
            {
                ChangePointDate changePointDate = new ChangePointDate(
                    temporalValueItem.ChangePoint.Year, temporalValueItem.ChangePoint.Month, temporalValueItem.ChangePoint.Day);

                ChangePoint changePoint = null;

                if (temporalValueItem.Type == "currency")
                {
                    changePoint = string.IsNullOrEmpty(temporalValueItem.Value.ToString()) ?
                        new ChangePoint(changePointDate, null) :
                        new ChangePoint(changePointDate, new Oracle.Determinations.Masquerade.Lang.Double(temporalValueItem.Value.ToString()));
                }

                if (temporalValueItem.Type == "text")
                {
                    changePoint = string.IsNullOrEmpty(temporalValueItem.Value.ToString()) ?
                        new ChangePoint(changePointDate, null) :
                        new ChangePoint(changePointDate, temporalValueItem.Value.ToString());
                }

                if (changePoint != null)
                {
                    changepoints.Add(changePoint);
                }
            }

            return changepoints;
        }

        #endregion

    }
}
