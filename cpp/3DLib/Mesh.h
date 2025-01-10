#pragma once

#include <glm/glm.hpp>

#include <vector>
#include <string>


struct alignas(16) Vertex{
	glm::vec3 m_Position;
	float pad1;
	glm::vec3 m_Normal;
	float pad2;
	glm::vec2 m_UV;
};

class Mesh
{
public:
	Mesh(const std::string& fileLocation);
	~Mesh();

	void init();

	const std::vector<Vertex>& getVertices() const {
		return m_Vertices;
	}

	const std::vector<uint32_t>& getIndices() const {
		return m_Indices;
	}

	uint32_t getNrOfTriangles() const {
		return m_Indices.size() / 3;
	}
private:
	std::string m_FileLocation;
	std::vector<Vertex> m_Vertices;
	std::vector<uint32_t> m_Indices;
};
