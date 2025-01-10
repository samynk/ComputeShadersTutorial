#include "SurfaceRenderer.h"
#include <vector>
#include <stdexcept>
#include <fstream>
#include <sstream>


SurfaceRenderer::SurfaceRenderer(GLuint bindingId, GLuint w, GLuint h, const std::string& vertexShader, const std::string fragmentShader)
	:
	m_VertexShaderFile{vertexShader},
	m_FragmentShaderFile{fragmentShader},
	m_FullScreenImage{bindingId,w,h,GL_READ_WRITE},
	m_ProgramID{ 0 },
	m_VertexArrayObject{ 0 },
	m_VertexBufferObject{ 0 },
	m_Width{w},
	m_Height{h}
{
}

SurfaceRenderer::~SurfaceRenderer()
{
	if (m_ProgramID != 0) {
		glDeleteProgram(m_ProgramID);
		m_ProgramID = 0;
	}
	if (m_VertexArrayObject != 0)
	{
		glDeleteVertexArrays(1, &m_VertexArrayObject);
		m_VertexArrayObject = 0;
	}
}

void SurfaceRenderer::init()
{
	createShaderProgram(m_VertexShaderFile, m_FragmentShaderFile);
	m_FullScreenImage.init();
	setupQuad();
	
}

void SurfaceRenderer::bindAsCompute() const
{
	m_FullScreenImage.bindAsCompute();
}

void SurfaceRenderer::bindAsCompute(GLuint bindingSlot) const
{
	m_FullScreenImage.bindAsCompute(bindingSlot);
}

void SurfaceRenderer::createShaderProgram(const std::string& vertexShader, const std::string fragmentShader)
{
	GLuint vertexShaderID = compileShader(loadShaderSource(vertexShader), GL_VERTEX_SHADER);
	GLuint fragmentShaderID = compileShader(loadShaderSource(fragmentShader), GL_FRAGMENT_SHADER);

	// Link shaders into a program
	m_ProgramID = glCreateProgram();
	glAttachShader(m_ProgramID, vertexShaderID);
	glAttachShader(m_ProgramID, fragmentShaderID);
	glLinkProgram(m_ProgramID);

	// Check for linking errors
	GLint success;
	glGetProgramiv(m_ProgramID, GL_LINK_STATUS, &success);
	if (!success) {
		std::vector<char> infoLog(1024);
		glGetProgramInfoLog(m_ProgramID, 1024, nullptr, infoLog.data());
		throw std::runtime_error("ERROR::PROGRAM::LINKING_FAILED\n" + std::string(infoLog.begin(), infoLog.end()));
	}
	m_screenTextureLoc = glGetUniformLocation(m_ProgramID, "screenTexture");
	// Shaders can be deleted after linking
	glDeleteShader(vertexShaderID);
	glDeleteShader(fragmentShaderID);
}

void SurfaceRenderer::setupQuad()
{
	glGenVertexArrays(1, &m_VertexArrayObject);
	glGenBuffers(1, &m_VertexBufferObject);

	glBindVertexArray(m_VertexArrayObject);

	glBindBuffer(GL_ARRAY_BUFFER, m_VertexBufferObject);
	glBufferData(GL_ARRAY_BUFFER, 24*sizeof(float), &quadVertices, GL_STATIC_DRAW);

	// Position attribute
	glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 4 * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
	// Texture coordinate attribute
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 4 * sizeof(float), (void*)(2 * sizeof(float)));
	glEnableVertexAttribArray(1);

	glBindVertexArray(0);
	glBindBuffer(GL_ARRAY_BUFFER, 0);
}

void SurfaceRenderer::drawQuadWithTexture()
{
	// Set the texture uniform
	glUseProgram(m_ProgramID);
	glUniform1i(m_screenTextureLoc, 0);
	m_FullScreenImage.bindAsTexture();

	// Draw the quad
	glBindVertexArray(m_VertexArrayObject);
	glDrawArrays(GL_TRIANGLES, 0, 6);
	glBindVertexArray(0);
}

GLuint SurfaceRenderer::compileShader(const std::string& shaderSource, GLenum shaderType) const
{
	GLuint shaderID = glCreateShader(shaderType);
	const char* computeShaderSource = shaderSource.c_str();
	GLint length = shaderSource.length();

	glShaderSource(shaderID, 1, &computeShaderSource, &length);
	glCompileShader(shaderID);

	// Check for compilation errors
	GLint success;
	glGetShaderiv(shaderID, GL_COMPILE_STATUS, &success);
	if (!success) {
		// Retrieve and log the error message
		std::vector<char> infoLog(1024);
		glGetShaderInfoLog(shaderID, static_cast<GLsizei>(infoLog.size()), nullptr, infoLog.data());
		std::string errorMessage(infoLog.begin(), infoLog.end());
		throw std::runtime_error("ERROR::SHADER::COMPUTE::COMPILATION_FAILED\n" + errorMessage);
	}

	return shaderID;
}

std::string SurfaceRenderer::loadShaderSource(const std::string& shaderFile) const
{
	std::ifstream inputFile(shaderFile);

	if (!inputFile.is_open()) {
		throw std::runtime_error("Error: Could not open the shader file " + shaderFile);
	}
	// Read the entire file into a string
	std::stringstream buffer;
	buffer << inputFile.rdbuf();
	return buffer.str();
}
