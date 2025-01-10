#version 430 core

layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

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

struct Sphere{
    vec3 loc;
    float radius;
    vec4 color;
};

uniform int nrOfSpheres;
layout(std430, binding = 1) buffer SphereBuffer {
    Sphere spheres[];
};

shared Sphere sharedSpheres[4]; // Assuming 4 spheres, to be shared across threads

uniform ivec2 imgDimension;
uniform float maxT = 1000.0f; // Far value

vec3 lightPos = vec3(-0.25,3,0.2);

void main() {
    ivec2 gid = ivec2(gl_GlobalInvocationID.xy);
    uint index = gid.y * imgDimension.x + gid.x;

	if ( rays[index].nullRay){
		return;
	}

	

	// Load sphere data into shared memory
	if (gl_LocalInvocationID.x == 0 && gl_LocalInvocationID.y == 0) {
		for (int i = 0; i < nrOfSpheres; ++i) {
			sharedSpheres[i] = spheres[i];
		}
	}
	barrier(); // Ensure all threads see the loaded data
	
	vec3 rayOrigin = rays[index].originAndT.xyz;
	vec3 rayDirection = rays[index].direction.xyz;
	float currentT = rays[index].originAndT.w;
	
	bool write = false;
	vec4 color = vec4(0,0,0,1);

	vec3 currentPos =  vec3(0,0,0);
	vec3 currentNormal = vec3(0,0,0);
	bool found = false;
	for(int si = 0; si < nrOfSpheres; ++si)
	{
		vec3 sphereLoc = sharedSpheres[si].loc;
		float r = sharedSpheres[si].radius;

		vec3 raySphereDiff = sphereLoc - rayOrigin; // take origin of ray (camera into account)
		float L2 = dot(raySphereDiff, raySphereDiff);
		float tca = dot(raySphereDiff, rayDirection);
		float od2 = L2 - tca * tca;
		
		float r2 = r*r;
		if (od2 < r2) {
			float thc = sqrt(r2 - od2);
			float t0 = tca - thc;
			if ( t0 > 0 && t0 < currentT) {
				currentPos = t0 * rayDirection + rayOrigin;
				currentNormal = (currentPos - sphereLoc) / r;
			
				vec3 lightVec = normalize ( lightPos - currentPos );
				float Id = clamp(dot(lightVec,currentNormal),0.1,1);

				color = vec4(Id * spheres[si].color.rgb,1);
				currentT = t0;
				found = true;
			}
		}
	}
	
	rays[index].originAndT.xyz = currentPos + 0.001*rays[index].direction.xyz;
	rays[index].originAndT.w = maxT;
	rays[index].direction.xyz =  rayDirection - 2*dot(currentNormal, rayDirection)*currentNormal ;
	rays[index].rayHits += int(found);
	rays[index].color.rgb += color.rgb;
	rays[index].color.a = 1;
	rays[index].nullRay = !found;	
}