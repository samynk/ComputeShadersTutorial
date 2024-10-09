#include "ComputeWindow.h"
#include "GrayFilter.h"
#include <string>
#include <iostream>

int main() {
	try {
		ComputeWindow<GrayFilter> window{ 512,512,"Compute Shader Tutorial" };
		window.init();
		window.renderLoop();
		window.close();
	}
	catch (const std::exception& e) {
		std::cerr << "An exception occurred: " << e.what() << std::endl;
		glfwTerminate();
		return EXIT_FAILURE;
	}
	return 0;
}
