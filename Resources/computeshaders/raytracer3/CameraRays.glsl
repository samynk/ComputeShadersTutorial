#version 430 core

layout (local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

uniform float fieldOfView = 1;
uniform float maxT = 1000;
uniform ivec2 imgDimension;
uniform vec3 cameraPosition;
uniform mat4 cameraMatrix;

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

void main()
{
	ivec2 gid = ivec2(gl_GlobalInvocationID.xy);
	uint index = imgDimension.x * gid.y + gid.x;

	float aspectRatio = imgDimension.x * 1.0f / imgDimension.y;
	
	float xcs = (2.0f * (gid.x + 0.5f) / imgDimension.x - 1.0f) * aspectRatio * fieldOfView;
	float ycs = (1.0f - 2.0f * (gid.y + 0.5f) / imgDimension.y) * fieldOfView;
	vec3 ray = normalize(vec3(xcs, ycs, 1.0f));
	mat3 rotMatrix = mat3(cameraMatrix);
	
	rays[index].originAndT.xyz = cameraPosition;
	rays[index].originAndT.w = maxT;
	rays[index].direction.xyz = rotMatrix *  ray;
	rays[index].color = vec4(0,0,0.1,1);
	rays[index].rayHits = 0;
	rays[index].nullRay = false;
	
}
	