using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.Shared.Mef;
using log4net;
using System.ComponentModel.Composition;

namespace SoftwareMind.Utils.Messaging
{
    [Export(typeof(IEventPublisher))]
    public class EventPublisher : IEventPublisher
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EventPublisher));

        public void Publish<T>(T message)
        {
            var subscriptions = ServiceLocator.Current.GetAllInstances<ISubscriber<T>>();
            subscriptions.ToList().ForEach(consumer => PublishToSubscriber(consumer, message));
        }

        private static void PublishToSubscriber<T>(ISubscriber<T> x, T eventMessage)
        {
            try
            {
                x.Handle(eventMessage);
            }
            catch (Exception)
            {
                log.ErrorFormat("Wystąpił błąd podczas publikowania wiadomości eventMessage: {0}", eventMessage);
            }

            finally
            {
                var instance = x as IDisposable;

                if (instance != null)
                {
                    instance.Dispose();
                }
            }
        }
    }

}
