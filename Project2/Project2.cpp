#include "ConvertBuffer.h"
#include "ComputeWindow.h"

int main()
{
	try {
		ComputeWindow<ConvertBuffer> window{ 512, 512, "Conversion from buffer to texture" };
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
