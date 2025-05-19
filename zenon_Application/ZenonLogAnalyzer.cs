// Project: zenon_Application
// Author: LinoValdi (Linandro Valderrama)
// Last Modified Date: 19th May 2025

using System.Globalization;

namespace zenon_Application
{
    public class ZenonLogAnalyzer
    {
        private readonly Dictionary<string, int> _channelToVariableIdMap = [];

        private readonly Dictionary<int, VariableStats> _variableStatistics = [];

        /// <summary>
        /// Processes the input file and extracts variable mappings and values.
        /// It calculates the min and max values for each variable and writes the output to a file.
        /// </summary>
        /// <param name="inputFilePath">The path to the input file.</param>
        /// <remarks>
        /// The input file is expected to have sections marked by lines starting and ending with '-'.
        /// The sections of interest are:
        /// - VARIABLES: Contains variable mappings in the format "ChannelIndex=VariableId".
        /// - VALUES: Contains value entries in the format "@ChannelIndex:Value;Status;Timestamp".
        /// </remarks>
        public void ProcessFile(string inputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine($"Error: Input file not found: {inputFilePath}");
                return;
            }

            string currentSection = "";

            try
            {
                using (StreamReader reader = new(inputFilePath))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (line.StartsWith("-") && line.EndsWith("-"))
                        {
                            currentSection = line;
                            continue;
                        }

                        switch (currentSection)
                        {
                            case "-VARIABLES-":
                                ParseVariableMapping(line);
                                break;
                            case "-VALUES-":
                                ParseValueEntry(line);
                                break;
                        }
                    }
                }

                WriteOutput(inputFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Parses a line from the variable mapping section.
        /// It extracts the channel index and variable ID.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        private void ParseVariableMapping(string line)
        {
            string[] parts = line.Split('=');
            if (parts.Length == 2)
            {
                string channelIndex = parts[0].Trim();

                if (int.TryParse(parts[1].Trim(), out int variableId))
                    _channelToVariableIdMap[channelIndex] = variableId;
                else
                    Console.WriteLine($"Warning: Could not parse variable ID in line: {line}");
            }
            else
            {
                Console.WriteLine($"Warning: Malformed variable mapping line: {line}");
            }
        }


        /// <summary>
        /// Parses a line from the value entry section.
        /// It extracts the channel index, value, status, and timestamp.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <remarks>
        /// The expected format is "@ChannelIndex:Value;Status;Timestamp".
        /// The method updates the statistics for the corresponding variable.
        /// </remarks>
        private void ParseValueEntry(string line)
        {
            string[] channelAndValueParts = line.Split([':'], 2);
            if (channelAndValueParts.Length == 2)
            {
                string channelIndex = channelAndValueParts[0].Trim();
                string[] valueParts = channelAndValueParts[1].Split(';');

                if (valueParts.Length == 3)
                {
                    if (!_channelToVariableIdMap.TryGetValue(channelIndex, out int variableId))
                    {
                        Console.WriteLine($"Warning: Kanal-Index '{channelIndex}' not found in variable mappings. Skipping line: {line}");
                        return;
                    }

                    // Using InvariantCulture for parsing double to handle '.' as decimal separator
                    if (!double.TryParse(valueParts[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        Console.WriteLine($"Warning: Could not parse value in line: {line}");
                        return;
                    }

                    // Parsing timestamp. Format: "dd.MM.yyyy HH:mm:ss.fff"
                    if (!DateTime.TryParseExact(valueParts[2].Trim(), "dd.MM.yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timestamp))
                    {
                        Console.WriteLine($"Warning: Could not parse timestamp in line: {line}");
                        return;
                    }

                    if (!_variableStatistics.ContainsKey(variableId))
                        _variableStatistics[variableId] = new VariableStats(variableId);

                    _variableStatistics[variableId].Update(value, timestamp);
                }
                else
                {
                    Console.WriteLine($"Warning: Malformed value entry (not enough parts after ';'): {line}");
                }
            }
            else
            {
                Console.WriteLine($"Warning: Malformed value entry (no ':' found): {line}");
            }
        }


        /// <summary>
        /// Writes the output to a file.
        /// The output file is named based on the input file name with "_stats" appended.
        /// </summary>
        /// <param name="inputFilePath">The path to the input file.</param>
        /// <remarks>
        /// The output file contains the statistics for each variable in the format:
        /// "VariableId;MinValue;MinTimestamp;MaxValue;MaxTimestamp".
        /// </remarks>
        private void WriteOutput(string inputFilePath)
        {
            string outputDirectory = Path.GetDirectoryName(inputFilePath) ?? ".";
            string outputFileName = Path.GetFileNameWithoutExtension(inputFilePath) + "_stats.txt";
            string outputFilePath = Path.Combine(outputDirectory, outputFileName);

            try
            {
                using (StreamWriter writer = new(outputFilePath))
                {
                    foreach (var statsEntry in _variableStatistics.OrderBy(kv => kv.Key))
                        writer.WriteLine(statsEntry.Value.ToString());
                }
                Console.WriteLine($"Successfully processed file. Output written to: {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing output file: {ex.Message}");
            }
        }
    }
}