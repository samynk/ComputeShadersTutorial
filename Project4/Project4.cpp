#include "FlockCompute.h"
#include "ComputeWindow.h"

int main()
{
	try {
		ComputeWindow<FlockCompute> window{ 1024,1024,"Boids" };
		window.init();
		window.renderLoop();
		window.close();
	}
	catch (const std::exception& e) {
		std::cerr << "An exception occurred: " << e.what() << std::endl;
		glfwTerminate();
		return EXIT_FAILURE;
	}
}
