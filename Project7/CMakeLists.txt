set(ProjectName Project7)

set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/ClearDepthBuffer.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer2/CameraRays.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer2/SphereRayTracer.glsl"
)

add_executable(${ProjectName} "Project7.cpp"  "SphereRayTracer.h" "SphereRayTracer.cpp" ${SHADERS})
configure(${ProjectName} "07_RaytracerCameraSphereScene")
