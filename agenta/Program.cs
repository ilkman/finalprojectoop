using System.Text.RegularExpressions;
using System.Threading;

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

        // test output remove later
        foreach (var fileEntry in indexedData)
        {
            foreach (var wordEntry in fileEntry.Value)
            {
                Console.WriteLine($"{fileEntry.Key}:{wordEntry.Key}:{wordEntry.Value}");
            }
        }

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
}
