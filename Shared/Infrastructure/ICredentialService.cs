using System;

namespace SoftwareMind.Shared.Infrastructure
{
    [Serializable]
    public class Credentials
    {
        public string Identity { get; set; }
    }

    public interface ICredentialService
    {
        Credentials GetCredentials();
    }

    public interface ISimulatedCredentialService : ICredentialService
    {
        void SetCredentials(string credentials);
    }

    public interface ICredentialServiceResolver
    {
        ICredentialService ResolveService();
    }
}
