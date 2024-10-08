#pragma once

#include <concepts>
#include <GL/glew.h>

class SurfaceRenderer;  // Assume this is defined elsewhere

template<typename T>

concept HasCompute = 
    std::constructible_from<T, GLuint, GLuint> 
    && 
    requires(T obj, const SurfaceRenderer & renderer) {
        { obj.init(renderer) } -> std::same_as<void>;
        { obj.compute(renderer) } -> std::same_as<void>;
    };
