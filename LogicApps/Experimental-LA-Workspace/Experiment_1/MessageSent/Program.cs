using Azure.Messaging.ServiceBus;

// Usage:
// dotnet run --project .\MessageSent -- [queueName] [messageText]
// dotnet run --project .\MessageSent -- [queueName] --file payload.xml
// dotnet run --project .\MessageSent -- [queueName] [messageText] --subject MySubject --message-id MyId
string queueName = "test-queue";
string messageText = $"Test message sent at {DateTime.UtcNow:O}";
string? payloadFile = null;
string subject = "ManualTest";
string messageId = Guid.NewGuid().ToString();
bool hasInlineMessage = false;

if (args.Length > 0)
{
	queueName = args[0];
}

for (int i = 1; i < args.Length; i++)
{
	if (string.Equals(args[i], "--subject", StringComparison.OrdinalIgnoreCase))
	{
		if (i + 1 >= args.Length)
		{
			Console.Error.WriteLine("Missing value after --subject.");
			Environment.Exit(1);
		}

		subject = args[i + 1];
		i++;
		continue;
	}

	if (string.Equals(args[i], "--message-id", StringComparison.OrdinalIgnoreCase))
	{
		if (i + 1 >= args.Length)
		{
			Console.Error.WriteLine("Missing value after --message-id.");
			Environment.Exit(1);
		}

		messageId = args[i + 1];
		i++;
		continue;
	}

	if (string.Equals(args[i], "--file", StringComparison.OrdinalIgnoreCase))
	{
		if (i + 1 >= args.Length)
		{
			Console.Error.WriteLine("Missing file path after --file.");
			Environment.Exit(1);
		}

		payloadFile = args[i + 1];
		i++;
		continue;
	}

	if (!hasInlineMessage)
	{
		messageText = args[i];
		hasInlineMessage = true;
	}
}

if (!string.IsNullOrWhiteSpace(payloadFile))
{
	if (!File.Exists(payloadFile))
	{
		Console.Error.WriteLine($"Payload file not found: {payloadFile}");
		Environment.Exit(1);
	}

	messageText = await File.ReadAllTextAsync(payloadFile);
}

string connectionString =
	"Endpoint=sb://localhost:5672/;SharedAccessKeyName=RootManageSharedAccessKey;" +
	"SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";

await using var client = new ServiceBusClient(connectionString);
ServiceBusSender sender = client.CreateSender(queueName);

var message = new ServiceBusMessage(messageText)
{
	Subject = subject,
	MessageId = messageId
};

await sender.SendMessageAsync(message);
Console.WriteLine($"Sent to queue '{queueName}': {messageText}");
Console.WriteLine($"Subject='{subject}', MessageId='{messageId}'");
