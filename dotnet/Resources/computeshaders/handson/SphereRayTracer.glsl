#version 430 core

layout(local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

struct Ray{
	vec3 origin;
	vec3 direction;
};

layout(std430, binding = 0) buffer cameraRays {
    Ray rays[];
};

layout(std430, binding = 1) buffer TBuffer {
    float tBuffer[];
};

struct Sphere{
    vec3 loc;
    float radius;
    vec4 color;
};

uniform int nrOfSpheres;
layout(std430, binding = 2) buffer SphereBuffer {
    Sphere spheres[];
};

layout(binding = 0, rgba8) writeonly uniform image2D outputTexture;

uniform ivec2 imgDimension;
uniform float maxDepthValue = 1000.0f; // Far value

vec3 lightPos = vec3(-0.25,3,0.2);

void main() {
    ivec2 gid = ivec2(gl_GlobalInvocationID.xy);
    
	
}