// MaxReduce.glsl
#version 450
// ----- workgroup size (power of two, e.g. 256) -----
layout (local_size_x = 512, local_size_y = 1, local_size_z = 1) in;

// ---------- resources ----------
layout(std430, binding = 0) readonly buffer InputBuffer  { float  inData[];  };
layout(std430, binding = 1) writeonly buffer OutputBuffer { float outData[]; };

uniform int uCount;   // total elements in the current pass

// ---------- kernel ----------
shared float sData[gl_WorkGroupSize.x];

void main()
{
    /* 1. load from the buffer into fast shared memory */
    uint  index = gl_WorkGroupID.x * gl_WorkGroupSize.x + gl_LocalInvocationID.x;
    float  value = (index < uCount) ? inData[index] : 0.0f;

    sData[gl_LocalInvocationID.x] = value;
    memoryBarrierShared();  
    barrier();   // make sure every thread has written

    /* 2. treestyle reduction inside the workgroup */
    for (uint stride = gl_WorkGroupSize.x >> 1; stride > 0; stride >>= 1)
    {
        if (gl_LocalInvocationID.x < stride)
            sData[gl_LocalInvocationID.x] =
                max(sData[gl_LocalInvocationID.x],
                    sData[gl_LocalInvocationID.x + stride]);
        memoryBarrierShared();  
        barrier();
    }

    /* 3. thread writes this block's maximum */
    if (gl_LocalInvocationID.x == 0u)
    {
        float result = sData[0];
        outData[gl_WorkGroupID.x] = result;
    }
}