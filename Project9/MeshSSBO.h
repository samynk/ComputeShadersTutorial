#pragma once

#include "Mesh.h"
#include "ShaderStorageBufferObject.h"

#include <string>
class MeshSSBO

{
public:
	MeshSSBO(const Mesh& mesh);
	~MeshSSBO();

	void bindVertices(GLuint bindingId);
	void bindIndices(GLuint bindingId);

	void init();

	GLint getNrOfTriangles() {
		return m_NrOfTriangles;
	}

private:
	GLint m_NrOfTriangles;
	ShaderStorageBufferObject<Vertex> m_Vertices;
	ShaderStorageBufferObject<uint32_t> m_Indices;
};
