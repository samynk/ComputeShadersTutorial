#include "ComputeShader.h"
#include <fstream>
#include <iostream>
#include <sstream>
#include <stdexcept>
#include <vector>

#include <glm/gtc/type_ptr.hpp>

ComputeShader::ComputeShader(const std::string& fileLocation) :m_FileLocation{ fileLocation }
{
	std::ifstream inputFile{ m_FileLocation };

	if (!inputFile.is_open()) {
		throw std::runtime_error("Error: Could not open the shader file " + m_FileLocation);
	}

	// Read the entire file into a string
	std::stringstream buffer;
	buffer << inputFile.rdbuf();
	m_ShaderContents = buffer.str();
	m_SourceLength = m_ShaderContents.length();
	m_SourceValid = true;
	inputFile.close();
}

ComputeShader::~ComputeShader()
{
	if (m_ShaderID != 0) {
		glDeleteShader(m_ShaderID);
		m_ShaderID = 0;
	}
	if (m_ComputeProgramID != 0) {
		glDeleteProgram(m_ComputeProgramID);
		m_ComputeProgramID = 0;
	}
}

ComputeShader::ComputeShader(ComputeShader&& other) noexcept
	: m_FileLocation(std::move(other.m_FileLocation)),
	m_SourceValid(other.m_SourceValid),
	m_ShaderContents(std::move(other.m_ShaderContents)),
	m_SourceLength(other.m_SourceLength),
	m_ShaderID(other.m_ShaderID),
	m_ComputeProgramID(other.m_ComputeProgramID)
{
	other.m_ShaderID = 0;
	other.m_ComputeProgramID = 0;
	other.m_SourceValid = false;
}

ComputeShader& ComputeShader::operator=(ComputeShader&& other) noexcept
{
	if (this != &other) {
		// Clean up existing resources
		if (m_ShaderID != 0) {
			glDeleteShader(m_ShaderID);
		}
		if (m_ComputeProgramID != 0) {
			glDeleteProgram(m_ComputeProgramID);
		}

		// Move data
		m_FileLocation = std::move(other.m_FileLocation);
		m_SourceValid = other.m_SourceValid;
		m_ShaderContents = std::move(other.m_ShaderContents);
		m_SourceLength = other.m_SourceLength;
		m_ShaderID = other.m_ShaderID;
		m_ComputeProgramID = other.m_ComputeProgramID;

		// Reset the other object
		other.m_ShaderID = 0;
		other.m_ComputeProgramID = 0;
		other.m_SourceValid = false;
	}
	return *this;
}

void ComputeShader::compile()
{
	if (m_SourceValid)
	{
		m_ShaderID = glCreateShader(GL_COMPUTE_SHADER);
		const char* computeShaderSource = m_ShaderContents.c_str();

		glShaderSource(m_ShaderID, 1, &computeShaderSource, &m_SourceLength);
		glCompileShader(m_ShaderID);

		// Check for compilation errors
		GLint success;
		glGetShaderiv(m_ShaderID, GL_COMPILE_STATUS, &success);
		if (!success) {
			// Retrieve and log the error message
			std::vector<char> infoLog(1024);
			glGetShaderInfoLog(m_ShaderID, static_cast<GLsizei>(infoLog.size()), nullptr, infoLog.data());
			std::string errorMessage(infoLog.begin(), infoLog.end());
			throw std::runtime_error("ERROR::SHADER::COMPUTE::COMPILATION_FAILED: " + m_FileLocation + "\n"
				+ errorMessage);
		}

		// Link the shader into a program
		m_ComputeProgramID = glCreateProgram();
		glAttachShader(m_ComputeProgramID, m_ShaderID);
		glLinkProgram(m_ComputeProgramID);

		// Check for linking errors
		glGetProgramiv(m_ComputeProgramID, GL_LINK_STATUS, &success);
		if (!success) {
			std::vector<char> infoLog(1024);
			glGetProgramInfoLog(m_ComputeProgramID, static_cast<GLsizei>(infoLog.size()), nullptr, infoLog.data());
			std::string errorMessage(infoLog.begin(), infoLog.end());
			throw std::runtime_error("ERROR::PROGRAM::COMPUTE::LINKING_FAILED\n" + errorMessage);

		}
		
		// Define an array to hold the work group sizes
		GLint workGroupSize[3];

		// Query the compute shader program for the work group sizes
		glGetProgramiv(m_ComputeProgramID, GL_COMPUTE_WORK_GROUP_SIZE, workGroupSize);

		m_LocalSizeX = workGroupSize[0];
		m_LocalSizeY = workGroupSize[1];
		m_LocalSizeZ = workGroupSize[2];

		// Shader has been linked with the compute program, so it's no longer necessary.
		glDetachShader(m_ComputeProgramID, m_ShaderID);
		glDeleteShader(m_ShaderID);
		m_ShaderID = 0;
	}
}

void ComputeShader::use() const
{
	glUseProgram(m_ComputeProgramID);
	GLenum error = glGetError();
	if (error != GL_NO_ERROR)
	{
		throw std::runtime_error("OpenGL Error in ComputeShader::use(): " + std::to_string(error));
	}
}

void ComputeShader::compute(GLuint totalWidth, GLuint  totalHeight) const
{
	// Calculate the number of workgroups
	GLuint workGroupSizeX = (GLuint)ceil(totalWidth * 1.0f / m_LocalSizeX);
	GLuint workGroupSizeY = (GLuint)ceil(totalHeight * 1.0f / m_LocalSizeY);

	// Dispatch compute shader
	glDispatchCompute(workGroupSizeX, workGroupSizeY, 1);
	GLenum error = glGetError();
	if (error != GL_NO_ERROR)
	{
		throw std::runtime_error("OpenGL Error in ComputeShader::dispatchCompute(): " + std::to_string(error));
	}
}

GLint ComputeShader::getParameterLocation(const std::string& parameterName)
{
	return glGetUniformLocation(m_ComputeProgramID, parameterName.c_str());
}

void ComputeShader::setUniformBool(GLint paramLoc, GLboolean value)
{
	glUniform1i(paramLoc, value);
}

void ComputeShader::setUniformInteger(GLint paramLoc, GLint value)
{
	glUniform1i(paramLoc, value);
}

void ComputeShader::setUniformInteger2(GLint paramLoc, GLint x, GLint y)
{
	glUniform2i(paramLoc, x, y);
}

void ComputeShader::setUniformFloat(GLint paramLoc, GLfloat value)
{
	glUniform1f(paramLoc, value);
}

void ComputeShader::setUniformFloat3(GLint paramLoc, GLfloat x, GLfloat y, GLfloat z)
{
	glUniform3f(paramLoc, x, y, z);
}

void ComputeShader::setUniformMatrix(GLint paramLoc, const glm::mat4& matrix) {
	glUniformMatrix4fv(paramLoc, 1, GL_FALSE, glm::value_ptr(matrix));
	
}
