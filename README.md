# ComputeShadersTutorial
A set of projects that demonstrate various aspects of compute shaders, focused on game related topics such as postprocessing, raytracing, ...

The project uses GLFW for rendering the (single) output of the compute shaders. 

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
