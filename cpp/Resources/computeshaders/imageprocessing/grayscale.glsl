#version 430 core
layout (local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

layout(binding = 0, rgba8) readonly uniform image2D inputImage;
layout(binding = 1, rgba8) writeonly uniform image2D outputImage;

void main() {
	ivec2 gid = ivec2(gl_GlobalInvocationID.xy);
    vec3 color = imageLoad(inputImage, gid).xyz;
	float gray = 0.299*color.r + 0.587*color.g + 0.114*color.z;
	// imageStore(outputImage, gid, vec4( gray, gray, gray, 1));

	// sepia filter
	float rr = (color.x * .393) + (color.y *.769) + (color.z * .189);
	float rg = (color.x * .349) + (color.y *.686) + (color.z * .168);
	float rb = (color.x * .272) + (color.y *.534) + (color.z * .131);
    imageStore(outputImage, gid, vec4( rr, rg, rb, 1));
}
