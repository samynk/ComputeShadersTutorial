#version 430 core
layout (local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

layout(binding = 0, rgba8) readonly uniform image2D inputImage;
layout(binding = 1, rgba8) writeonly uniform image2D outputImage;

const float kernel[7] = float[](0.006,0.061, 0.242, 0.383, 0.242, 0.061,0.006);
uniform bool horizontal;

void main() {
	
	ivec2 gid = ivec2(gl_GlobalInvocationID.xy);
    vec3 color = vec3(0,0,0);
	
    for (int offset = -3; offset <= 3; ++offset) {
		ivec2 coord;
		if (horizontal) {
            coord = gid + ivec2(offset, 0);
        } else {
            coord = gid + ivec2(0, offset);
        }
        color += imageLoad(inputImage, coord).xyz * kernel[offset + 3];
    }
	
    imageStore(outputImage, gid, vec4(color,1));
}
