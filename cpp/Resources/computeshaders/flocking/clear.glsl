#version 430 core

layout (local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

layout(binding = 0, rgba8) writeonly uniform image2D outputTexture;

const vec4 colorMap[] = vec4[](
    vec4(0.0, 0.0, 0.0, 1.0),  // 0 -> Black
    vec4(0.0, 1.0, 0.0, 1.0),  // 1 -> Red
    vec4(0.0, 1.0, 0.0, 1.0)   // 2 -> Green
);

void main() {
    ivec2 gid = ivec2(gl_GlobalInvocationID.xy);

    // Write the value to the output texture
    imageStore(outputTexture, gid, colorMap[0]);
}