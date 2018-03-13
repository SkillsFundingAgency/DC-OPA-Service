﻿using System.IO;
using System.Reflection;
using ESFA.DC.OPA.Model.Interface;
using ESFA.DC.OPA.Service.Interface;
using ESFA.DC.OPA.Service.Interface.Builders;
using Oracle.Determinations.Engine;

namespace ESFA.DC.OPA.Service
{
    public class OPAService : IOPAService
    {
        private readonly ISessionBuilder _sessionBuilder;
        private readonly IOPADataEntityBuilder _dataEntityBuilder;
        private readonly string _rulebaseZipPath;

        public OPAService(ISessionBuilder sessionBuilder, IOPADataEntityBuilder dataEntityBuilder, string rulebaseZipPath)
        {
            _sessionBuilder = sessionBuilder;
            _dataEntityBuilder = dataEntityBuilder;
            _rulebaseZipPath = rulebaseZipPath;
        }

        public IDataEntity ExecuteSession(IDataEntity globalEntity)
        {
            var assembly = Assembly.GetCallingAssembly();

            var rulebaseLocation = assembly.GetName().Name + _rulebaseZipPath;

            Session session;

            using (Stream stream = assembly.GetManifestResourceStream(rulebaseLocation))
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
