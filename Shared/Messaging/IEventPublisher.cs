using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.Utils.Messaging
{
    public interface IEventPublisher
    {
        void Publish<T>(T message);
    }

}
