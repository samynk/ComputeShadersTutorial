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


bool intersectRayTriangle(vec3 orig, vec3 dir, vec3 v0, vec3 v1, vec3 v2, out float t, out vec3 intersectionPoint)
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
        // Calculate the exact intersection point
        intersectionPoint = orig + dir * t;
        return true;
    }
    
    return false; // This means there is a line intersection but not a ray intersection.
}


void main() {
    ivec2 gid = ivec2(gl_GlobalInvocationID.xy);
    uint index = gid.y * imgDimension.x + gid.x;

	if ( rays[index].nullRay){
		return;
	}
	
	vec3 rayOrigin = rays[index].originAndT.xyz;
	vec3 rayDirection = rays[index].direction.xyz;
	float currentT = rays[index].originAndT.w;
	
	bool write = false;
	vec4 color = vec4(0,0,0,1);

	vec3 currentPos =  vec3(0,0,0);
	vec3 currentNormal = vec3(0,0,0);
	bool found = false;
	for(int si = 0; si < nrOfTriangles; ++si)
	{
		int i1 = indices[si*3];
		int i2 = indices[si*3+1];
		int i3 = indices[si*3+2];

		vec3 p1 = vertices[i1].loc;
		vec3 p2 = vertices[i2].loc;
		vec3 p3 = vertices[i3].loc;

		float t0;
		
		if( intersectRayTriangle(rayOrigin, rayDirection, p1, p2, p3, t0, currentPos) )
			currentNormal = normalize(cross(p3-p1,p2-p1));
			if ( dot(currentNormal,rayDirection) < 0 && t0 < currentT) {
				vec3 lightVec = normalize ( lightPos - currentPos );
				float Id = clamp(dot(lightVec,currentNormal),0.1,1);
				// hardcoded color.
				color = vec4(Id * vec3(1,0,1),1);
				currentT = t0;
				found = true;
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