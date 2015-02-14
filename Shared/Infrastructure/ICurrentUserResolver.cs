using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SoftwareMind.Shared.Infrastructure
{
    public interface ICurrentUserResolver
    {
        ICurrentUser Resolve(Func<IUserEntry> entry);
    }
}
