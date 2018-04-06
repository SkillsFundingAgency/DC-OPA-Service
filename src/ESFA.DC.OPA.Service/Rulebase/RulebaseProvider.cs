using System.IO;
using System.Reflection;
using ESFA.DC.OPA.Service.Interface.Rulebase;

namespace ESFA.DC.OPA.Service.Rulebase
{
    public class RulebaseProvider : IRulebaseProvider
    {
        private readonly string _rulebaseZipPath;

        public RulebaseProvider(string rulebaseZipPath)
        {
            _rulebaseZipPath = rulebaseZipPath;
        }

        public Stream GetStream(Assembly assembly)
        {
            return assembly.GetManifestResourceStream(_rulebaseZipPath);
        }
    }
}
