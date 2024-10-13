#include "RLEReader.h"

#include <fstream>
#include <sstream>
#include <iostream>
#include <cctype>
#include <vector>
#include <algorithm>

RLEReader::RLEReader(const std::string& rleFile)
	:m_RLEFile{ rleFile }
{
}

void RLEReader::read(int width, int height,std::vector<int>& array)
{
	std::ifstream inputFile(m_RLEFile);

	if (!inputFile.is_open()) {
		throw std::runtime_error("Error: Could not open the game of life pattern file " + m_RLEFile);
	}


	// Read the entire file into a string
	std::stringstream buffer;
	buffer << inputFile.rdbuf();
	inputFile.close();

	std::string line;
	int fwidth = 0, fheight = 0;
	
	int arrayIndex = 0;
	int xStart = 0;
	while (std::getline(buffer, line)) {
		// Process each line as needed
		std::cout << "Line: " << line << std::endl;
		line.erase(std::remove_if(line.begin(), line.end(), ::isspace),
			line.end());
		// Example: Skip comment lines that start with '#'
		if (line.empty() || line[0] == '#') {
			continue;  // Skip this iteration and read the next line
		}
		else if (line.find_first_of('x') != std::string::npos)
		{
			std::stringstream dimensions(line);
			std::string segment;
			while (std::getline(dimensions, segment, ','))
			{

				size_t equalIndex = segment.find_first_of('=');
				if (equalIndex != std::string::npos) {
					std::string key = segment.substr(0, equalIndex);
					std::string value = segment.substr(equalIndex + 1, segment.length() - equalIndex);
					if (key == "x") {
						fwidth = std::stoi(value);
					}
					else if (key == "y") {
						fheight = std::stoi(value);
					}
				}
			}
			if (fwidth > 0 && fheight > 0 && fwidth < width && fheight <height)
			{
				for (int idx = 0; idx < (width * height); ++idx) {
					array[idx] = 0;
				}
				
				xStart = (width - fwidth) / 2;
				arrayIndex = width * ((height - fheight) / 2) + xStart;
			}
			else {
				throw std::runtime_error("Error: Game of life file does not fit into provided storage.");
			}
		}
		else {
			std::string run_count_str;
			for (char& c : line) {

				if (std::isdigit(c)) {
					run_count_str += c;  // Accumulate digits
				}
				else if (c == 'b' || c == 'o') {
					int run_count = run_count_str.empty() ? 1 : std::stoi(run_count_str);
					int cell_value = (c == 'o') ? 1 : 0;
					for (int rleIndex = 0; rleIndex < run_count; ++rleIndex) {
						array[arrayIndex++] = cell_value;
					}
					run_count_str.clear();
				}
				else if (c == '$') {
					int run_count = run_count_str.empty() ? 1 : std::stoi(run_count_str);
					int currentLine = (arrayIndex / width) + 1 + (run_count - 1);
					arrayIndex = currentLine*width + xStart;
					run_count_str.clear();
				}
				else if (c == '!') {
					break;  // End of pattern
				}
			}
		}
	}
	std::cout << "Final arrayIndex " << arrayIndex << " should be smaller than " << (width * height) << std::endl;
}
