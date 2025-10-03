#pragma once
#include "SurfaceRenderer.h"
#include "ComputeShader.h"
#include "ShaderStorageBufferObject.h"
#include "GL/glew.h"

class AddFloats {
public:
	AddFloats(GLuint width, GLuint height);
	~AddFloats();

	void init(const SurfaceRenderer & renderer);
	void compute(const SurfaceRenderer & renderer);
private:
	ShaderStorageBufferObject<GLfloat>  m_Operand1Data;
	ShaderStorageBufferObject<GLfloat>  m_Operand2Data;
	ShaderStorageBufferObject<GLfloat>  m_Result;
	ComputeShader m_AddFloatsComputeShader;
};
