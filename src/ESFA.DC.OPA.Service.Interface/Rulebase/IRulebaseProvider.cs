using System;
using System.IO;
using System.Reflection;

namespace ESFA.DC.OPA.Service.Interface.Rulebase
{
    public interface IRulebaseProvider
    {
        Stream GetStream(Assembly assembly);
    }
}
