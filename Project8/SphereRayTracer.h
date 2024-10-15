#pragma once
#include "SurfaceRenderer.h"
#include "ComputeShader.h"
#include "ShaderStorageBufferObject.h"
#include "Camera.h"
#include "glm/glm.hpp"

struct alignas(16) Ray {
	glm::vec4 originAndT;    // Combines origin (vec3) and tVal (float), 16 bytes
	glm::vec4 direction;     // Use vec4 instead of vec3 to maintain alignment, 16 bytes
	glm::vec4 color;         // Color information, already 16 bytes
	uint32_t rayHits;       // Number of ray hits, 4 bytes
	bool nullRay;       // Whether the ray is null, 1 byte
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
	void computeBounce(const SurfaceRenderer& renderer);
	void writeOutput(const SurfaceRenderer& renderer);
	ComputeShader m_CameraRays;
	GLint m_ImgDimensionLoc1;
	GLint m_CameraPositionLoc;
	GLint m_CameraMatrixLoc;
	glm::vec3 m_CameraPosition;

	ComputeShader m_RaysToTexture;
	GLint m_ImgDimensionLoc2;

	ComputeShader m_SphereRayTracer;
	GLint m_NrOfSpheresLoc;
	GLint m_ImgDimensionLoc3;

	ShaderStorageBufferObject<Ray> m_Rays;
	ShaderStorageBufferObject<Sphere> m_Spheres;

	Camera m_Camera;
	float m_Phi{ 0};
	float m_T{ 0 };
};