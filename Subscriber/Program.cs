﻿using Subscriber.Dtos;
using System.Net.Http.Json;


Console.WriteLine("Press ESC to stop");

do
{
	HttpClient client = new HttpClient();
	Console.WriteLine("Listening...");

	while (!Console.KeyAvailable)
	{
		List<int> ackIds = await GetMessagesAsync(client);

		Thread.Sleep(2000);

		if (ackIds.Count > 0)
		{
			await AckMessagesAsync(client, ackIds);
		}

	}

} while (Console.ReadKey(true).Key != ConsoleKey.Escape);

static async Task<List<int>> GetMessagesAsync(HttpClient httpClient)
{
    List<int> ackIds = new();
    List<MessageReadDto>? newMessages = new();

	try
	{
		newMessages = await httpClient.GetFromJsonAsync<List<MessageReadDto>>("https://localhost:7202/api/subscriptions/2/messages");
	}
	catch (Exception)
	{
		return ackIds;
	}

	foreach(var msg in newMessages)
	{
		Console.WriteLine($"{msg.Id} - {msg.TopicMessage} - {msg.MessageStatus}");
        ackIds.Add(msg.Id);
    }

	return ackIds;
}

static async Task AckMessagesAsync(HttpClient httpClient, List<int> ackIds)
{
	var response = await httpClient.PostAsJsonAsync("https://localhost:7202/api/subscriptions/2/messages", ackIds);
	var returnMessage = await response.Content.ReadAsStringAsync();

	Console.WriteLine(returnMessage);
}