#pragma once
#include "SurfaceRenderer.h"
#include "ComputeShader.h"
#include "ShaderStorageBufferObject.h"
#include "glm/glm.hpp"

struct Sphere {
	glm::vec3 m_Location;
	float m_Radius;
	glm::vec4 m_Color;
};

class SphereRayTracer
{
public:
	SphereRayTracer(GLuint width, GLuint height);
	~SphereRayTracer();

	void init(const SurfaceRenderer& renderer);
	void compute(const SurfaceRenderer& renderer);
private:
	ComputeShader m_CameraRays;
	GLint m_ImgDimensionLoc1;
	
	ComputeShader m_ClearDepthBuffer;
	GLint m_ImgDimensionLoc2;
	GLint m_MaxDepthValueLoc;
	float m_MaxDepthValue{ 1000.0f };
	
	ComputeShader m_SphereRayTracer;
	GLint m_NrOfSpheresLoc;
	GLint m_ImgDimensionLoc3;

	ShaderStorageBufferObject<glm::vec4> m_Rays;
	ShaderStorageBufferObject<float> m_DepthBuffer;
	ShaderStorageBufferObject<Sphere> m_Spheres;
};
