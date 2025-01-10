
set(ProjectName Project4)

set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/flocking/clear.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/flocking/convert_flock_to_texture.glsl"
    "${CMAKE_SOURCE_DIR}/Resources/computeshaders/flocking/flocking.glsl"
)

add_executable(${ProjectName} "Project4.cpp"  "FlockCompute.h" "FlockCompute.cpp" ${SHADERS})
configure(${ProjectName} "04_Flocking")