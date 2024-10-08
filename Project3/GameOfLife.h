#pragma once
#include "SurfaceRenderer.h"
#include "ShaderStorageBufferObject.h"
#include "ComputeShader.h"

class GameOfLife
{
public:
	GameOfLife(GLuint width, GLuint height);
	~GameOfLife();

	void init(const SurfaceRenderer& renderer);
	void compute(const SurfaceRenderer& renderer);
private:
	ShaderStorageBufferObject<GLint>  m_GridData0;
	ShaderStorageBufferObject<GLint>  m_GridData1;
	ComputeShader m_ConvertComputeShader;
	ComputeShader m_GameOfLifeShader;
	GLint m_ScaleFactorLocation;
	GLint m_DimensionLocation;
	GLint m_ConvertInputDimensionLocation;

	GLuint m_Width;
	GLuint m_Height;

	GLuint m_CurrentDataID{ 0 };
	GLuint m_FrameCount{ 0 };
};
