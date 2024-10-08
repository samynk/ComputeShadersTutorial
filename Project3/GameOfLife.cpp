#include "GameOfLife.h"

GameOfLife::GameOfLife(GLuint width, GLuint height) :
	m_GameOfLifeShader{ "computeshaders/gameoflife/gameoflife.glsl" },
	m_ConvertComputeShader{ "computeshaders/convert_to_texture.glsl" },
	m_GridData0{ "patterns/spacefillersynth.rle", 0, width, height, 2 },
	m_GridData1{ 1, width, height, 1},
	m_Width{ width },
	m_Height{ height },
	m_ScaleFactorLocation{ 0 },
	m_ConvertInputDimensionLocation{ 0 },
	m_DimensionLocation{ 0 }
{
}

GameOfLife::~GameOfLife()
{
}

void GameOfLife::init(const SurfaceRenderer& renderer)
{
	m_GridData0.init();
	m_GridData1.init();

	m_GameOfLifeShader.compile();
	m_ConvertComputeShader.compile();

	m_ScaleFactorLocation = m_ConvertComputeShader.getParameterLocation("scaleFactor");
	m_ConvertInputDimensionLocation = m_ConvertComputeShader.getParameterLocation("inputDimension");
	m_DimensionLocation = m_GameOfLifeShader.getParameterLocation("gridDimension");
	
}

void GameOfLife::compute(const SurfaceRenderer& renderer)
{
	m_GameOfLifeShader.use();
	GLuint bufferWidth = m_GridData0.getBufferWidth();
	GLuint bufferHeight = m_GridData0.getBufferHeight();
	m_GameOfLifeShader.setUniformInteger2(m_DimensionLocation, bufferWidth, bufferHeight);
	m_GridData0.bindAsCompute(m_CurrentDataID);
	GLuint nextID = (m_CurrentDataID + 1) % 2;
	m_GridData1.bindAsCompute(nextID);
	m_GameOfLifeShader.compute(bufferWidth, bufferHeight);
	glMemoryBarrier(GL_SHADER_STORAGE_BARRIER_BIT);

	m_ConvertComputeShader.use();
	m_ConvertComputeShader.setUniformInteger(m_ScaleFactorLocation,m_GridData0.getScaleFactor());
	switch (m_CurrentDataID)
	{
	case 0:
		m_GridData0.bindAsCompute(0);
		break;
	case 1:
		m_GridData1.bindAsCompute(0);
		break;
	}

	renderer.bindAsCompute(); // binding number 0
	m_ConvertComputeShader.setUniformInteger2(m_ConvertInputDimensionLocation, bufferWidth, bufferHeight);
	m_ConvertComputeShader.compute(m_Width, m_Height);
	glMemoryBarrier(GL_SHADER_IMAGE_ACCESS_BARRIER_BIT);
	 
	m_CurrentDataID = nextID;
}
