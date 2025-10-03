#version 430 core

#define LOCAL_SIZE 16
#define WORKGROUP_SIZE LOCAL_SIZE*LOCAL_SIZE
#define MAX_LOCAL_CANDIDATES  128


layout(local_size_x = LOCAL_SIZE, local_size_y = LOCAL_SIZE, local_size_z = LOCAL_SIZE) in;

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

struct Vertex{
    vec3 loc;
    vec3 normal;
    vec2 texCoord;
};

uniform int nrOfSpheres;
layout(std430, binding = 1) buffer VertexBuffer {
    Vertex vertices[];
};

layout(std430, binding = 2) buffer IndexBuffer {
    int indices[];
};

uniform ivec2 imgDimension;
uniform int nrOfTriangles;
uniform float maxT = 1000.0f; // Far value

vec3 lightPos = vec3(-0.25,5,-1);


bool intersectRayTriangleMT(vec3 orig, vec3 dir, vec3 v0, vec3 v1, vec3 v2, out float t)
{
    // Epsilon for floating point comparison
    float EPSILON = 0.000001;

    // Find vectors for two edges sharing v0
    vec3 edge1 = v1 - v0;
    vec3 edge2 = v2 - v0;

    // Begin calculating determinant - also used to calculate u parameter
    vec3 h = cross(dir, edge2);
    float a = dot(edge1, h);

    // If the determinant is close to 0, the ray lies in the plane of the triangle
    if (a > -EPSILON && a < EPSILON) 
        return false; // This means ray is parallel to the triangle.

    float f = 1.0 / a;
    vec3 s = orig - v0;
    float u = f * dot(s, h);

    // Check if the intersection is outside the triangle
    if (u < 0.0 || u > 1.0) 
        return false;

    // Calculate v parameter and test bounds
    vec3 q = cross(s, edge1);
    float v = f * dot(dir, q);

    // Check if the intersection is outside the triangle
    if (v < 0.0 || u + v > 1.0) 
        return false;

    // Calculate the distance along the ray to the intersection point
    t = f * dot(edge2, q);

    // If t is less than 0, the triangle is behind the ray
    if (t > EPSILON) {
        return true;
    }
    
    return false; // This means there is a line intersection but not a ray intersection.
}

int signbit(float x) {
    // reinterpret the float’s bits as unsigned, shift the MSB into LSB
    return int(floatBitsToUint(x) >> 31u);
}

// vector: per-component 0/1
ivec4 signbit(vec4 v) {
    uvec4 bits = floatBitsToUint(v);
    return ivec4(bits >> 31u);
}

bool intersectRayTriangleGA(vec3 orig, vec3 d, vec3 A, vec3 B, vec3 C, out vec4 V)
{
    // Epsilon for floating point comparison
	float EPSILON = 1e-6;

    // Find vectors for two edges sharing v0
    vec3 a = A-orig;
    vec3 b = B-orig;
    vec3 c = C-orig;

    const vec3 aob = cross(a,b);
	const vec3 cod = cross(d,c);

	V.w = dot( aob, c);
	V.z = dot( aob, d) ;
	V.x = -dot( cod, b);
	V.y =  dot( cod, a);

	ivec4 Vs = signbit(V);
			
	return ( (Vs.x+Vs.y+Vs.z+Vs.w) == 0);

}


    // Shared buffers per work-group
shared uint localCount[WORKGROUP_SIZE];
shared uint localCands[WORKGROUP_SIZE][MAX_LOCAL_CANDIDATES];
shared vec4 localVolumes[WORKGROUP_SIZE][MAX_LOCAL_CANDIDATES];

void main() {
    uint index = gl_GlobalInvocationID.y * imgDimension.x + gl_GlobalInvocationID.x;

	if ( rays[index].nullRay){
		return;
	}

	 // Initialize the per-ray local count
	uint localIndex = gl_LocalInvocationID.x * LOCAL_SIZE + gl_LocalInvocationID.y;
	
    localCount[localIndex] = 0;
    barrier();
	
	vec3 rayOrigin = rays[index].originAndT.xyz;
	vec3 rayDirection = rays[index].direction.xyz;
	

	uint startSi = gl_GlobalInvocationID.z * LOCAL_SIZE ;
	for(int si = 0; si < nrOfTriangles; ++si)
	{
		int i1 = indices[si*3];
		int i2 = indices[si*3+1];
		int i3 = indices[si*3+2];

		vec3 p1 = vertices[i1].loc;
		vec3 p2 = vertices[i2].loc;
		vec3 p3 = vertices[i3].loc;
		
		vec4 V;
		if( intersectRayTriangleGA(rayOrigin, rayDirection, p1, p2, p3, V) )
		{		
			uint slot = atomicAdd(localCount[localIndex],1);
			if (slot < MAX_LOCAL_CANDIDATES) {
				localCands[localIndex][slot] = si;
				localVolumes[localIndex][slot] = V;
			}
		}
	}
	barrier();

	vec3 normal;
	vec3 currentPos =  vec3(0,0,0);
	float currentT = rays[index].originAndT.w;
	uint closestTri = -1;
	for(int si = 0; si < localCount[localIndex]; ++si)
	{
		vec4 V = localVolumes[localIndex][si];
		float invRayVol =1.0f/( V.x+V.y+V.z);
		float t = V.w * invRayVol;
		if ( t < currentT )
		{
			currentT = t;
			closestTri = si;
		}
	}

	vec4 color = vec4(0,0,0,1);
	if (closestTri > -1)
	{
		int i1 = indices[closestTri*3];
		int i2 = indices[closestTri*3+1];
		int i3 = indices[closestTri*3+2];

		vec3 p1 = vertices[i1].loc;
		vec3 p2 = vertices[i2].loc;
		vec3 p3 = vertices[i3].loc;

		normal = normalize(cross(p3-p1, p2-p1));
		vec3 currentPos = rayOrigin + currentT * rayDirection;
		vec3 lightVec = normalize ( lightPos - currentPos );
		float Id = clamp(dot(lightVec,normal),0.1,1);
		// hardcoded color.
		color = vec4(Id * vec3(1,0.25,1),1);

		rays[index].originAndT.xyz = currentPos + 0.001*rays[index].direction.xyz;
		rays[index].originAndT.w = maxT;
		rays[index].direction.xyz =  rayDirection - 2*dot(normal, rayDirection)*normal ;
		rays[index].rayHits++;
		rays[index].color.rgb += color.rgb;
		rays[index].color.a = 1;
		rays[index].nullRay = false;
	}else{
		rays[index].nullRay = true;
	}
}