#include "event_handlers.h"

void windowResized(GLFWwindow* window, int width, int height)
{
	glViewport(0, 0, width, height);
}