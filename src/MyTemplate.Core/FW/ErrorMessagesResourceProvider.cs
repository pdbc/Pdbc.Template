using System.Resources;
using Aertssen.Framework.Infra.Services;
using MyTemplate.I18N;

namespace MyTemplate.Core.FW
{
    public class ErrorMessagesResourceProvider : IErrorMessagesResourceProvider
    {
        public ResourceManager GetResourceManagerForErrorMessages()
        {
            return Errors.ResourceManager;
        }
    }
}
