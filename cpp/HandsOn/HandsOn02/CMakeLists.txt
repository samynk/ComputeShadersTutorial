set(ProjectName HandsOn02)

set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/handson/addfloats.glsl"
)

add_executable(${ProjectName} "Main.cpp" "AddFloats.h" "AddFloats.cpp" ${SHADERS})
configure(${ProjectName} "HandsOn/02_AddFloats")

