#pragma once

#include "ComputeShader.h"
#include "SurfaceRenderer.h"
#include "ShaderStorageBufferObject.h"
#include "GL/glew.h"

struct Boid{
	float m_px, m_py;
	float m_vx, m_vy;
};

class FlockCompute
{
public:
	FlockCompute(GLuint width, GLuint height);
	~FlockCompute();

	void init(const SurfaceRenderer& renderer);
	void compute(const SurfaceRenderer& renderer);

private:
	ShaderStorageBufferObject<Boid>  m_GridData0;
	ShaderStorageBufferObject<Boid>  m_GridData1;

	ComputeShader m_FlockCompute;
	ComputeShader m_ClearShader;
	ComputeShader m_ConvertFlock;

	GLint m_CurrentBufferID = 0;
};
