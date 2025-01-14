#version 430 core

layout (local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

uniform float fieldOfView = 1;
uniform ivec2 imgDimension;
uniform vec3 cameraPosition;
uniform mat4 cameraMatrix;

struct Ray{
	vec3 origin;
	vec3 direction;
};

layout(std430, binding = 0) buffer cameraRays {
    Ray rays[];
};

void main()
{
	ivec2 gid = ivec2(gl_GlobalInvocationID.xy);
	uint index = imgDimension.x * gid.y + gid.x;



	float aspectRatio = imgDimension.x * 1.0f / imgDimension.y;
	
	float xcs = (2.0f * (gid.x + 0.5f) / imgDimension.x - 1.0f) * aspectRatio * fieldOfView;
	float ycs = (1.0f - 2.0f * (gid.y + 0.5f) / imgDimension.y) * fieldOfView;
	vec3 ray = normalize(vec3(xcs, ycs, 1.0f));
	mat3 rotMatrix = mat3(cameraMatrix);
	
	rays[index].origin = cameraPosition;
	rays[index].direction = rotMatrix *  ray;
}
	