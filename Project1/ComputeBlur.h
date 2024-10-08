#pragma once
#include "SurfaceRenderer.h"
#include "ComputeShader.h"
#include "GL/glew.h"

class ComputeBlur {
public:
	ComputeBlur(GLuint width, GLuint height);
	~ComputeBlur();

	void init(const SurfaceRenderer & renderer);
	void compute(const SurfaceRenderer & renderer);
private:
	GLImage m_InputImage; 
	GLImage m_BlurredHorizontal;
	ComputeShader m_BlurComputeShader;

	GLint m_HorizontalLocation;
};
