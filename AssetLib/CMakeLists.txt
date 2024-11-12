add_library(AssetLib STATIC 	 
	"RLEReader.h" 
	"RLEReader.cpp" 
	"ObjReader.h" 
	"ObjReader.cpp"
)

target_include_directories(AssetLib PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})

target_link_libraries(AssetLib PRIVATE glm::glm)

 set_target_properties(AssetLib PROPERTIES
        FOLDER "DAE Libraries" )