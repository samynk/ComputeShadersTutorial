#include "ComputeWindow.h"
#include "SphereRayTracer.h"

int main()
{
	try {
		ComputeWindow<SphereRayTracer> window{ 1024,1024,"Raytracer with spheres", false };
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