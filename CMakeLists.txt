# CMakeList.txt : Top-level CMake project file, do global configuration
# and include sub-projects here.
#
cmake_minimum_required (VERSION 3.22)



# Enable Hot Reload for MSVC compilers if supported.
if (POLICY CMP0141)
  cmake_policy(SET CMP0141 NEW)
  set(CMAKE_MSVC_DEBUG_INFORMATION_FORMAT "$<IF:$<AND:$<C_COMPILER_ID:MSVC>,$<CXX_COMPILER_ID:MSVC>>,$<$<CONFIG:Debug,RelWithDebInfo>:EditAndContinue>,$<$<CONFIG:Debug,RelWithDebInfo>:ProgramDatabase>>")
endif()

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

project ("ComputeShaders")
set(USE_FOLDERS ON)
file(GLOB SHADERS
        "${CMAKE_SOURCE_DIR}/Resources/computeshaders/*.comp"
    )

# Include FetchContent module
include(FetchContent)

# Fetch GLM
FetchContent_Declare(
  glm
  GIT_REPOSITORY https://github.com/g-truc/glm.git
  GIT_TAG        1.0.1
)
FetchContent_MakeAvailable(glm)

# Fetch GLFW
FetchContent_Declare(
  glfw
  GIT_REPOSITORY https://github.com/glfw/glfw.git
  GIT_TAG        3.4  # Use the desired GLFW version
  GIT_SHALLOW TRUE
)
FetchContent_MakeAvailable(glfw)

# Fetch GLEW
FetchContent_Declare(
  glew
  GIT_REPOSITORY https://github.com/Perlmint/glew-cmake.git
  GIT_TAG        glew-cmake-2.2.0  # Use the desired GLEW version
  GIT_SHALLOW TRUE
)
FetchContent_MakeAvailable(glew)

# Fetch stb
FetchContent_Declare(
  stb
  GIT_REPOSITORY https://github.com/nothings/stb.git
  GIT_TAG        master  # Or specify a specific commit or tag
  GIT_SHALLOW TRUE
)
FetchContent_MakeAvailable(stb)

include(${CMAKE_SOURCE_DIR}/util.cmake)

set_target_properties(glfw PROPERTIES
        FOLDER "External Libraries" )

set_target_properties(glm PROPERTIES
        FOLDER "External Libraries" )

set_target_properties(libglew_shared PROPERTIES
        FOLDER "External Libraries" )

set_target_properties(libglew_static PROPERTIES
        FOLDER "External Libraries" )

# Include sub-projects.
add_subdirectory("ComputeLib")
add_subdirectory("AssetLib")
#add_subdirectory("3DLib")
add_subdirectory("Project0")
add_subdirectory("Project1")
add_subdirectory("Project2")
add_subdirectory("Project3")
add_subdirectory("Project4")
add_subdirectory("Project5")
add_subdirectory("Project6")
add_subdirectory("Project7")

add_subdirectory("3DLib")
