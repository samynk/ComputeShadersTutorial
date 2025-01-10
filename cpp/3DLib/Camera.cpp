#include "Camera.h"
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

Camera::Camera(glm::vec3 position, glm::vec3 upAxis, glm::vec3 forwardAxis, bool rightHanded) :
	m_CameraPosition{ position },
	m_WorldUpAxis{ upAxis },
	m_RightHanded{ rightHanded }
{
	glm::vec3 cameraRight;
	if (m_RightHanded)
	{
		cameraRight = glm::cross(forwardAxis, m_WorldUpAxis);
	}
	else {
		cameraRight = glm::cross(m_WorldUpAxis, forwardAxis);
	}

	m_CameraReference[0] = glm::vec4{ cameraRight,0 };
	m_CameraReference[1] = glm::vec4{ m_WorldUpAxis, 0 };
	m_CameraReference[2] = glm::vec4{ forwardAxis,0 };
}

void Camera::addHorizontalRotation(float dPhi)
{
	m_Phi += dPhi;
}

void Camera::setPhi(float phi)
{
	m_Phi = phi;
}

void Camera::addVerticalRotation(float dAzimuth)
{
	m_Azimuth += dAzimuth;
}

void Camera::update()
{
	// Create the azimuth rotation
	glm::vec3 cameraRight = m_CameraReference[0];
	glm::mat4 azimuthMatrix = glm::rotate(glm::mat4(1.0f), m_Azimuth, cameraRight);
	glm::mat4 pitchedCamera = azimuthMatrix * m_CameraReference;

	// create the rotation around the up axis
	glm::vec3 cameraUp = m_CameraReference[1];
	m_CameraMatrix = glm::rotate(pitchedCamera, m_Phi, cameraUp);
	m_CameraMatrix[3] = glm::vec4{ m_CameraPosition,1 };
}

bool Camera::isRightHanded() const
{
	return m_RightHanded;
}

const glm::mat4& Camera::getCameraMatrix() const
{
	return m_CameraMatrix;
}
