using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects.NET;

namespace SoftwareMind.Shared.Helper
{
    public static class SessionHelper
    {
        public static void Exec(this Session session, TransactionMode mode, Action<Session> method)
        {
            try
            {
                session.ExecTransactionally(mode, () => method(session));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static T Exec<T>(this Session session, TransactionMode mode, Func<Session, T> method)
        {
            try
            {
                T result = default(T);
                session.ExecTransactionally(mode, () => result = method(session));

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Exec(this Session session, TransactionMode mode, Action method)
        {
            try
            {
                session.ExecTransactionally(mode, () => method());
            }
            catch(Exception)
            {
                throw;
            }
        }

        public static T Exec<T>(this Session session, TransactionMode mode, Func<T> method)
        {
            try
            {
                T result = default(T);
                session.ExecTransactionally(mode, () => result = method());

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
