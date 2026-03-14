using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

// Simple utility to peek or receive messages from the emulator queue.
// Usage: dotnet run [peek|receive] [maxCount]

var argsList = args.Length == 0 ? new[] {"peek", "10"} : args;
string mode = argsList[0].ToLower();
int max = argsList.Length > 1 && int.TryParse(argsList[1], out var parsed) ? parsed : 10;

string cs = "Endpoint=sb://localhost:5672/;SharedAccessKeyName=RootManageSharedAccessKey;" +
            "SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";

await using var client = new ServiceBusClient(cs);
var receiver = client.CreateReceiver("test-queue");

if (mode == "peek")
{
    var messages = await receiver.PeekMessagesAsync(max);
    foreach (var msg in messages)
    {
        Console.WriteLine($"[{msg.SequenceNumber}] {msg.Body}");
    }
}
else if (mode == "receive")
{
    Console.WriteLine("Receiving and completing messages...");
    var received = await receiver.ReceiveMessagesAsync(max);
    foreach (var msg in received)
    {
        Console.WriteLine($"[{msg.SequenceNumber}] {msg.Body}");
        await receiver.CompleteMessageAsync(msg);
    }
}
else
{
    Console.WriteLine("Unknown mode. Use peek or receive.");
}
