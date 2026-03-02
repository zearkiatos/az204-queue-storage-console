using System;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Configuration;

class Program
{
    static void Main(string[] args)
    {
        AppConfiguration.ValidateConfiguration();

        string connectionString = AppConfiguration.ConnectionString;
        string queueName = AppConfiguration.QueueName;

        QueueClient queueClient = new QueueClient(connectionString, queueName);

        queueClient.CreateIfNotExists();
        Console.WriteLine($"The queue '{queueName}' has been created or already existed.");

        InsertMessages(queueClient);

        PeekMessages(queueClient);

        ProcessAndDeleteMessages(queueClient);

        UpdateMessage(queueClient);

        GetQueueLength(queueClient);

        DeleteQueue(queueClient);
    }

    static void InsertMessages(QueueClient queueClient)
    {
        Console.WriteLine("\n--- Inserting messages to the queue ---");
        for (int i = 1; i <= 5; i++)
        {
            string message = $"Message {i}";
            queueClient.SendMessage(message);
            Console.WriteLine($"Message inserted: {message}");
        }
    }

    static void PeekMessages(QueueClient queueClient)
    {
        Console.WriteLine("\n--- Peeking messages in the queue ---");
        PeekedMessage[] messages = queueClient.PeekMessages(maxMessages: 3);

        foreach (var message in messages)
        {
            Console.WriteLine($"Message verified: {message.Body}");
        }
    }

    static void ProcessAndDeleteMessages(QueueClient queueClient)
    {
        Console.WriteLine("\n--- Receiving and deleting messages from the queue ---");
        QueueMessage[] messages = queueClient.ReceiveMessages(maxMessages: 3);

        foreach (var message in messages)
        {
            Console.WriteLine($"Message received: {message.Body}");

            queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
            Console.WriteLine($"Message deleted: {message.Body}");
        }
    }

    static void UpdateMessage(QueueClient queueClient)
    {
        Console.WriteLine("\n--- Updating the message in the queue ---");
        QueueMessage[] messages = queueClient.ReceiveMessages(maxMessages: 1);

        if (messages.Length > 0)
        {
            string newContent = "Updated message content";
            queueClient.UpdateMessage(messages[0].MessageId, messages[0].PopReceipt, newContent, TimeSpan.FromSeconds(60));
            Console.WriteLine($"Message updated: {messages[0].Body} to '{newContent}'");
        }
    }

    static void GetQueueLength(QueueClient queueClient)
    {
        Console.WriteLine("\n--- Getting the remaining messages in the queue ---");
        QueueProperties properties = queueClient.GetProperties();
        Console.WriteLine($"Approximately the number of messages remaining are: {properties.ApproximateMessagesCount}");
    }

    static void DeleteQueue(QueueClient queueClient)
    {
        Console.WriteLine("\n--- Deleting the queue ---");
        queueClient.Delete();
        Console.WriteLine("Queue deleted successfully.");
    }
}

