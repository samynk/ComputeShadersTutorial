#include "ComputeBlur.h"

ComputeBlur::ComputeBlur(GLuint width, GLuint height):
	m_InputImage{ 0, GL_READ_ONLY, "computeshaders/input2.png" },
	m_BlurredHorizontal(0, GL_READ_WRITE, "computeshaders/input2.png"),
	m_BlurComputeShader{ std::string("computeshaders/imageprocessing/blur.glsl") },
	m_HorizontalLocation{0}
{

}

ComputeBlur::~ComputeBlur()
{
}

void ComputeBlur::init(const SurfaceRenderer& renderer)
{
	m_BlurComputeShader.compile();
	m_HorizontalLocation = m_BlurComputeShader.getParameterLocation("horizontal");
	m_InputImage.init();
	m_BlurredHorizontal.init();
}

void ComputeBlur::compute(const SurfaceRenderer& renderer)
{
	m_BlurComputeShader.use();

	m_InputImage.bindAsCompute(0);
	m_BlurredHorizontal.bindAsCompute(1);
	m_BlurComputeShader.setUniformBool(m_HorizontalLocation, GL_TRUE);
	m_BlurComputeShader.compute(renderer.getWidth(), renderer.getHeight());
	// Ensure all writes to the image are completed before continuing
	glMemoryBarrier(GL_TEXTURE_UPDATE_BARRIER_BIT);

	m_BlurredHorizontal.bindAsCompute(0);
	renderer.bindAsCompute(1);
	m_BlurComputeShader.setUniformBool(m_HorizontalLocation, GL_FALSE);
	m_BlurComputeShader.compute(renderer.getWidth(), renderer.getHeight());
	glMemoryBarrier(GL_TEXTURE_UPDATE_BARRIER_BIT);
}
