using System.Collections.Generic;
using SoftwareMind.Shared.Dto;

namespace SoftwareMind.Shared.Infrastructure
{
    public interface IStatementInvoker
    {
        void SetAction(IAccessStoreActionDTO action);

        IContainer Invoke(IStatement statement);
        IContainer Invoke(string query, params KeyValuePair<string, object>[] parameters);
    }
}