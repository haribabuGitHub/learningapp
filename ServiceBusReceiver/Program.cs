using Azure.Messaging.ServiceBus;

namespace NServiceBusReceiver
{
    public class Orders
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    internal static class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("Hello, World!");

            List<Orders> orders = new List<Orders>
            {
                new Orders { OrderId = 1, ProductName = "Laptop", Quantity = 2 },
                new Orders { OrderId = 2, ProductName = "Smartphone", Quantity = 5 },
                new Orders { OrderId = 3, ProductName = "Tablet", Quantity = 3 }
            };

            string connectionString = "";
            string queueName = "appqueue1";

            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(queueName);

            try
            {
                int i = 3;
                foreach (var order in orders)
                {
                    ServiceBusMessage orderMessage = new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(order));
                    orderMessage.ApplicationProperties.Add("OrderId", order.OrderId);
                    orderMessage.MessageId = i.ToString();
                    i++;
                    await sender.SendMessageAsync(orderMessage);
                    Console.WriteLine($"Order message for OrderId {order.OrderId} sent.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }

            // Peek messages (example: peek up to 10 messages)
          //  await PeekMessagesAsync(client, queueName, maxMessages: 10);
        }

        public static async Task PeekMessagesAsync(ServiceBusClient client, string queueName, int maxMessages = 10)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentException("Queue name is required", nameof(queueName));
            if (maxMessages <= 0) maxMessages = 1;

            await using ServiceBusReceiver receiver = client.CreateReceiver(queueName);
            try
            {
                IReadOnlyList<ServiceBusReceivedMessage> messages = await receiver.PeekMessagesAsync(maxMessages);
                if (messages == null || messages.Count == 0)
                {
                    Console.WriteLine("No messages found when peeking.");
                    return;
                }

                Console.WriteLine($"Peeked {messages.Count} message(s):");
                foreach (var msg in messages)
                {
                    Console.WriteLine($"- SequenceNumber: {msg.SequenceNumber}, MessageId: {msg.MessageId}");
                    Console.WriteLine($"  Body: {msg.Body.ToString()}");
                    if (msg.ApplicationProperties != null && msg.ApplicationProperties.Count > 0)
                    {
                        Console.WriteLine("  ApplicationProperties:");
                        foreach (var kvp in msg.ApplicationProperties)
                        {
                            Console.WriteLine($"    {kvp.Key}: {kvp.Value}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error peeking messages: {ex.Message}");
            }
        }
    }
}
