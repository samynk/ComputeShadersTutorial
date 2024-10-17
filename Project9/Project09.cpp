#include "ComputeWindow.h"
#include "ObjRayTracer.h"

int main()
{
	try {
		ComputeWindow<ObjRayTracer> window{ 1024,1024,"Raytracer with obj files", false };
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