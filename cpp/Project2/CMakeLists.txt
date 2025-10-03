set(ProjectName Project2)

set(SHADERS 
	"${CMAKE_SOURCE_DIR}/Resources/computeshaders/convert_to_texture.glsl"
)

add_executable(${ProjectName} "Project2.cpp"  "ConvertBuffer.h" "ConvertBuffer.cpp" ${SHADERS})
configure(${ProjectName} "Projects/02_SSBO_to_Texture")