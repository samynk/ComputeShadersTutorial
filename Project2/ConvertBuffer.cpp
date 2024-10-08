#include "ConvertBuffer.h"


ConvertBuffer::ConvertBuffer(GLuint width, GLuint height) :
	m_ConvertComputeShader{ "computeshaders/convert_to_texture.glsl" },
	m_GridData{ "patterns/spacefillersynth.rle", 0, width, height,1 },
	m_Width{ width },
	m_Height{ height },
	m_ScaleFactorLocation{ 0 },
	m_ConvertInputDimensionLocation{ 0 }
{
}

ConvertBuffer::~ConvertBuffer()
{
}

void ConvertBuffer::init(const SurfaceRenderer& renderer)
{
	m_ConvertComputeShader.compile();
	m_GridData.init();
	m_ScaleFactorLocation = m_ConvertComputeShader.getParameterLocation("scaleFactor");
	m_ConvertInputDimensionLocation = m_ConvertComputeShader.getParameterLocation("inputDimension");
}

void ConvertBuffer::compute(const SurfaceRenderer& renderer)
{
	m_ConvertComputeShader.use();
	m_ConvertComputeShader.setUniformInteger(m_ScaleFactorLocation, m_GridData.getScaleFactor());
	m_GridData.bindAsCompute(0);
	m_ConvertComputeShader.setUniformInteger2(m_ConvertInputDimensionLocation, 
		m_GridData.getBufferWidth(), 
		m_GridData.getBufferHeight());
	renderer.bindAsCompute(0); // binding number 0
	m_ConvertComputeShader.compute(m_Width, m_Height);
	glMemoryBarrier(GL_SHADER_IMAGE_ACCESS_BARRIER_BIT);
}
