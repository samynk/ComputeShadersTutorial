#pragma once
#include "SurfaceRenderer.h"
#include "ComputeShader.h"
#include "GL/glew.h"

class GrayFilter
{
public:
	GrayFilter(GLuint width, GLuint height);
	~GrayFilter();

	void init(const SurfaceRenderer& renderer);
	void compute(const SurfaceRenderer& renderer);
private:
	GLImage m_InputImage;
	ComputeShader m_GrayComputeShader;
};
