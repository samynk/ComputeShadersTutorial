function(configure PROJECT_NAME PROJECT_FOLDER)
    # Find required OpenGL package
    find_package(OpenGL REQUIRED)

    # Set C++ standard if CMake version is greater than 3.12
    if (CMAKE_VERSION VERSION_GREATER 3.12)
        set_property(TARGET ${PROJECT_NAME} PROPERTY CXX_STANDARD 20)
    endif()

    # Include directories for OpenGL, GLM, GLEW, and STB
    target_include_directories(
        ${PROJECT_NAME} 
        PRIVATE
        ${OPENGL_INCLUDE_DIRS}
        ${glew_SOURCE_DIR}/include
        ${stb_SOURCE_DIR}
    )

    # Link libraries to the executable target
    target_link_libraries(${PROJECT_NAME} PRIVATE OpenGL::GL glm::glm glfw libglew_static ComputeLib)

    # Copy the resources folder after build
    add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E copy_directory
            "${CMAKE_SOURCE_DIR}/Resources"
            "$<TARGET_FILE_DIR:${PROJECT_NAME}>/"
    )
    message("setting folder to ${PROJECT_FOLDER}")
    set_target_properties(${PROJECT_NAME} PROPERTIES
        FOLDER "${PROJECT_FOLDER}" )
endfunction()