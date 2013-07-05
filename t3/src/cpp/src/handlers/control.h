#ifndef CONTROL_H_
#define CONTROL_H_

#include <GLFW/glfw3.h>


void windowResized(GLFWwindow* window, int width, int height);
void key_callback(GLFWwindow* window, int key, int scancode, int action, int mods);

#endif