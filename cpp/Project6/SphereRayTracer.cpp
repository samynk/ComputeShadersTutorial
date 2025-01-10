#include "SphereRayTracer.h"

SphereRayTracer::SphereRayTracer(GLuint width, GLuint height)
	:
	m_CameraRays{ "computeshaders/raytracer1/CameraRays.glsl" },
	m_ClearDepthBuffer{ "computeShaders/ClearDepthBuffer.glsl" },
	m_SphereRayTracer{ "computeshaders/raytracer1/SphereRayTracer.glsl" },
	m_DepthBuffer{ 0,width,height,1 },
	m_Rays{ 0,width,height,1 },
	m_Spheres{ 4 },
	m_ImgDimensionLoc1{ 0 },
	m_ImgDimensionLoc2{ 0 },
	m_ImgDimensionLoc3{ 0 },
	m_MaxDepthValueLoc{ 0 },
	m_NrOfSpheresLoc{ 0 }
{
}

SphereRayTracer::~SphereRayTracer()
{
}

void SphereRayTracer::init(const SurfaceRenderer& renderer)
{
	m_CameraRays.compile();
	m_ClearDepthBuffer.compile();
	m_SphereRayTracer.compile();

	m_Rays.init();
	m_DepthBuffer.init();


	m_Spheres.set(0, { glm::vec3{0,0,2}, 0.4f, glm::vec4(0, 1, 0, 1) });
	m_Spheres.set(1, { glm::vec3{-0.45,-0.25,1.9}, 0.25f, glm::vec4(1, 1, 0, 1) });
	m_Spheres.set(2, { glm::vec3{0.25,-0.2,2}, 0.3f, glm::vec4(0, 1, 1, 1) });
	m_Spheres.set(3, { glm::vec3{0,0.45,2}, 0.4f, glm::vec4(1 ,0, 1, 1) });
	m_Spheres.init();


	m_ImgDimensionLoc1 = m_CameraRays.getParameterLocation("imgDimension");
	m_ImgDimensionLoc2 = m_ClearDepthBuffer.getParameterLocation("imgDimension");
	m_MaxDepthValueLoc = m_ClearDepthBuffer.getParameterLocation("maxDepthValue");
	m_ImgDimensionLoc3 = m_SphereRayTracer.getParameterLocation("imgDimension");
	m_NrOfSpheresLoc = m_SphereRayTracer.getParameterLocation("nrOfSpheres");
}

void SphereRayTracer::compute(const SurfaceRenderer& renderer)
{
	// generate the rays
	m_CameraRays.use();
	m_Rays.bindAsCompute(0);
	m_CameraRays.setUniformInteger2(m_ImgDimensionLoc1, renderer.getWidth(), renderer.getHeight());
	m_CameraRays.compute(renderer.getWidth(), renderer.getHeight());


	// clear the depth buffer and set the entire buffer to the far plane.
	m_ClearDepthBuffer.use();
	m_DepthBuffer.bindAsCompute(0);
	m_ClearDepthBuffer.setUniformInteger2(m_ImgDimensionLoc2, renderer.getWidth(), renderer.getHeight());
	m_ClearDepthBuffer.setUniformFloat(m_MaxDepthValueLoc, m_MaxDepthValue);
	m_ClearDepthBuffer.compute(renderer.getWidth(), renderer.getHeight());

	glMemoryBarrier(GL_SHADER_STORAGE_BARRIER_BIT);

	// raytracer
	m_SphereRayTracer.use();

	m_Rays.bindAsCompute(0);
	m_DepthBuffer.bindAsCompute(1);
	m_Spheres.bindAsCompute(2);

	renderer.bindAsCompute(0);
	m_SphereRayTracer.setUniformInteger(m_NrOfSpheresLoc, m_Spheres.size());
	m_SphereRayTracer.setUniformInteger2(m_ImgDimensionLoc3, renderer.getWidth(), renderer.getHeight());
	m_SphereRayTracer.compute(renderer.getWidth(), renderer.getHeight());
	glMemoryBarrier(GL_SHADER_IMAGE_ACCESS_BARRIER_BIT);
}
