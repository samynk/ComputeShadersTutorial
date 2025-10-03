set(ProjectName Project10)

set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer3/CameraRays.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer3/ObjRayTracer2.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer3/RaysToTexture.glsl"
)

add_executable(${ProjectName}  ${SHADERS} "ObjRayTracer.cpp" "ObjRayTracer.h" "Project10.cpp" "MeshSSBO.h" "MeshSSBO.cpp")
configure(${ProjectName} "Projects/10_RaytracerWithGA")
