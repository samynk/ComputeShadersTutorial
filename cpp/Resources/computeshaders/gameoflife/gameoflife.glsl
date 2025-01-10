#version 430 core

layout (local_size_x = 16, local_size_y = 16, local_size_z = 1) in;

layout(std430, binding = 0) buffer InputData {
    int inputArray[];  // SSBO containing the 2D array of the current state of the game of life.
};

layout(std430, binding = 1) buffer OutputData {
    int outputArray[];  // SSBO containing the 2D array of the new state of the game of life.
};



uniform ivec2 gridDimension;

int sampleInput(int x, int y){
	if ( x < 0 || x > gridDimension.x-1 || y < 0 || y > gridDimension.y-1 )
		return 0;
	else
		return inputArray[y *gridDimension.x + x]; 
}

void writeOutput(ivec2 pos, int value)
{
	outputArray[pos.y*gridDimension.x +pos.x] = value;
}

void main() {
    ivec2 gid = ivec2(gl_GlobalInvocationID.xy);  // Get global workgroup ID

    int neighbours = 0;
	// row above
	neighbours += sampleInput(gid.x-1,gid.y+1);
	neighbours += sampleInput(gid.x  ,gid.y+1);
	neighbours += sampleInput(gid.x+1,gid.y+1);
	// same row
	neighbours += sampleInput(gid.x-1,gid.y);
	neighbours += sampleInput(gid.x+1,gid.y);
	// row below
	neighbours += sampleInput(gid.x-1,gid.y-1);
	neighbours += sampleInput(gid.x  ,gid.y-1);
	neighbours += sampleInput(gid.x+1,gid.y-1);

    int cell = sampleInput(gid.x, gid.y);
	
	writeOutput(gid, ((neighbours==3) || (neighbours==2 && cell==1))?1:0);
}