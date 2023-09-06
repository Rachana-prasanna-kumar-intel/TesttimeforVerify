using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        // Step 1: Read the text file
        string textFilePath = GetValidFilePath("Enter the path to the input text file:");
        string[] allLines = File.ReadAllLines(textFilePath);

        // Step 2: Find start and end line indices
        int startLineIndex = -1;
        int endLineIndex = -1;

        for (int i = 0; i < allLines.Length; i++)
        {
            if (allLines[i].Contains("iC_tVerify Statistics: Per Test Instance [BEGIN]"))
            {
                startLineIndex = i + 1;
            }
            else if (allLines[i].Contains("iC_tVerify Statistics: Per Test Instance [FINISH]"))
            {
                endLineIndex = i;
                break;
            }
        }

        if (startLineIndex == -1 || endLineIndex == -1)
        {
            Console.WriteLine("Start or end line not found.");
            return;
        }

        string[] extractedLines = new string[endLineIndex - startLineIndex - 1];
        Array.Copy(allLines, startLineIndex + 1, extractedLines, 0, extractedLines.Length);

        // Step 3: Remove row 2 from the extracted lines
        extractedLines = extractedLines.Where((line, index) => index != 0).ToArray();

        // Step 4: Write the extracted lines to a CSV file
        string csvFilePath = GetValidOutputFilePath("Enter the path to the output CSV file:");
        using (StreamWriter writer = new StreamWriter(csvFilePath))
        {
            // Define the column names
            string[] columnNames = { "Index", "Private_Bytes", "Virtual_bytes", "Verify_time", "Test_Class", "Instance_Name" };

            // Write the column names to the CSV file
            writer.WriteLine(string.Join(",", columnNames));

            foreach (string line in extractedLines)
            {
                string[] fields = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] remainingFields = new string[fields.Length - 2];
                Array.Copy(fields, 2, remainingFields, 0, remainingFields.Length);

                writer.WriteLine(string.Join(",", remainingFields));
            }
        }

        Console.WriteLine("CSV file created successfully with row 2 removed.");
    }

    static string GetValidFilePath(string message)
    {
        string filePath;
        while (true)
        {
            Console.WriteLine(message);
            filePath = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                break;

            Console.WriteLine("Invalid file path. Please try again.");
        }

        return filePath;
    }

    static string GetValidOutputFilePath(string message)
    {
        string filePath;
        while (true)
        {
            Console.WriteLine(message);
            filePath = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
                    break;
            }

            Console.WriteLine("Invalid file path or directory does not exist. Please try again.");
        }

        return filePath;
    }
}
