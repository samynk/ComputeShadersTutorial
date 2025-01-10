#include "SphereRayTracer.h"

#include "glm/gtc/constants.hpp"

SphereRayTracer::SphereRayTracer(GLuint width, GLuint height)
	:
	m_CameraRays{ "computeshaders/raytracer3/CameraRays.glsl" },
	m_RaysToTexture{ "computeShaders/raytracer3/RaysToTexture.glsl" },
	m_SphereRayTracer{ "computeshaders/raytracer3/SphereRayTracer.glsl" },
	m_Rays{ 0,width,height,1 },
	m_Spheres{ 4 },
	m_ImgDimensionLoc1{ 0 },
	m_CameraMatrixLoc{ 0 },
	m_ImgDimensionLoc2{ 0 },
	m_ImgDimensionLoc3{ 0 },
	m_NrOfSpheresLoc{ 0 },
	m_CameraPositionLoc{ 0 },
	m_CameraPosition{ 0,0,0.75 },
	m_Camera{ glm::vec3{0,0,0}, glm::vec3{0,1,0}, glm::vec3{1,0,0}, true }
{
}

SphereRayTracer::~SphereRayTracer()
{
}

void SphereRayTracer::init(const SurfaceRenderer& renderer)
{
	m_CameraRays.compile();
	m_RaysToTexture.compile();
	m_SphereRayTracer.compile();

	m_Rays.init();
	m_Spheres.set(0, { glm::vec3{-0.35,0,2}, 0.2f, glm::vec4(0, 1, 0, 1) });
	m_Spheres.set(1, { glm::vec3{0.35,0,2}, 0.2f, glm::vec4(1, 1, 0, 1) });
	m_Spheres.set(2, { glm::vec3{0,-0.25,1.5}, 0.2f, glm::vec4(0, 1, 1, 1) });
	m_Spheres.set(3, { glm::vec3{0,0.25,2}, 0.2f, glm::vec4(1 ,0, 1, 1) });
	m_Spheres.init();


	m_ImgDimensionLoc1 = m_CameraRays.getParameterLocation("imgDimension");
	m_CameraPositionLoc = m_CameraRays.getParameterLocation("cameraPosition");
	m_CameraMatrixLoc = m_CameraRays.getParameterLocation("cameraMatrix");
	m_ImgDimensionLoc2 = m_RaysToTexture.getParameterLocation("imgDimension");
	m_ImgDimensionLoc3 = m_SphereRayTracer.getParameterLocation("imgDimension");
	m_NrOfSpheresLoc = m_SphereRayTracer.getParameterLocation("nrOfSpheres");
}

void SphereRayTracer::compute(const SurfaceRenderer& renderer)
{
	// to do replace with actual time
	m_T += 0.005;
	m_Phi = glm::pi<float>()/2 + glm::pi<float>() / 6 * sin(m_T);

	m_Camera.setPhi(m_Phi);
	m_Camera.update();
	//m_CameraPosition.z += 0.001f;
	// generate the rays
	m_CameraRays.use();
	m_Rays.bindAsCompute(0);
	m_CameraRays.setUniformInteger2(m_ImgDimensionLoc1, renderer.getWidth(), renderer.getHeight());
	m_CameraRays.setUniformFloat3(m_CameraPositionLoc, m_CameraPosition.x, m_CameraPosition.y, m_CameraPosition.z);
	m_CameraRays.setUniformMatrix(m_CameraMatrixLoc, m_Camera.getCameraMatrix());
	m_CameraRays.compute(renderer.getWidth(), renderer.getHeight());
	glMemoryBarrier(GL_SHADER_STORAGE_BARRIER_BIT);

	// bounce 1
	computeBounce(renderer);
	// bounce 2
	computeBounce(renderer);
	//// bounce 3
	//computeBounce(renderer);
	//// bounce 4
	//computeBounce(renderer);
	// write the final output
	writeOutput(renderer);
}

void SphereRayTracer::computeBounce(const SurfaceRenderer& renderer)
{
	m_SphereRayTracer.use();

	m_Rays.bindAsCompute(0);
	m_Spheres.bindAsCompute(1);
	m_SphereRayTracer.setUniformInteger(m_NrOfSpheresLoc, m_Spheres.size());
	m_SphereRayTracer.setUniformInteger2(m_ImgDimensionLoc3, renderer.getWidth(), renderer.getHeight());
	m_SphereRayTracer.compute(renderer.getWidth(), renderer.getHeight());
	glMemoryBarrier(GL_SHADER_STORAGE_BARRIER_BIT);
}

void SphereRayTracer::writeOutput(const SurfaceRenderer& renderer)
{
	m_RaysToTexture.use();
	m_Rays.bindAsCompute(0);
	renderer.bindAsCompute(0);
	m_RaysToTexture.setUniformInteger2(m_ImgDimensionLoc2, renderer.getWidth(), renderer.getHeight());
	m_RaysToTexture.compute(renderer.getWidth(), renderer.getHeight());
	glMemoryBarrier(GL_SHADER_IMAGE_ACCESS_BARRIER_BIT);
}
