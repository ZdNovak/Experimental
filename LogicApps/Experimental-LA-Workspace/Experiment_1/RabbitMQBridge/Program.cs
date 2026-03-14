using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

class Program
{
    private static readonly HttpClient httpClient = new HttpClient();

    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // Declare the queue
        await channel.QueueDeclareAsync(queue: "test-queue",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");

            // Forward to Logic Apps
            await ForwardToLogicApps(message);
        };

        await channel.BasicConsumeAsync(queue: "test-queue",
                                       autoAck: true,
                                       consumer: consumer);

        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();
    }

    private static async Task ForwardToLogicApps(string message)
    {
        try
        {
            var logicAppsUrl = "http://localhost:7071/runtime/webhooks/workflow/api/management/workflows/wf-transform-BizTalkMsg-to-AzureJson/triggers/manual/run";
            var payload = new { message = message };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(logicAppsUrl, content);
            response.EnsureSuccessStatusCode();

            Console.WriteLine($" [✓] Forwarded to Logic Apps: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" [✗] Error forwarding to Logic Apps: {ex.Message}");
        }
    }
}
