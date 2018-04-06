using System;
using ESFA.DC.OPA.Model;
using ESFA.DC.OPA.Model.Interface;
using ESFA.DC.OPA.Service.Interface.Builders;
using Oracle.Determinations.Engine;
using Oracle.Determinations.Engine.Local.Temporal;
using Oracle.Determinations.Masquerade.Util;

namespace ESFA.DC.OPA.Service.Builders
{
    public class OPADataEntityBuilder : IOPADataEntityBuilder
    {
        private readonly DateTime _yearStartDate;

        public OPADataEntityBuilder(DateTime yearStartDate)
        {
            _yearStartDate = yearStartDate;
        }

        public IDataEntity CreateOPADataEntity(EntityInstance entityInstance, IDataEntity parentEntity)
        {
            var globalEntity = MapOpaToEntity(entityInstance, parentEntity);

            return globalEntity;
        }

        #region Map OPA Session to Data Entity

        protected internal IDataEntity MapOpaToEntity(EntityInstance instance, IDataEntity parentEntity)
        {
            IDataEntity dataEntity = new DataEntity(instance.GetEntity().GetName())
            {
                Parent = parentEntity
            };

            MapAttributes(instance, dataEntity);

            var childEntities = instance.GetEntity().GetChildEntities();

            MapEntities(instance, childEntities, dataEntity);
            if (parentEntity == null)
            {
                parentEntity = dataEntity;
            }
            else
            {
                parentEntity.Children.Add(dataEntity);
            }

            return parentEntity;
        }

        protected internal void MapAttributes(EntityInstance instance, IDataEntity dataEntity)
        {
            foreach (RBAttr attribute in instance.GetEntity().GetAttributes())
            {
                var attributeData = MapOpaAttributeToDataEntity(instance, attribute);

                if (attributeData != null)
                {
                    dataEntity.Attributes.Add(attributeData.Name, attributeData);
                }
            }
        }

        protected internal IAttributeData MapOpaAttributeToDataEntity(EntityInstance entityInstance, RBAttr attr)
        {
            object value = attr.GetValue(entityInstance);
            if (value is TemporalValue)
            {
                IAttributeData attributeData = new AttributeData(attr.GetName(), null);
                var temporalValue = value as TemporalValue;
                for (int period = 0; period < 12; period++)
                {
                    var date = _yearStartDate.AddMonths(period);
                    var index = temporalValue.FindChangePointIndex(new ChangePointDate(date.Year, date.Month, date.Day));
                    var val = temporalValue.GetValue(index);
                    attributeData.Changepoints.Add(new TemporalValueItem(date, val, string.Empty));
                }

                return attributeData;
            }

            return new AttributeData(attr.GetName(), value is string ? value.ToString().Trim() : value);
        }

        protected internal void MapEntities(EntityInstance instance, List childEntities, IDataEntity dataEntity)
        {
            foreach (Entity childEntity in childEntities)
            {
                var childInstances = instance.GetChildren(childEntity);
                foreach (EntityInstance childInstance in childInstances)
                {
                    MapOpaToEntity(childInstance, dataEntity);
                }
            }
        }

        #endregion

    }
}
