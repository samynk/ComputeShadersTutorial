#version 430 core

layout (local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

layout(std430, binding = 0) buffer cameraRays {
    vec4 rayDirections[];
};

layout(binding = 0, rgba8) writeonly uniform image2D outputTexture;

uniform ivec2 imgDimension;
uniform float scaleFactor = 1;

void main() {
    ivec2 gid = ivec2(gl_GlobalInvocationID.xy);  // Get global workgroup ID
    // get the size of the image.
    ivec2 size = imageSize(outputTexture);
    // Compute the original array position based on the scale factor
    int arrayIndex = gid.y * size.x  + gid.x; 

    // Fetch the value from the SSBO
    vec3 vector = rayDirections[arrayIndex].xyz + vec3(1,1,1);

    // Write the value to the output texture
    imageStore(outputTexture, gid, vec4( vector/2,1) );
}