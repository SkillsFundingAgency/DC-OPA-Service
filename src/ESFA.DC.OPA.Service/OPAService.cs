using System.IO;
using System.Reflection;
using ESFA.DC.OPA.Model.Interface;
using ESFA.DC.OPA.Service.Interface;
using ESFA.DC.OPA.Service.Interface.Builders;
using ESFA.DC.OPA.Service.Interface.Rulebase;
using Oracle.Determinations.Engine;

namespace ESFA.DC.OPA.Service
{
    public class OPAService : IOPAService
    {
        private readonly ISessionBuilder _sessionBuilder;
        private readonly IOPADataEntityBuilder _dataEntityBuilder;
        private readonly IRulebaseProviderFactory _rulebaseProviderFactory;

        public OPAService(ISessionBuilder sessionBuilder, IOPADataEntityBuilder dataEntityBuilder, IRulebaseProviderFactory rulebaseProviderFactory)
        {
            _sessionBuilder = sessionBuilder;
            _dataEntityBuilder = dataEntityBuilder;
            _rulebaseProviderFactory = rulebaseProviderFactory;
        }

        public IDataEntity ExecuteSession(IDataEntity globalEntity)
        {
            var rulebaseProvider = _rulebaseProviderFactory.Build();

            Session session;

            using (Stream stream = rulebaseProvider.GetStream(Assembly.GetCallingAssembly()))
            {
                session = _sessionBuilder.CreateOPASession(stream, globalEntity);
            }

            session.Think();

            var outputGlobalInstance = session.GetGlobalEntityInstance();
            var outputEntity = _dataEntityBuilder.CreateOPADataEntity(outputGlobalInstance, null);

            return outputEntity;
        }
    }
}
