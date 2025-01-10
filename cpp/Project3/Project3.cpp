#include "GameOfLife.h"
#include "ComputeWindow.h"

int main()
{
	try {
		ComputeWindow<GameOfLife> window{ 1024,1024,"Game of Life" };
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
