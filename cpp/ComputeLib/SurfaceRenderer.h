#pragma once

#include <string>
#include "GLImage.h"

class SurfaceRenderer
{

public:
	SurfaceRenderer(GLuint bindingId, GLuint width, GLuint height, const std::string& vertexShader, const std::string fragmentShader);
    ~SurfaceRenderer();

    // not meant to be copied or moved
     // Delete copy constructor and copy assignment operator
    SurfaceRenderer(const SurfaceRenderer&) = delete;
    SurfaceRenderer& operator=(const SurfaceRenderer&) = delete;

    // Delete move constructor and move assignment operator
    SurfaceRenderer(SurfaceRenderer&&) = delete;
    SurfaceRenderer& operator=(SurfaceRenderer&&) = delete;

    void init();
    void bindAsCompute() const;
    void bindAsCompute(GLuint bindingSlot) const;
    void drawQuadWithTexture();

    GLuint getWidth() const {
        return m_Width;
    }
    GLuint getHeight() const {
        return m_Height;
    }

private:
    GLuint compileShader(const std::string& shaderSource, GLenum shaderType) const;
    std::string loadShaderSource(const std::string& shaderFile) const;
    void createShaderProgram(const std::string& vertexShader, const std::string fragmentShader);
    void setupQuad();

    std::string m_VertexShaderFile;
    std::string m_FragmentShaderFile;
    GLint m_screenTextureLoc{ 0 };

    float quadVertices[24] = {
        // Positions   // TexCoords
        -1.0f,  1.0f,  0.0f, 0.0f, // Top-left
        -1.0f, -1.0f,  0.0f, 1.0f, // Bottom-left
         1.0f, -1.0f,  1.0f, 1.0f, // Bottom-right

        -1.0f,  1.0f,  0.0f, 0.0f, // Top-left
         1.0f, -1.0f,  1.0f, 1.0f, // Bottom-right
         1.0f,  1.0f,  1.0f, 0.0f  // Top-right
    };
    GLuint m_Width;
    GLuint m_Height;

    // Automatically released.
    GLImage m_FullScreenImage;
    // To release in destructor.
    GLuint m_ProgramID;
    GLuint m_VertexArrayObject;
    GLuint m_VertexBufferObject;
};