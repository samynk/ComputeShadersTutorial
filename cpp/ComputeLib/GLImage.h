#pragma once
#include <GL/glew.h>
#include <string>

class GLImage final
{
public:
    GLImage(GLuint bindingId, GLuint width, GLuint height, GLenum accessType);
    GLImage(GLuint bindingId, GLenum accessType, const std::string& fileLocation);
    ~GLImage();

    // Delete copy constructor and copy assignment to prevent copying
    GLImage(const GLImage&) = delete;
    GLImage& operator=(const GLImage&) = delete;

    // Allow move semantics
    GLImage(GLImage&& other) noexcept;
    GLImage& operator=(GLImage&& other) noexcept;

    void init();
    
    void bindAsCompute() const;
    void bindAsCompute(GLuint bindingSlot) const;
    void bindAsTexture() const;
    void write(const std::string& fileLocation) const;

private:
    void initEmpty();
    void initTexture();

    std::string m_TextureLocation;
    bool m_Empty = true;

    GLuint m_Binding = 0;
    GLuint m_Width = 0;
    GLuint m_Height = 0;
    GLenum m_AccessType;

    // Resource that should be released
    GLuint m_ImageID = 0;
};
