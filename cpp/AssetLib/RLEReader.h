#pragma once
#include <string>

#include <vector>
class RLEReader
{
public:

	RLEReader(const std::string& rleFile);
	void read(int width, int height, std::vector<int>& dest);
private:
	std::string m_RLEFile;
};
