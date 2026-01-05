using EventHub;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;
using System.Text;
using Azure.Messaging.EventHubs.Consumer;
 // Add this using directive at the top of the file

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        List<Device> devices = new List<Device>() {
           new Device() {deviceId = "01", Temperature = 40.4f },
           new Device(){deviceId = "02",Temperature = 50f },
           new Device() {deviceId = "03", Temperature = 60.4f },
           new Device(){deviceId = "04",Temperature = 70f },
       };
        //await sendEvents(devices);
        await ReadEvents();

        async Task sendEvents(List<Device> devices)
        {
            EventHubProducerClient producerClient = new EventHubProducerClient(connectionstring);

            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
            {
                foreach (var device in devices)
                {
                    string eventBody = System.Text.Json.JsonSerializer.Serialize(device);
                    EventData eventData = new EventData(Encoding.UTF8.GetBytes(eventBody));
                    eventBatch.TryAdd(eventData);
                }
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("A batch of events has been published.");
            }
        }

        async Task ReadEvents()
        {
            //  string connectionstring = "";
            string connectionstring = "";
            string consumerGroup = "$Default";
            EventHubConsumerClient eventHubConsumerClient = new EventHubConsumerClient(consumerGroup, connectionstring);
            var cancellationtoken = new CancellationTokenSource();
            cancellationtoken.CancelAfter(TimeSpan.FromSeconds(300));

            string partitionId = (await eventHubConsumerClient.GetPartitionIdsAsync()).First();

            await foreach (PartitionEvent partitionEvent in eventHubConsumerClient.ReadEventsAsync(cancellationtoken.Token))
            {
                Console.WriteLine($"Participation Id {partitionEvent.Partition.PatitionId}");
                Console.WriteLine($"Participation Data {partitionEvent.Data.Offset}");
                Console.WriteLine($"Participation Key {partitionEvent.Data.PartitionKey}");
                Console.WriteLine($"Participation Event Body {partitionEvent.Data.EventBody}");
            }

            //await foreach (PartitionEvent partitionEvent in eventHubConsumerClient.ReadEventsFromPartitionAsync(partitionId,EventPosition.Earliest))
            //{
            //    Console.WriteLine($"Participation Id {partitionEvent.Partition.PartitionId}");
            //    Console.WriteLine($"Participation Data {partitionEvent.Data.Offset}");
            //    Console.WriteLine($"Participation Sequence Number {partitionEvent.Data.SequenceNumber}");
            //    Console.WriteLine($"Participation Event Body {partitionEvent.Data.EventBody}");
            //}
        }
    }
}