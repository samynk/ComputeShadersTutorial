#version 430 core

layout(local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

layout(std430, binding = 0) buffer TBuffer {
    float tBuffer[];
};

uniform ivec2 imgDimension;
uniform float maxDepthValue = 1000.0f; // Far value

void main() {
    uint index = gl_GlobalInvocationID.y * imgDimension.x + gl_GlobalInvocationID.x;

    if (gl_GlobalInvocationID.x < imgDimension.x && gl_GlobalInvocationID.y < imgDimension.y) {
        tBuffer[index] = maxDepthValue;
    }
}
