#pragma once
#include "SurfaceRenderer.h"
#include "ShaderStorageBufferObject.h"
#include "ComputeShader.h"
#include "glm/glm.hpp"


class CameraRayGenerator
{
public:
	CameraRayGenerator(GLuint width, GLuint height);
	~CameraRayGenerator();

	void init(const SurfaceRenderer& renderer);
	void compute(const SurfaceRenderer& renderer);
private:
	ComputeShader m_CameraRays;
	GLint m_ImgDimensionLoc1;
	ComputeShader m_VisualizeRays;
	GLint m_ImgDimensionLoc2;
	ShaderStorageBufferObject<glm::vec4> m_Rays;
};
