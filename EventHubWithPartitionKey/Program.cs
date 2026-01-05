using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        List<Device> devices = new List<Device>() {
           new Device() {deviceId = "D1", Temperature = 40.4f },
           new Device(){deviceId = "D1",Temperature = 50f },
       };
        await sendEvents(devices); // Call the function to resolve CS8321
       // await ReadEvents();

        static async Task sendEvents(List<Device> devices)
        {
            string connectionstring = "";
            EventHubProducerClient producerClient = new EventHubProducerClient(connectionstring);
            List<EventData> eventDatas = new List<EventData>();
                foreach (var device in devices)
                {
                    string eventBody = System.Text.Json.JsonSerializer.Serialize(device);
                    EventData eventData = new EventData(Encoding.UTF8.GetBytes(eventBody));
                    eventDatas.Add(eventData);
                }
                await producerClient.SendAsync(eventDatas, new SendEventOptions() { PartitionKey = "D1" });
                Console.WriteLine("A batch of events has been published.");
        }
    }
}

// Add this class definition above or below the Program class to resolve CS0246
internal class Device
{
    public string deviceId { get; set; }
    public float Temperature { get; set; }
}