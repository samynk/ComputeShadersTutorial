set(ProjectName Project1)

set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/imageprocessing/blur.glsl"
)

add_executable(${ProjectName} "Main.cpp" "ComputeBlur.h" "ComputeBlur.cpp" ${SHADERS})
configure(${ProjectName} "01_BlurFilter")

