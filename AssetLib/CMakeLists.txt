add_library(AssetLib STATIC 
	 
	 
	"RLEReader.h" 
	"RLEReader.cpp")

target_include_directories(AssetLib PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})