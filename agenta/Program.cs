using System.Text.RegularExpressions;

class Program
{
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

        var indexedData = new Dictionary<string, Dictionary<string, int>>();

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

        // test output
        foreach (var fileEntry in indexedData)
        {
            foreach (var wordEntry in fileEntry.Value)
            {
                Console.WriteLine($"{fileEntry.Key}:{wordEntry.Key}:{wordEntry.Value}");
            }
        }

        Console.WriteLine("Press any key to close");
        Console.ReadKey();
    }
}
