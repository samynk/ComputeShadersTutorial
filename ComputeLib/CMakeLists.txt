add_library(ComputeLib STATIC 
	"SurfaceRenderer.h" "SurfaceRenderer.cpp"  
	"ComputeShader.h" "ComputeShader.cpp"
	"GLImage.h" "GLImage.cpp"
	"ComputeWindow.h"  
	"ShaderStorageBufferObject.h"   "Compute.h")
find_package(OpenGL REQUIRED)

target_include_directories(ComputeLib PUBLIC 
	${CMAKE_CURRENT_SOURCE_DIR}
    ${OPENGL_INCLUDE_DIRS} 
    ${glew_SOURCE_DIR}/include 
    ${stb_SOURCE_DIR}
)

target_link_libraries(ComputeLib PUBLIC AssetLib OpenGL::GL glfw libglew_static)