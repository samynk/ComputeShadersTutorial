using namespace std;
#include "ComputeWindow.h"
#include "ComputeBlur.h"
#include <string>
#include <iostream>

int main() {
	try {
		ComputeWindow<ComputeBlur> window{ 512,512,"Blur filter" };
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
