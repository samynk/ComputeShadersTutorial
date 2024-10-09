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
	ComputeWindow(GLuint width, GLuint height, const std::string& title);

	void init();
	void renderLoop();
	void close();

	bool isMovingForward();
	bool isMovingBack();
	bool isMovingLeft();
	bool isMovingRight();

private:
	GLuint m_Width;
	GLuint m_Height;
	std::string m_Title;

	GLFWwindow* m_pWindow;
	SurfaceRenderer m_SurfaceRenderer;

	C m_Compute;
};

template<HasCompute C>
ComputeWindow<C>::ComputeWindow(GLuint width, GLuint height, const std::string& title)
	:m_Width{ width },
	m_Height{ height },
	m_Title{ title },
	m_SurfaceRenderer{ 0, width, height,"shaders/fullscreen_quad.vert","shaders/fullscreen_quad.frag" },
	m_pWindow{ nullptr },
	m_Compute{ width, height}
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
		glfwSetWindowUserPointer(m_pWindow, this);
		// glfwSwapInterval(1);

		// Initialize GLEW
		glewExperimental = GL_TRUE; // Needed for core profile
		GLenum glewStatus = glewInit();
		if (glewStatus != GLEW_OK) {
			throw std::runtime_error(std::string("GLEW Initialization failed: ") + reinterpret_cast<const char*>(glewGetErrorString(glewStatus)));
		}
		m_SurfaceRenderer.init();
		m_Compute.init(m_SurfaceRenderer);

		
		

		/*
		GLint workGroupCount[3];
		glGetIntegeri_v(GL_MAX_COMPUTE_WORK_GROUP_COUNT, 0, &workGroupCount[0]); // X dimension
		glGetIntegeri_v(GL_MAX_COMPUTE_WORK_GROUP_COUNT, 1, &workGroupCount[1]); // Y dimension
		glGetIntegeri_v(GL_MAX_COMPUTE_WORK_GROUP_COUNT, 2, &workGroupCount[2]); // Z dimension

		std::cout << "Maximum number of work groups (X): " << workGroupCount[0] << std::endl;
		std::cout << "Maximum number of work groups (Y): " << workGroupCount[1] << std::endl;
		std::cout << "Maximum number of work groups (Z): " << workGroupCount[2] << std::endl;

		GLint workGroupSize[3];
		glGetIntegeri_v(GL_MAX_COMPUTE_WORK_GROUP_SIZE, 0, &workGroupSize[0]); // X dimension
		glGetIntegeri_v(GL_MAX_COMPUTE_WORK_GROUP_SIZE, 1, &workGroupSize[1]); // Y dimension
		glGetIntegeri_v(GL_MAX_COMPUTE_WORK_GROUP_SIZE, 2, &workGroupSize[2]); // Z dimension

		std::cout << "Maximum work group size (X): " << workGroupSize[0] << std::endl;
		std::cout << "Maximum work group size (Y): " << workGroupSize[1] << std::endl;
		std::cout << "Maximum work group size (Z): " << workGroupSize[2] << std::endl;
		*/
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

		// Swap buffers and poll IO events
		glfwSwapBuffers(m_pWindow);
		glfwPollEvents();
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


