#include "CameraRayGenerator.h"
#include "ComputeWindow.h"

int main()
{
	try {
		ComputeWindow<CameraRayGenerator> window{ 512,512,"Camera rays" };
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
