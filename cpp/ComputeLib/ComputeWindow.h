#pragma once

#include "SurfaceRenderer.h"
#include "ComputeShader.h"
#include "GLImage.h"
#include "Compute.h"
#include "ShaderStorageBufferObject.h"

#include <GL/glew.h>
#include <GLFW/glfw3.h>

#include <string>
#include <memory>
#include <stdexcept>
#include <iostream>
#include <sstream>


struct GLFWDeleter {
	void operator()(GLFWwindow* window) const {
		if (window) {
			glfwDestroyWindow(window);
		}
		glfwTerminate();
	}
};

template<HasCompute C>
class ComputeWindow
{
public:
	ComputeWindow(GLuint width, GLuint height, const std::string& title, bool vsync = true);

	void init();
	void renderLoop();
	void close();

	bool isMovingForward();
	bool isMovingBack();
	bool isMovingLeft();
	bool isMovingRight();

private:
	void showFPS();
	double m_LastTime{ 0 };
	uint16_t m_NrOfFrames{ 0 };
	bool m_VSync;

	GLuint m_Width;
	GLuint m_Height;
	std::string m_Title;

	GLFWwindow* m_pWindow;
	SurfaceRenderer m_SurfaceRenderer;

	C m_Compute;
};

template<HasCompute C>
ComputeWindow<C>::ComputeWindow(GLuint width, GLuint height, const std::string& title, bool vsync)
	:m_Width{ width },
	m_Height{ height },
	m_Title{ title },
	m_SurfaceRenderer{ 0, width, height,"shaders/fullscreen_quad.vert","shaders/fullscreen_quad.frag" },
	m_pWindow{ nullptr },
	m_Compute{ width, height},
	m_VSync{true}
{

}

template<HasCompute C>
void ComputeWindow<C>::init()
{
	try {
		if (!glfwInit()) {
			throw std::runtime_error("Failed to initialize GLFW.");
		}

		// Set OpenGL version to 4.3 (compute shaders require this)
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		

		m_pWindow = glfwCreateWindow(m_Width, m_Height, m_Title.c_str(), nullptr, nullptr);
		if (!m_pWindow) {
			throw std::runtime_error("Failed to create GLFW window.");
		}
		glfwMakeContextCurrent(m_pWindow);
		glfwSwapInterval(m_VSync?1:0);
		
		glfwSetWindowUserPointer(m_pWindow, this);
		
		// Initialize GLEW
		glewExperimental = GL_TRUE; // Needed for core profile
		GLenum glewStatus = glewInit();
		if (glewStatus != GLEW_OK) {
			throw std::runtime_error(std::string("GLEW Initialization failed: ") + reinterpret_cast<const char*>(glewGetErrorString(glewStatus)));
		}
		m_SurfaceRenderer.init();
		m_Compute.init(m_SurfaceRenderer);
		m_LastTime = glfwGetTime();
	}
	catch (const std::exception& e) {
		std::cerr << "An exception occurred: " << e.what() << std::endl;
		close();
	}
}

template<HasCompute C>
void ComputeWindow<C>::renderLoop()
{
	while (!glfwWindowShouldClose(m_pWindow)) {
		// Input handling
		if (glfwGetKey(m_pWindow, GLFW_KEY_ESCAPE) == GLFW_PRESS)
			glfwSetWindowShouldClose(m_pWindow, true);

		// Rendering commands here
		glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT);

		m_Compute.compute(m_SurfaceRenderer);
		m_SurfaceRenderer.drawQuadWithTexture();
		showFPS();
		// Swap buffers and poll IO events
		glfwSwapBuffers(m_pWindow);
		glfwPollEvents();
	}
}

template<HasCompute C>
void ComputeWindow<C>::showFPS() {
	double currentTime = glfwGetTime();
	double delta = currentTime - m_LastTime;
	m_NrOfFrames++;
	if (delta >= 1.0) { // If last cout was more than 1 sec ago
		// cout << 1000.0 / double(m_NrOfFrames) << endl;

		uint32_t fps =static_cast<uint32_t>(double(m_NrOfFrames) / delta);

		std::stringstream ss;
		ss << m_Title << " [" << fps << " FPS]";

		glfwSetWindowTitle(m_pWindow, ss.str().c_str());

		m_NrOfFrames = 0;
		m_LastTime = currentTime;
	}
}

template<HasCompute C>
void ComputeWindow<C>::close()
{
	glfwDestroyWindow(m_pWindow);
	glfwTerminate();
}

template<HasCompute C>
bool ComputeWindow<C>::isMovingForward() {
	return glfwGetKey(m_pWindow, GLFW_KEY_UP) == GLFW_PRESS;
}

template<HasCompute C>
bool ComputeWindow<C>::isMovingBack() {
	return glfwGetKey(m_pWindow, GLFW_KEY_DOWN) == GLFW_PRESS;
}

template<HasCompute C>
bool ComputeWindow<C>::isMovingLeft() {
	return glfwGetKey(m_pWindow, GLFW_KEY_LEFT) == GLFW_PRESS;
}

template<HasCompute C>
bool ComputeWindow<C>::isMovingRight() {
	return glfwGetKey(m_pWindow, GLFW_KEY_RIGHT) == GLFW_PRESS;
}


