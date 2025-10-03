#include "CameraRayGenerator.h"

CameraRayGenerator::CameraRayGenerator(GLuint width, GLuint height)
	:m_CameraRays{ "computeshaders/handson/CameraRays.glsl" },
	m_VisualizeRays{ "computeshaders/handson/VisualizeRays.glsl" },
	m_Rays{ 0,width,height,1 },
	m_ImgDimensionLoc1{ 0 }
{
	
}

CameraRayGenerator::~CameraRayGenerator()
{
}

void CameraRayGenerator::init(const SurfaceRenderer& renderer)
{

}

void CameraRayGenerator::compute(const SurfaceRenderer& renderer)
{
	
}
