using System.Text.RegularExpressions;
using System.Threading;
using System.IO.Pipes;
using System.Text.Json;

class Program
{
    static Dictionary<string, Dictionary<string, int>> indexedData = new();

    static void Main(string[] args)
    {
        Console.WriteLine("AgentA running");
        Console.Write("Please give the path for the txt files folder: ");
        string directoryPath = Console.ReadLine();

        //checking if the path is correct and exitts
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("Error path is wrong");
            return;
        }

        Thread fileReaderThread = new(() => ReadAndIndexFiles(directoryPath));
        fileReaderThread.Start();
        fileReaderThread.Join(); // rem later

        

        SendDataToMaster("agent1pipe");

        Console.WriteLine("Done reading");
        Console.ReadKey();
    }

    static void ReadAndIndexFiles(string directoryPath)
    {
        string[] files = Directory.GetFiles(directoryPath, "*.txt");

        foreach (var file in files)
        {
            var wordCounts = new Dictionary<string, int>();
            string content = File.ReadAllText(file);
            // extract the words
            var words = Regex.Matches(content.ToLower(), @"\b\w+\b");

            foreach (Match word in words)
            {
                if (wordCounts.ContainsKey(word.Value))
                    wordCounts[word.Value]++;
                else
                    wordCounts[word.Value] = 1;
            }

            indexedData[Path.GetFileName(file)] = wordCounts;
        }
    }


    static void SendDataToMaster(string pipeName)
    {
        try
        {
            using NamedPipeClientStream pipeClient = new(".", pipeName, PipeDirection.Out);
            Console.WriteLine("Master connectiong...");
            pipeClient.Connect(); // master connection

            string json = JsonSerializer.Serialize(indexedData);
            using StreamWriter writer = new(pipeClient);
            writer.AutoFlush = true;
            writer.WriteLine(json);

            Console.WriteLine("Data sent");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Data couldnt send error: {ex.Message}");
        }
    }

}
