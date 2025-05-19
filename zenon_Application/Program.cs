// Project: zenon_Application
// Author: LinoValdi (Linandro Valderrama)
// Last Modified Date: 19th May 2025

namespace zenon_Application
{
    // This class is the entry point of the application.
    // It processes command line arguments and invokes the ZenonLogAnalyzer.
    // The ZenonLogAnalyzer class is responsible for reading and processing the log file.
    // It extracts variable mappings and values, and writes the output to a file.
    // The output file is named based on the input file name with a ".out" extension.
    // The program handles exceptions and provides usage instructions if no arguments are provided.
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the path to the input file.");
                Console.WriteLine("Usage: ZenonLogAnalyzer <input_file_path>");
                Console.WriteLine("Example: ZenonLogAnalyzer.exe A1.TXT");
                Console.WriteLine("Note: The input file should be in the same directory as the executable or provide a full path.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            string inputFile = args[0];
            var analyzer = new ZenonLogAnalyzer();
            analyzer.ProcessFile(inputFile);
        }
    }
}