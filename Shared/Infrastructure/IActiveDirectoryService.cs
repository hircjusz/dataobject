using System.Collections.Generic;
using System.Collections.Specialized;
using SoftwareMind.Shared.Service;

namespace SoftwareMind.Shared.Infrastructure
{
    public interface IActiveDirectoryData
    {
        long ID { get; set; }

        string AccountName { get; set; }
        string AdUserName { get; set; }
        string Department { get; set; }
        string DomainName { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string UserName { get; set; }
        string UserPrincipalName { get; set; }

        StringCollection UserProfileCodes { get; set; }
    }

    public interface IActiveDirectoryService : IAnonymousService
    {
        IActiveDirectoryData GetUserData(string userName, string domainName, bool autoCreate = false);
        IEnumerable<IActiveDirectoryData> GetGroupData(string groupName, string domainName);
    }

    public interface IActiveDirectoryServiceResolver
    {
        IActiveDirectoryService ResolveService();
    }
}
