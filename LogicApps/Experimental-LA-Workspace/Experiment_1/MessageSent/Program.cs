using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

var cs = "Endpoint=sb://localhost:5672/;SharedAccessKeyName=RootManageSharedAccessKey;"
       + "SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";

await using var client = new ServiceBusClient(cs);
await using var sender = client.CreateSender("test-queue");
await sender.SendMessageAsync(new ServiceBusMessage("hello from local!"));
Console.WriteLine("sent");
