#include "MeshSSBO.h"


MeshSSBO::MeshSSBO(const Mesh& mesh)
	:
	m_Vertices{mesh.getVertices()},
	m_Indices{mesh.getIndices()},
	m_NrOfTriangles(mesh.getNrOfTriangles())
{

}

MeshSSBO::~MeshSSBO()
{
}

void MeshSSBO::bindVertices(GLuint bindingId)
{
	m_Vertices.bindAsCompute(bindingId);
}

void MeshSSBO::bindIndices(GLuint bindingId)
{
	m_Indices.bindAsCompute(bindingId);
}

void MeshSSBO::init()
{
	m_Vertices.init();
	m_Indices.init();
}
