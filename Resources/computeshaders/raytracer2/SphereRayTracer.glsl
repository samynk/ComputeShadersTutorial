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

void main() {
    ivec2 gid = ivec2(gl_GlobalInvocationID.xy);
    uint index = gid.y * imgDimension.x + gid.x;
	
	vec3 rayOrigin = rays[index].origin;
	vec3 rayDirection = rays[index].direction;
	
	bool write = false;
	vec4 color = vec4(0,0,0,1);
	for(int si = 0; si < nrOfSpheres; ++si)
	{
		vec3 sphereLoc = spheres[si].loc;
		float r = spheres[si].radius;

		vec3 raySphereDiff = sphereLoc - rayOrigin; // take origin of ray (camera into account)
		float L2 = dot(raySphereDiff, raySphereDiff);
		float tca = dot(raySphereDiff, rayDirection);
		float od2 = L2 - tca * tca;
		
		float r2 = r*r;
		if (od2 < r2) {
			float thc = sqrt(r2 - od2);
			float t0 = tca - thc;
			if ( t0 > 0 && t0 < tBuffer[index] ) {
				vec3 currentPos = t0 * rayDirection + rayOrigin;
				vec3 currentNormal = (currentPos - sphereLoc) / r;
				color = spheres[si].color;
				tBuffer[index] = t0;
			}
		}
	}
	
	// Write the value to the output texture
	imageStore(outputTexture, gid, color) ;
	
}