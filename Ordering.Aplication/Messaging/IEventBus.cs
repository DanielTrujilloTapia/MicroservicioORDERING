using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Aplication.Messaging
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event) where T: class;
    }
}
