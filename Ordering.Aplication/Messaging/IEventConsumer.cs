using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Aplication.Messaging
{
    public interface IEventConsumer
    {
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
