using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ComputeShaderTutorial
{
    public class RLEReader
    {
        private readonly string _rleFile;

        public RLEReader(string rleFile)
        {
            _rleFile = rleFile;
        }

        /// <summary>
        /// Reads an RLE file describing a Game of Life pattern and populates `array`.
        /// </summary>
        /// <param name="width">Width of the target grid</param>
        /// <param name="height">Height of the target grid</param>
        /// <param name="array">1D array of length (width * height)</param>
        public void Read(int width, int height, int[] array)
        {
            // Make sure the file exists
            if (!File.Exists(_rleFile))
            {
                throw new FileNotFoundException(
                    $"Error: Could not open the game of life pattern file {_rleFile}",
                    _rleFile
                );
            }

            // Read the entire file into a single string
            string fileContents = File.ReadAllText(_rleFile);

            // Use a StringReader to process line by line
            using StringReader reader = new StringReader(fileContents);

            // Variables to store the declared width/height from the file
            int fwidth = 0;
            int fheight = 0;

            // We'll keep track of the array index where we write new cells
            int arrayIndex = 0;

            // This xStart determines where the pattern is horizontally centered
            int xStart = 0;

            // Read lines until we run out
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                // For debug/logging (mirrors std::cout in C++)
                Console.WriteLine($"Line: {line}");

                // Remove all whitespace from the line
                // (Similar to erase/remove_if(line.begin(), line.end(), ::isspace))
                line = new string(line.Where(c => !char.IsWhiteSpace(c)).ToArray());

                // Skip empty lines or comments
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }
                else if (line.Contains('x'))
                {
                    // Parse something like: x = 10, y = 5
                    // We'll split by comma, then by '='
                    string[] segments = line.Split(',');

                    foreach (var segment in segments)
                    {
                        int equalIndex = segment.IndexOf('=');
                        if (equalIndex >= 0)
                        {
                            string key = segment.Substring(0, equalIndex).Trim();
                            string value = segment.Substring(equalIndex + 1).Trim();

                            if (key == "x")
                            {
                                fwidth = int.Parse(value);
                            }
                            else if (key == "y")
                            {
                                fheight = int.Parse(value);
                            }
                        }
                    }

                    // Validate that the file’s dimensions fit within our array
                    if (fwidth > 0 && fheight > 0 && fwidth < width && fheight < height)
                    {
                        // Clear array
                        for (int idx = 0; idx < width * height; idx++)
                        {
                            array[idx] = 0;
                        }

                        // Center the pattern horizontally and vertically
                        xStart = (width - fwidth) / 2;
                        arrayIndex = width * ((height - fheight) / 2) + xStart;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "Error: Game of life file does not fit into provided storage."
                        );
                    }
                }
                else
                {
                    // RLE data lines
                    // Example: "3o$2b$..." meaning run counts and cell states
                    string runCountStr = "";

                    foreach (char c in line)
                    {
                        if (char.IsDigit(c))
                        {
                            // Accumulate digits for run count
                            runCountStr += c;
                        }
                        else if (c == 'b' || c == 'o')
                        {
                            // If runCountStr is empty, default to 1
                            int runCount = string.IsNullOrEmpty(runCountStr)
                                            ? 1
                                            : int.Parse(runCountStr);

                            // 'o' = live cell (1), 'b' = dead cell (0)
                            int cellValue = (c == 'o') ? 1 : 0;

                            for (int i = 0; i < runCount; i++)
                            {
                                array[arrayIndex++] = cellValue;
                            }

                            // Reset runCountStr
                            runCountStr = "";
                        }
                        else if (c == '$')
                        {
                            // Move down one or more lines
                            int runCount = string.IsNullOrEmpty(runCountStr)
                                            ? 1
                                            : int.Parse(runCountStr);

                            // Determine the current row + runCount
                            int currentLine = (arrayIndex / width) + (runCount - 1) + 1;

                            arrayIndex = currentLine * width + xStart;

                            runCountStr = "";
                        }
                        else if (c == '!')
                        {
                            // End of pattern
                            break;
                        }
                        // If other characters appear, we ignore them (or handle errors as needed)
                    }
                }
            }

            Console.WriteLine($"Final arrayIndex {arrayIndex} should be smaller than {width * height}");
        }
    }

}
