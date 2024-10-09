#pragma once
#include "glm/glm.hpp"

class Camera final
{
public:
	Camera(glm::vec3 position, glm::vec3 upAxis, glm::vec3 forwardAxis, bool rh);

	void addHorizontalRotation(float dPhi);
	void setPhi(float phi);
	void addVerticalRotation(float dAzimuth);
	void update();

	bool isRightHanded() const;

	const glm::mat4& getCameraMatrix() const;

private:
	glm::vec3 m_CameraPosition{ 0,0,0 };
	glm::vec3 m_WorldUpAxis{ 0,1,0 }; // default Y-axis is the world up.
	bool m_RightHanded{ true };

	float m_Phi{ 0 }; // angle around the X-axis.
	float m_Azimuth{ 0 }; // angle from the base plane.

	glm::mat4 m_CameraReference{ 1.0 };
	glm::mat4 m_CameraMatrix{ 1.0 }; // initialize as identity matrix.
	glm::mat4 m_ProjectionMatrix{ 1.0 }; // initialize as identity matrix.
};
