#include "CameraRayGenerator.h"

CameraRayGenerator::CameraRayGenerator(GLuint width, GLuint height)
	:m_CameraRays{ "computeshaders/raytracer1/CameraRays.glsl" },
	m_VisualizeRays{ "computeshaders/raytracer1/VisualizeRays.glsl" },
	m_Rays{ 0,width,height,1 },
	m_ImgDimensionLoc1{ 0 },
	m_ImgDimensionLoc2{ 0 }
{
	
}

CameraRayGenerator::~CameraRayGenerator()
{
}

void CameraRayGenerator::init(const SurfaceRenderer& renderer)
{
	
	m_CameraRays.compile();
	m_ImgDimensionLoc1 = m_CameraRays.getParameterLocation("imgDimension");
	m_VisualizeRays.compile();
	m_ImgDimensionLoc2 = m_VisualizeRays.getParameterLocation("imgDimension");
	m_Rays.init();
}

void CameraRayGenerator::compute(const SurfaceRenderer& renderer)
{
	m_CameraRays.use();
	m_CameraRays.setUniformInteger2(m_ImgDimensionLoc1, renderer.getWidth(), renderer.getHeight());
	m_Rays.bindAsCompute(0);
	m_CameraRays.compute(renderer.getWidth(), renderer.getHeight());

	// convert and render the rays as colors.
	m_VisualizeRays.use();
	m_VisualizeRays.setUniformInteger2(m_ImgDimensionLoc2, renderer.getWidth(), renderer.getHeight());
	m_Rays.bindAsCompute(0);
	renderer.bindAsCompute(0);
	m_VisualizeRays.compute(renderer.getWidth(), renderer.getHeight());
}
