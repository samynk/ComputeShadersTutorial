#include "GLImage.h"
#include <iostream>
#include <vector>

#define STB_IMAGE_IMPLEMENTATION
#include <stb_image.h>
#define STB_IMAGE_WRITE_IMPLEMENTATION
#include <stb_image_write.h>

GLImage::GLImage(GLuint binding, GLuint width, GLuint height, GLenum accessType)
	:m_ImageID{ 0 },
	m_Binding{ binding },
	m_Width{ width },
	m_Height{ height },
	m_AccessType(accessType),
	m_Empty{ true }
{
}

GLImage::GLImage(GLuint bindingId, GLenum accesType, const std::string& fileLocation)
	:m_ImageID{ 0 },
	m_Binding{ bindingId },
	m_Width{ 0 },
	m_Height{ 0 },
	m_AccessType{ accesType },
	m_TextureLocation{ fileLocation },
	m_Empty{ false }
{
}

GLImage::~GLImage()
{
	if (m_ImageID != 0) {
		glDeleteTextures(1, &m_ImageID);
		m_ImageID = 0;
	}
}

GLImage::GLImage(GLImage&& other) noexcept
	: m_Binding(other.m_Binding),
	m_Width(other.m_Width),
	m_Height(other.m_Height),
	m_AccessType(other.m_AccessType),
	m_ImageID(other.m_ImageID)
{
	other.m_ImageID = 0;
}

GLImage& GLImage::operator=(GLImage&& other) noexcept
{
	if (this != &other) {
		// Delete existing texture
		if (m_ImageID != 0) {
			glDeleteTextures(1, &m_ImageID);
		}

		// Move data
		m_Binding = other.m_Binding;
		m_Width = other.m_Width;
		m_Height = other.m_Height;
		m_AccessType = other.m_AccessType;
		m_ImageID = other.m_ImageID;

		// Reset the other object
		other.m_ImageID = 0;
	}
	return *this;
}

void GLImage::init() {
	if (m_Empty) {
		initEmpty();
	}
	else {
		initTexture();
	}
}

void GLImage::initEmpty() {
	glGenTextures(1, &m_ImageID);
	if (m_ImageID == 0) {
		throw std::runtime_error("Failed to generate texture.");
	}

	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, m_ImageID);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);

	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, m_Width, m_Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, nullptr);

	GLenum error = glGetError();
	if (error != GL_NO_ERROR)
	{
		throw std::runtime_error("OpenGL Error in GLImage::GLImage(): " + std::to_string(error));
	}
}

void GLImage::initTexture() {
	int width, height, nrChannels;
	unsigned char* data = stbi_load(m_TextureLocation.c_str(), &width, &height, &nrChannels, 0);
	if (data == nullptr) {
		throw std::runtime_error("Failed to load texture image from " + m_TextureLocation);
	}

	m_Width = width;
	m_Height = height;
	glGenTextures(1, &m_ImageID);

	if (m_ImageID == 0) {
		stbi_image_free(data);
		throw std::runtime_error("Failed to generate texture.");
	}

	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, m_ImageID);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);

	// Determine the format based on the number of channels
	GLenum format = GL_RGB;
	switch (nrChannels) {
	case 1:
		format = GL_RED;
		break;
	case 3:
		format = GL_RGB;
		break;
	case 4:
		format = GL_RGBA;
		break;
	default:
		stbi_image_free(data);
		throw std::runtime_error("Unsupported number of channels in texture image.");
	}

	// Upload texture data
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, m_Width, m_Height, 0, format, GL_UNSIGNED_BYTE, data);

	GLenum error = glGetError();
	if (error != GL_NO_ERROR)
	{
		stbi_image_free(data);
		throw std::runtime_error("OpenGL Error in GLImage::GLImage(file): " + std::to_string(error));
	}

	stbi_image_free(data);
	glBindTexture(GL_TEXTURE_2D, 0);
}

void GLImage::bindAsCompute() const
{
	glBindImageTexture(m_Binding, m_ImageID, 0, GL_FALSE, 0, m_AccessType, GL_RGBA8);
	GLenum error = glGetError();
	if (error != GL_NO_ERROR)
	{
		throw std::runtime_error("OpenGL Error in GLImage::bind(): " + std::to_string(error));
	}
}

void GLImage::bindAsCompute(GLuint bindingSlot) const
{
	glBindImageTexture(bindingSlot, m_ImageID, 0, GL_FALSE, 0, m_AccessType, GL_RGBA8);
	GLenum error = glGetError();
	if (error != GL_NO_ERROR)
	{
		throw std::runtime_error("OpenGL Error in GLImage::bind(): " + std::to_string(error));
	}
}

void GLImage::bindAsTexture() const
{
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, m_ImageID);

}

void GLImage::write(const std::string& fileLocation) const
{
	glBindTexture(GL_TEXTURE_2D, m_ImageID);

	GLint width, height;
	glGetTexLevelParameteriv(GL_TEXTURE_2D, 0, GL_TEXTURE_WIDTH, &width);
	glGetTexLevelParameteriv(GL_TEXTURE_2D, 0, GL_TEXTURE_HEIGHT, &height);

	GLint nrChannels = 4; // Assuming RGBA
	std::vector<unsigned char> data(width * height * nrChannels);

	glGetTexImage(GL_TEXTURE_2D, 0, GL_RGBA, GL_UNSIGNED_BYTE, data.data());

	GLenum error = glGetError();
	if (error != GL_NO_ERROR)
	{
		glBindTexture(GL_TEXTURE_2D, 0);
		throw std::runtime_error("OpenGL Error in GLImage::write(): " + std::to_string(error));
	}

	if (!stbi_write_png(fileLocation.c_str(), width, height, nrChannels, data.data(), width * nrChannels)) {
		glBindTexture(GL_TEXTURE_2D, 0);
		throw std::runtime_error("Failed to write image to file: " + fileLocation);
	}

	glBindTexture(GL_TEXTURE_2D, 0);
}
