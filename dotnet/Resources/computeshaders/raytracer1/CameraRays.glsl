#version 430 core

layout (local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

uniform float fieldOfView = 1;
uniform ivec2 imgDimension;

layout(std430, binding = 0) buffer cameraRays {
    vec4 rayDirections[];
};

void main()
{
	ivec2 gid = ivec2(gl_GlobalInvocationID.xy);

	float aspectRatio = imgDimension.x * 1.0f / imgDimension.y;
	

	float xcs = (2.0f * (gid.x + 0.5f) / imgDimension.x - 1.0f) * aspectRatio * fieldOfView;
	float ycs = (1.0f - 2.0f * (gid.y + 0.5f) / imgDimension.y) * fieldOfView;
	vec3 ray = normalize(vec3(xcs, ycs, 1.0f));

	rayDirections[imgDimension.x * gid.y + gid.x].xyz = ray;
}
	