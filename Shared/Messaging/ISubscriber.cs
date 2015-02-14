using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.Utils.Messaging
{
    public interface ISubscriber<T>
    {
        void Handle(T eventMessage);
    }

}
