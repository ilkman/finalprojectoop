using System.IO.Pipes;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Master running searching for agent A");

        using NamedPipeServerStream pipeServer = new("agent1pipe", PipeDirection.In);
        pipeServer.WaitForConnection();

        using StreamReader reader = new(pipeServer);
        string json = reader.ReadLine();

        if (json == null)
        {
            Console.WriteLine("No data");
            return;
        }

        var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(json);

        if (data == null)
        {
            Console.WriteLine("Deserialization error");
            return;
        }

        foreach (var fileEntry in data)
        {
            foreach (var wordEntry in fileEntry.Value)
            {
                Console.WriteLine($"{fileEntry.Key}:{wordEntry.Key}:{wordEntry.Value}");
            }
        }

        Console.WriteLine("Done receiveing data");
        Console.ReadKey();
    }
}
