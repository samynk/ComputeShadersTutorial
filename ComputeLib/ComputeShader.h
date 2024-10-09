#pragma once
#include <string>
#include "GL/glew.h"
#include "glm/glm.hpp"

class ComputeShader
{
public:
	ComputeShader(const std::string& fileLocation);
	~ComputeShader();
	// Delete copy constructor and copy assignment to prevent copying
	ComputeShader(const ComputeShader&) = delete;
	ComputeShader& operator=(const ComputeShader&) = delete;

	// Allow move semantics
	ComputeShader(ComputeShader&& other) noexcept;
	ComputeShader& operator=(ComputeShader&& other) noexcept;
	
	void compile();
	void use() const;
	void compute(GLuint xSize, GLuint localSizeY) const;

	GLint getParameterLocation(const std::string& parameterName);
	void setUniformBool(GLint paramLoc, GLboolean value); 
	void setUniformInteger(GLint paramLoc, GLint value);
	void setUniformInteger2(GLint paramLoc, GLint x, GLint y);
	void setUniformFloat(GLint paramLoc, GLfloat x);
	void setUniformFloat3(GLint paramLoc, GLfloat x, GLfloat y, GLfloat z);
	void setUniformMatrix(GLint paramLoc, const glm::mat4& matrix);
private:
	std::string m_FileLocation;
	bool m_SourceValid{ false };
	std::string m_ShaderContents;
	GLint m_SourceLength{ 0 };
	// opengl specific --> resource that should be released
	GLuint m_ShaderID = 0;
	GLuint m_ComputeProgramID = 0;
	// reasonable default for workgroup sizes.
	GLuint m_LocalSizeX = 16;
	GLuint m_LocalSizeY = 16;
	GLuint m_LocalSizeZ = 1;
};
