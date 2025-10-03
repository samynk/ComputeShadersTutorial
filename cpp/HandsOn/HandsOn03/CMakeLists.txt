set(ProjectName "HandsOn03")
set(SOURCES 
	"HandsOn03.cpp" 
	"CameraRayGenerator.h" 
	"CameraRayGenerator.cpp"
)
set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/handson/CameraRays.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/handson/VisualizeRays.glsl"
)
message(${SHADERS})
add_executable(${ProjectName} ${SOURCES} ${SHADERS} )
configure(${ProjectName} "HandsOn/03_VisualizeCamera")


