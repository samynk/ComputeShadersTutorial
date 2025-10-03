set(ProjectName Project09)

set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer3/CameraRays.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer3/ObjRayTracer.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer3/RaysToTexture.glsl"
)

add_executable(${ProjectName}  ${SHADERS} "ObjRayTracer.cpp" "ObjRayTracer.h" "Project09.cpp" "MeshSSBO.h" "MeshSSBO.cpp")
configure(${ProjectName} "Projects/09_RaytracerWithObjMesh")
