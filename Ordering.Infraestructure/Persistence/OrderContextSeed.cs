using Microsoft.Extensions.Logging;
using Ordering.Domain.Common.Entities;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfigureOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("Se inicializa la base de datos con el contexto {DbContextName}", typeof(OrderContext).Name);
            }
        }

        private static IEnumerable<Order> GetPreconfigureOrders()
        {
            return new List<Order>
            {
                new Order(){ UserName= "swn", FirstName="Erik",
                LastName="Guerrero",EmailAddress="erik.guerrero@uttt.edu.mx",
                AddressLine="Hidalgo", Country="Mexico", TotalPrice=450}
            };
        }
    }
}
