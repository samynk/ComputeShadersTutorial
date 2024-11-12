#include "ObjRayTracer.h"

#include "glm/gtc/constants.hpp"

ObjRayTracer::ObjRayTracer(GLuint width, GLuint height)
	:
	m_CameraRays{ "computeshaders/raytracer3/CameraRays.glsl" },
	m_RaysToTexture{ "computeshaders/raytracer3/RaysToTexture.glsl" },
	m_ObjRayTracer{ "computeshaders/raytracer3/ObjRayTracer.glsl" },
	m_Rays{ 0,width,height,1 },
	m_Object{ Mesh{"objects/dae.obj"} },
	m_ImgDimensionLoc1{ 0 },
	m_CameraMatrixLoc{ 0 },
	m_ImgDimensionLoc2{ 0 },
	m_ImgDimensionLoc3{ 0 },
	m_NrOfTrianglesLoc{ 0 },
	m_CameraPositionLoc{ 0 },
	m_CameraPosition{ 0,2,-4 },
	m_Camera{ glm::vec3{0,0,0}, glm::vec3{0,1,0}, glm::vec3{1,0,0}, true }
{
}

ObjRayTracer::~ObjRayTracer()
{
}

void ObjRayTracer::init(const SurfaceRenderer& renderer)
{
	m_CameraRays.compile();
	m_RaysToTexture.compile();
	m_ObjRayTracer.compile();

	m_Rays.init();
	m_Object.init();

	m_ImgDimensionLoc1 = m_CameraRays.getParameterLocation("imgDimension");
	m_CameraPositionLoc = m_CameraRays.getParameterLocation("cameraPosition");
	m_CameraMatrixLoc = m_CameraRays.getParameterLocation("cameraMatrix");
	m_ImgDimensionLoc2 = m_RaysToTexture.getParameterLocation("imgDimension");
	m_ImgDimensionLoc3 = m_ObjRayTracer.getParameterLocation("imgDimension");
	m_NrOfTrianglesLoc = m_ObjRayTracer.getParameterLocation("nrOfTriangles");
}

void ObjRayTracer::compute(const SurfaceRenderer& renderer)
{
	// to do replace with actual time
	m_T += 0.005;
	m_Phi =  glm::pi<float>() / 6 * sin(m_T) + glm::pi<float>()/3;

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
	writeOutput(renderer);
}

void ObjRayTracer::computeBounce(const SurfaceRenderer& renderer)
{
	m_ObjRayTracer.use();

	m_Rays.bindAsCompute(0);
	m_Object.bindVertices(1);
	m_Object.bindIndices(2);
	m_ObjRayTracer.setUniformInteger2(m_ImgDimensionLoc3, renderer.getWidth(), renderer.getHeight());
	m_ObjRayTracer.setUniformInteger(m_NrOfTrianglesLoc, m_Object.getNrOfTriangles());
	m_ObjRayTracer.compute(renderer.getWidth(), renderer.getHeight());
	glMemoryBarrier(GL_SHADER_STORAGE_BARRIER_BIT);
}

void ObjRayTracer::writeOutput(const SurfaceRenderer& renderer)
{
	m_RaysToTexture.use();
	m_Rays.bindAsCompute(0);
	renderer.bindAsCompute(0);
	m_RaysToTexture.setUniformInteger2(m_ImgDimensionLoc2, renderer.getWidth(), renderer.getHeight());
	m_RaysToTexture.compute(renderer.getWidth(), renderer.getHeight());
	glMemoryBarrier(GL_SHADER_IMAGE_ACCESS_BARRIER_BIT);
}
