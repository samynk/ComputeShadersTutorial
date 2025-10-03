set(ProjectName Project6)

set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer1/CameraRays.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/ClearDepthBuffer.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer1/SphereRayTracer.glsl"
)

add_executable(${ProjectName} "Project6.cpp"  "SphereRayTracer.h" "SphereRayTracer.cpp" ${SHADERS})
configure(${ProjectName} "Projects/06_RaytracerSphereScene")
