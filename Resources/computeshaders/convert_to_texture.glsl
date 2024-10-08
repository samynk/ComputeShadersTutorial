#version 430 core

layout (local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

layout(std430, binding = 0) buffer InputData {
    int inputArray[];  // SSBO containing the 2D array of integers
};

layout(binding = 0, rgba8) writeonly uniform image2D outputTexture;

uniform int scaleFactor;
uniform ivec2 inputDimension;

const vec4 colorMap[] = vec4[](
    vec4(0.0, 0.0, 0.0, 1.0),  // 0 -> Black
    vec4(0.0, 1.0, 0.0, 1.0),  // 1 -> Red
    vec4(0.0, 1.0, 0.0, 1.0)   // 2 -> Green
);

void main() {
    ivec2 gid = ivec2(gl_GlobalInvocationID.xy);  // Get global workgroup ID

    // Compute the original array position based on the scale factor
    ivec2 originalPosition = gid / scaleFactor;
    int arrayIndex = originalPosition.y * inputDimension.x  + originalPosition.x;  // Assuming a 4x4 input array

    // Fetch the value from the SSBO
    int value = inputArray[arrayIndex];

    // Write the value to the output texture
    imageStore(outputTexture, gid, colorMap[value]);
}
