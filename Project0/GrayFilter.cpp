#include "GrayFilter.h"

GrayFilter::GrayFilter(GLuint width, GLuint height)
	:m_InputImage{ 0, GL_READ_ONLY, "computeshaders/input2.png" },
	m_GrayComputeShader{"computeshaders/imageprocessing/grayscale.glsl"}
{

}

GrayFilter::~GrayFilter()
{
}

void GrayFilter::init(const SurfaceRenderer& renderer) 
{
	m_InputImage.init();
	m_GrayComputeShader.compile();
}

void GrayFilter::compute(const SurfaceRenderer& renderer) 
{
	m_GrayComputeShader.use();
	m_InputImage.bindAsCompute(0);
	renderer.bindAsCompute(1);
	m_GrayComputeShader.compute(renderer.getWidth(), renderer.getHeight());
}
