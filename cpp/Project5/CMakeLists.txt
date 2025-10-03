set(SOURCES 
	"Project5.cpp" 
	"CameraRayGenerator.h" 
	"CameraRayGenerator.cpp"
)
set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer1/CameraRays.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer1/VisualizeRays.glsl"
)
message(${SHADERS})
add_executable(Project5 ${SOURCES} ${SHADERS} )
configure(Project5 "Projects/05_RaytracingCameraRays")


