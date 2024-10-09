add_library(3DLib STATIC  "3DLib.h" "Camera.h" "Camera.cpp")

target_include_directories(3DLib PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})

set_target_properties(3DLib PROPERTIES
        FOLDER "DAE Libraries" )

target_link_libraries(3DLib PRIVATE glm::glm)
