# ComputeShadersTutorial
A set of projects that demonstrate various aspects of compute shaders, focused on game related topics such as postprocessing, raytracing, ...

The project uses GLFW for rendering the (single) output of the compute shaders. 

##Project 00 - Gray filter

A simple gray filter that use the following formula to calculate the gray value:

```
float gray = 0.299*color.r + 0.587*color.g + 0.114*color.z;
```
This projects demonstrates how to load an input texture and bind the input texture and the output texture to the gray scale filter.

