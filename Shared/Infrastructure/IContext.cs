using System;
using DataObjects.NET;

namespace SoftwareMind.Shared.Infrastructure
{
    public interface IContext
    {
        ICurrentUser CurrentUser { get; }
        bool IsLogged { get; }

        Session Session { get; }
        string SessionId { get; }
        IUserEntry UserEntry { get; }

        void ExecWithNewSession(Action<Session> action);
        T ExecWithNewSession<T>(Func<Session, T> action);
    }

    public interface ISystemContext : IDisposable
    {
        Session Session { get; }

        void ExecWithNewSession(Action<Session> action);
        T ExecWithNewSession<T>(Func<Session, T> action);
    }
}
