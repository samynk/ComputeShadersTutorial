// square.glsl
#version 450
// ----- workgroup size (power of two, e.g. 256) -----
layout (local_size_x = 512, local_size_y = 1, local_size_z = 1) in;

// ---------- resources ----------
layout(std430, binding = 0) readonly buffer InputBuffer  { float  inData[];  };
layout(std430, binding = 1) writeonly buffer OutputBuffer { float outData[]; };

uniform int uCount;   // total elements in the current pass


void main()
{
    /* 1. load from the buffer into fast shared memory */
    uint  index = gl_WorkGroupID.x * gl_WorkGroupSize.x + gl_LocalInvocationID.x;
    float  newValue = inData[index] * inData[index];
    outData[index] = newValue;
}