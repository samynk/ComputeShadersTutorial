# ComputeShadersTutorial
A set of projects that demonstrate various aspects of compute shaders, focused on game related topics such as postprocessing, raytracing, ...

The project uses GLFW for rendering the (single) output of the compute shaders. 

## Building the software

It is a CMake project, which means that you can either build it from the command line, or you can also open the project in Visual Studio or Visual Code to build and run the examples.
In principle, the code should also run on Linux but this is untested as of now.

## Project 00 - Gray filter

A simple gray filter that use the following formula to calculate the gray value:

```
float gray = 0.299*color.r + 0.587*color.g + 0.114*color.z;
```
The compute shader also contains a simple sepia filter as a comment:

```
float rr = (color.x * .393) + (color.y *.769) + (color.z * .189);
float rg = (color.x * .349) + (color.y *.686) + (color.z * .168);
float rb = (color.x * .272) + (color.y *.534) + (color.z * .131);
```
This project demonstrates how to load an input texture and bind the input texture and the output texture to the gray scale or sepia filter.

## Project 01 - Blur filter

A blur filter with two 1D kernels that together calculate a 7x7 gaussian blur filter. The glMemoryBarrier function is needed as the compute shader has to
 run twice, once in the vertical direction and once in the horizontal direction. The output of the first pass then becomes the input for the second pass which
 writes the result in the final output texture.

## Project 02 - SSBO to Texture

SSBO: Shader Storage Buffer Object

This project showcases how to upload custom data to the gpu and use this data to generate an output texture. The concept of workgroups is further refined to include an 
(integer) scalefactor between the input data and the output texture which allows you to represent a single element of the input to for example a 2x2, 4x4, ... pixel rectangle in the output.

## Project 03 - Conway Game of Life

Conway's game of life as a compute shader. The project uses two SSBO's to swap between the old and the new state of the game of life grid. The current SSBO is then converted to a texture to
 show the current state of the simulation.

## Project 04 - Flocking

 This project shows a flocking simulation, again with two SSBO's to model the new state and the current state. This project also showcases using structured data (i.e. structs) in combination with
  SSBO's. In 2D , the Boid just has a 2D point for its location and a 2D vector for its velocity. This is a happy coincidence, because one of the pitfalls of compute shaders is the possible difference
  in memory layout for structured data. Mainly, on the gpu it is typical to layout data members on 16 byte boundaries, which can lead to wrong results on the GPU side.

## Project 05 - Raytracer camera rays

 Ah raytracing, every developer should have written at least a simple version of this classic parallel algorithm. To have a good idea if the implementation is going well, it is a good idea to split the implementation
  up into multiple smaller step. In this project, the camera rays are implemented for a camera that is at the origin and is looking in the Z-direction (0,0,1). The ray directions are normalized and a separate compute shader
   is then used to represent the ray directions as colors, which should result in a nice looking gradient. (xyz -> rgb).

## Project 06 - Raytracer sphere scene

A list of sphere data is uploaded as an SSBO in this project. For each pixel of the output, the corresponding ray direction is sampled and the distance of the hit with the center is calculated. A depth buffer SSBO is used
 to compare the depth values. This is not strictly necessary as the projects processes one pixel at the time and has only one list of the same primitive type. However when combining multiple primitives this depth buffer will be necessary. 

 The spheres are all flat shaded, as there are no lights yet.

## Project 07 - Raytracer sphere scene with camera (in progress)

Interactivity makes everything nicer. This project adds a camera to the raytracer and makes it possible to move around. This project also has a hardcoded light position that is used for a simple cosine law diffuse light effect.
 
## Project 08 - Raytracer with bounces

Bounces are added to the raytracer. A simple approach was chosen here where an SSBO with a Raytracer
 struct is the main datastructure which keeps track of the necessary data to calculate one pixel:
 
 - ray origin: camera position for the first bounce.
 - depth value (unused so far)
 - ray direction: the direction of the ray.
 - color (sum of all the contributions of all the bounces), set to background color at the start.
 - rayHits : number of hits that ray encountered.
   
## Project 09 - Raytracer with Triangle meshes

This project uses the ObjReader in the AssetLib to load and render a 3D mesh. Event with a limited number of triangles, the need for acceleration structures and culling invisible triangles becomes apparent. The Möller–Trumbore intersection algorithm is used here to calculate the intersection  between a ray and a triangle.
