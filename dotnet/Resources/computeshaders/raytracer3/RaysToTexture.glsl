#version 430 core

layout (local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

struct Ray{
	vec4 originAndT;
	vec4 direction;
	vec4 color;        
	uint rayHits;
	bool nullRay;
};

layout(std430, binding = 0) buffer cameraRays {
    Ray rays[];
};

layout(binding = 0, rgba8) writeonly uniform image2D outputTexture;
uniform ivec2 imgDimension;

void main() {
    ivec2 gid = ivec2(gl_GlobalInvocationID.xy);  // Get global workgroup ID
    int index = gid.y * imgDimension.x  + gid.x; 

    // Write the value to the output texture
	vec3 color = rays[index].color.rgb/rays[index].rayHits;
    imageStore(outputTexture, gid, vec4(color,1));
}