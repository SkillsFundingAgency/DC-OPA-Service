using ESFA.DC.OPA.Model.Interface;

namespace ESFA.DC.OPA.Service.Interface
{
    public interface IOPAService
    {
        IDataEntity ExecuteSession(IDataEntity globalEntity);
    }
}
