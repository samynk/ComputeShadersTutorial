#pragma once
#include "SurfaceRenderer.h"
#include "ComputeShader.h"
#include "ShaderStorageBufferObject.h"
#include "Camera.h"
#include "glm/glm.hpp"

struct Ray {
	glm::vec4 m_Origin;
	glm::vec4 m_Direction;
};

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
	GLint m_CameraPositionLoc;
	GLint m_CameraMatrixLoc;
	glm::vec3 m_CameraPosition;

	ComputeShader m_ClearDepthBuffer;
	GLint m_ImgDimensionLoc2;
	GLint m_MaxDepthValueLoc;
	float m_MaxDepthValue{ 1000.0f };

	ComputeShader m_SphereRayTracer;
	GLint m_NrOfSpheresLoc;
	GLint m_ImgDimensionLoc3;

	ShaderStorageBufferObject<Ray> m_Rays;
	ShaderStorageBufferObject<float> m_DepthBuffer;
	ShaderStorageBufferObject<Sphere> m_Spheres;

	Camera m_Camera;
	float m_Phi{ 0};
	float m_T{ 0 };
};