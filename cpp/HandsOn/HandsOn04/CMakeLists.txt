set(ProjectName HandsOn04)

set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/handson/CameraRays.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/handson/ClearDepthBuffer.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/handson/SphereRayTracer.glsl"
)

add_executable(${ProjectName} "HandsOn04.cpp"  "SphereRayTracer.h" "SphereRayTracer.cpp" ${SHADERS})
configure(${ProjectName} "HandsOn/04_Spheres")
