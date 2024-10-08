#pragma once

#include "RLEReader.h"

#include <GL/glew.h>

#include <string>
#include <vector>


template<typename T>
class ShaderStorageBufferObject
{
public:
	ShaderStorageBufferObject(GLuint size);
	ShaderStorageBufferObject(GLuint m_BindingID, GLuint width, GLuint height, GLint scaleFactor);
	ShaderStorageBufferObject(const std::string& file, GLuint m_BindingID, GLuint width, GLuint height, GLint scaleFactor);
	~ShaderStorageBufferObject();
	void init();
	GLuint getBufferWidth() const;
	GLuint getBufferHeight() const;
	GLuint getTextureWidth() const;
	GLuint getTextureHeight() const;
	GLint getScaleFactor() const;

	void bindAsCompute() const;
	void bindAsCompute(GLuint m_BindingID);
	void setAsCheckerBoard();
	void set(int index, T value);

	GLint size() const;
private:
	GLuint m_Width;
	GLuint m_Height;
	GLint m_ScaleFactor;
	GLuint m_BindingID;
	std::vector<T> m_pInputData;

	// To release
	GLuint m_SSBO_ID;
};

template <typename T>
ShaderStorageBufferObject<T>::ShaderStorageBufferObject(GLuint size)
	:ShaderStorageBufferObject<T>(0, size, 1, 1)
{

}

template <typename T>
ShaderStorageBufferObject<T>::ShaderStorageBufferObject(GLuint bindingID, GLuint width, GLuint height, GLint scaleFactor)
	:
	m_Width{ width / scaleFactor },
	m_Height{ height / scaleFactor },
	m_ScaleFactor{ scaleFactor },
	m_BindingID{ bindingID },
	m_SSBO_ID{ 0 }
{
	m_pInputData.resize(m_Width * m_Height);
}

template <typename T>
ShaderStorageBufferObject<T>::ShaderStorageBufferObject(const std::string& file, GLuint bindingID, GLuint width, GLuint height, GLint scaleFactor)
	:
	m_Width{ width / scaleFactor },
	m_Height{ height / scaleFactor },
	m_ScaleFactor{ scaleFactor },
	m_BindingID{ bindingID },
	m_SSBO_ID{ 0 }
{
	RLEReader reader{ file };
	m_pInputData.resize(m_Width * m_Height);
	reader.read(m_Width, m_Height, m_pInputData);
}

template <typename T>
ShaderStorageBufferObject<T>::~ShaderStorageBufferObject()
{
	if (m_SSBO_ID != 0)
	{
		glDeleteBuffers(1, &m_SSBO_ID);
		m_SSBO_ID = 0;
	}
}

template <typename T>
void ShaderStorageBufferObject<T>::init()
{
	glGenBuffers(1, &m_SSBO_ID);
	glBindBuffer(GL_SHADER_STORAGE_BUFFER, m_SSBO_ID);

	// Allocate memory for the SSBO and upload the data
	glBufferData(GL_SHADER_STORAGE_BUFFER, m_Width * m_Height * sizeof(T), m_pInputData.data(), GL_STATIC_DRAW);

	// Bind the SSBO to a specific binding point (e.g., binding point 0)
	glBindBufferBase(GL_SHADER_STORAGE_BUFFER, 0, m_SSBO_ID);

	// Unbind the buffer
	glBindBuffer(GL_SHADER_STORAGE_BUFFER, 0);
}

template <typename T>
GLuint ShaderStorageBufferObject<T>::getBufferWidth() const
{
	return m_Width;
}

template <typename T>
GLuint ShaderStorageBufferObject<T>::getBufferHeight() const
{
	return m_Height;
}

template <typename T>
GLint ShaderStorageBufferObject<T>::getScaleFactor() const
{
	return m_ScaleFactor;
}

template <typename T>
GLuint ShaderStorageBufferObject<T>::getTextureWidth() const
{
	return m_Width * m_ScaleFactor;
}

template <typename T>
GLuint ShaderStorageBufferObject<T>::getTextureHeight() const
{
	return m_Height * m_ScaleFactor;
}

template <typename T>
void ShaderStorageBufferObject<T>::bindAsCompute() const
{
	glBindBufferBase(GL_SHADER_STORAGE_BUFFER, m_BindingID, m_SSBO_ID);
}

template <typename T>
void ShaderStorageBufferObject<T>::bindAsCompute(GLuint bindingID)
{
	glBindBufferBase(GL_SHADER_STORAGE_BUFFER, bindingID, m_SSBO_ID);
}

template <typename T>
void ShaderStorageBufferObject<T>::setAsCheckerBoard()
{
	for (GLuint x = 0; x < m_Width; ++x)
	{
		for (GLuint y = 0; y < m_Height; ++y)
		{
			GLuint index = y * m_Width + x;
			m_pInputData[index] = (x + y) % 2;
		}
	}
}

template <typename T>
void ShaderStorageBufferObject<T>::set(int index, T value)
{
	m_pInputData[index] = value;
}

template <typename T>
GLint ShaderStorageBufferObject<T>::size() const
{
	return m_pInputData.size();
}