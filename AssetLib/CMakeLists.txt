add_library(AssetLib STATIC 
	 
	 
	"RLEReader.h" 
	"RLEReader.cpp")

target_include_directories(AssetLib PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})

 set_target_properties(AssetLib PROPERTIES
        FOLDER "DAE Libraries" )