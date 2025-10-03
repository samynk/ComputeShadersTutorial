#include "AddFloats.h"
#include <iostream>

AddFloats::AddFloats(GLuint width, GLuint height) :
	m_AddFloatsComputeShader{ std::string("computeshaders/handson/addfloats.glsl") },
	m_Operand1Data{ 1024 },
	m_Operand2Data{ 1024 },
	m_Result{ 1024 }
{
}

AddFloats::~AddFloats()
{
}

void AddFloats::init(const SurfaceRenderer& renderer)
{
	// initialize compute shaders and buffers
}

void AddFloats::compute(const SurfaceRenderer& renderer)
{
	// use compute shader
	// bind buffers
	// dispatch compute shader
	// download result.
}
