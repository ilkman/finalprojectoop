using System.IO.Pipes;
using System.Text.Json;
using System.Collections.Concurrent;

class Program
{
    static ConcurrentDictionary<string, Dictionary<string, int>> finalData = new();

    static void Main(string[] args)
    {
        Console.WriteLine("Master running searching for agents...");

        Thread agentAThread = new(() => ReceiveFromAgent("agent1pipe"));
        Thread agentBThread = new(() => ReceiveFromAgent("agent2pipe"));

        agentAThread.Start();
        agentBThread.Start();

        agentAThread.Join();
        agentBThread.Join();

     
        foreach (var fileEntry in finalData)
        {
            foreach (var wordEntry in fileEntry.Value)
            {
                Console.WriteLine($"{fileEntry.Key}:{wordEntry.Key}:{wordEntry.Value}");
            }
        }

        Console.WriteLine("Done receiveing data");
        Console.ReadKey();
    }

    static void ReceiveFromAgent(string pipeName)
    {
        try
        {
            using NamedPipeServerStream pipeServer = new(pipeName, PipeDirection.In);
            Console.WriteLine($"Waiting for {pipeName}");
            pipeServer.WaitForConnection();

            using StreamReader reader = new(pipeServer);
            string json = reader.ReadLine();

            if (json != null)
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(json);

                if (data != null)
                {
                    foreach (var file in data)
                    {
                        finalData[file.Key] = file.Value;
                    }

                    Console.WriteLine($"Received data from {pipeName}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"got Error from {pipeName}: {ex.Message}");
        }
    }
}