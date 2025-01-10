#include "Mesh.h"
#include "ObjReader.h"

Mesh::Mesh(const std::string& fileLocation)
	:m_FileLocation{ fileLocation }
{
	init();
}

Mesh::~Mesh()
{
	
}

void Mesh::init()
{
	ObjReader reader{ m_FileLocation };
	reader.parseObjFile();
	reader.buildOutputMesh();

	m_Indices = std::move(reader.getOIndexBuffer());

	std::vector<glm::vec3> positions = reader.getOVertices();
	std::vector<glm::vec3> normals = reader.getONormals();
	std::vector<glm::vec2> texcoords = reader.getOTexcoords();

	for (uint32_t vi = 0; vi < reader.getNumVertices();++vi)
	{
		m_Vertices.push_back({ positions[vi],0,normals[vi],0, texcoords[vi]});
	}
}
