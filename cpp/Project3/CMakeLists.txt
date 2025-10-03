set(ProjectName Project3)
set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/convert_to_texture.glsl"
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/gameoflife/gameoflife.glsl"
)

add_executable(${ProjectName} "Project3.cpp"  "GameOfLife.h" "GameOfLife.cpp" ${SHADERS})
configure(${ProjectName} "Projects/03_ConwayGameOfLife")

