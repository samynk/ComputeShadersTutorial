#version 430 core

layout (local_size_x = 32, local_size_y = 1, local_size_z = 1) in;


layout(std430, binding = 0) buffer InputData {
    struct Boid {
        vec2 position;
        vec2 velocity;
    } boids[];
};

layout(binding = 0, rgba8) writeonly uniform image2D outputTexture;

uniform int scaleFactor = 1;
uniform ivec2 outputDimension;

const vec4 colorMap[] = vec4[](
    vec4(0.0, 0.0, 0.0, 1.0),  // 0 -> Black
    vec4(0.0, 1.0, 0.0, 1.0),  // 1 -> Red
    vec4(0.0, 162.0/255.0, 132.0/255.0, 1.0)   // 2 -> Blue
);

void main() {
    ivec2 gid = ivec2(gl_GlobalInvocationID.xy);  // Get global workgroup ID
	vec2 pos = boids[gid.x].position;
	
	ivec2 pixel = ivec2((pos.x+500)/scaleFactor, (pos.y+500)/scaleFactor);

    // Write the value to the output texture
    imageStore(outputTexture, pixel, colorMap[2]);
}