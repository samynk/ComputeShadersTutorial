#include "FlockCompute.h"
#include <random>

FlockCompute::FlockCompute(GLuint width, GLuint height)
	:
	m_GridData0{ 0,10000,1,1 },
	m_GridData1{ 1,10000,1,1 },
	m_FlockCompute{ "computeshaders/flocking/flocking.glsl" },
	m_ConvertFlock{ "computeshaders/flocking/convert_flock_to_texture.glsl" },
	m_ClearShader{ "computeshaders/flocking/clear.glsl" }
{
}

FlockCompute::~FlockCompute()
{
}

void FlockCompute::init(const SurfaceRenderer& renderer)
{
	m_FlockCompute.compile();
	m_ConvertFlock.compile();
	m_ClearShader.compile();


	static std::random_device rd;
	static std::mt19937 gen(rd());
	std::uniform_real_distribution<float> dis(0.0f, 1.0f);
	std::uniform_real_distribution<float> disAngle(0.0f, 2.0f * 3.1415926);
	float radius = 100.0f;

	for (int iboid = 0; iboid < 2048; ++iboid)
	{
		// Generate random spherical coordinates
		float theta = disAngle(gen);  // Random angle in [0, 2*pi]
		float phi = acos(1.0f - 2.0f * dis(gen));  // Random angle in [0, pi] for uniform spherical distribution
		float r = radius * cbrt(dis(gen));  // Random radius scaled to maintain uniform density within sphere
		float speed =3.0f*dis(gen) + 1.0f;

		// Convert spherical coordinates to Cartesian
		float x = r * cos(phi);
		float y = r * sin(phi);
		float vx = speed*cos(theta);
		float vy = speed*sin(theta);

		m_GridData0.set(iboid, { x,y,vx,vy });
	}
	m_GridData0.init();
	m_GridData1.init();
}

void FlockCompute::compute(const SurfaceRenderer& renderer)
{
	GLint nextID = (m_CurrentBufferID + 1) % 2;
	m_FlockCompute.use();
	m_GridData0.bindAsCompute(m_CurrentBufferID);
	m_GridData1.bindAsCompute(nextID);
	m_FlockCompute.compute(m_GridData0.getBufferWidth(), 1);

	glMemoryBarrier(GL_SHADER_STORAGE_BARRIER_BIT);

	/*
	m_ClearShader.use();
	m_ClearShader.compute(renderer.getWidth(), renderer.getHeight());
	glMemoryBarrier(GL_SHADER_IMAGE_ACCESS_BARRIER_BIT);
	*/

	m_ConvertFlock.use();
	switch (m_CurrentBufferID) {
		case 0: m_GridData1.bindAsCompute(0); break;
		case 1: m_GridData0.bindAsCompute(0); break;
	}
	renderer.bindAsCompute(0);
	m_ConvertFlock.compute(m_GridData0.getBufferWidth(), 1);
	glMemoryBarrier(GL_SHADER_IMAGE_ACCESS_BARRIER_BIT);
	m_CurrentBufferID = nextID;
}
