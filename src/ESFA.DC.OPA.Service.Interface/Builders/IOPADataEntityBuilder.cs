using ESFA.DC.OPA.Model.Interface;
using Oracle.Determinations.Engine;

namespace ESFA.DC.OPA.Service.Interface.Builders
{
    public interface IOPADataEntityBuilder
    {
        IDataEntity CreateOPADataEntity(EntityInstance entityInstance, IDataEntity parentEntity);
    }
}
