#pragma once
#include "SurfaceRenderer.h"
#include "ComputeShader.h"
#include "ShaderStorageBufferObject.h"

class ConvertBuffer {
public:
	ConvertBuffer(GLuint width, GLuint height);
	~ConvertBuffer();

	void init(const SurfaceRenderer& renderer);
	void compute(const SurfaceRenderer& renderer);
private:
	ShaderStorageBufferObject<GLint>  m_GridData;
	ComputeShader m_ConvertComputeShader;
	GLint m_ScaleFactorLocation;
	GLint m_ConvertInputDimensionLocation;
	GLuint m_Width;
	GLuint m_Height;

	static const GLint scaleFactor = 4;
};

