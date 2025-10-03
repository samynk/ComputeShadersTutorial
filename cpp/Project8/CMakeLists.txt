set(ProjectName Project08)

set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer3/CameraRays.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer3/SphereRayTracer.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/raytracer3/RaysToTexture.glsl"
)

add_executable(${ProjectName} "Project08.cpp"  ${SHADERS} "SphereRayTracer.cpp" "SphereRayTracer.h")
configure(${ProjectName} "Projects/08_RaytracerWithBounce")
