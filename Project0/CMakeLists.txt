set(ProjectName Project0)
set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/imageprocessing/grayscale.glsl"
)
add_executable(${ProjectName} "Project0.cpp"  "GrayFilter.h" "GrayFilter.cpp" ${SHADERS})
configure(${ProjectName} "00_GrayFilter")
