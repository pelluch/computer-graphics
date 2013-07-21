#include <GL/glew.h>
#include "control.h"
#include <iostream>
#include <glm/glm.hpp>
#include <scene/scene.h>
#include <scene/camera.h>
#include "animation/spline.h"


float Control::_zoomAcceleration = 10.0f;
float Control::_mouseAcceleration = 0.1f;
glm::vec2 Control::_lastPosition = glm::vec2(0, 0);
bool Control::_hasPosition = false;
GameEngine * Control::_gameEngine;

void Control::windowResized(GLFWwindow* window, int width, int height)
{	
	Control::_gameEngine->resizeWindow(width, height);
}

void Control::keyCallBack(GLFWwindow* window, int key, int scancode, int action, int mods)
{

	glm::vec3 rotation, translation;
	if(key == GLFW_KEY_A)
		translation += glm::vec3(-10, 0, 0);
	else if(key == GLFW_KEY_D)
		translation -= glm::vec3(-10, 0, 0);
	else if(key == GLFW_KEY_W)
		translation += glm::vec3(0, 10, -0);
	else if(key == GLFW_KEY_S)
		translation -= glm::vec3(0, 10, -0);
	else if(key == GLFW_KEY_J)
		rotation += glm::vec3(0, 1, 0);
	else if(key == GLFW_KEY_L)
		rotation -= glm::vec3(0, 1, 0);
	else if(key == GLFW_KEY_I)
		rotation -= glm::vec3(1, 0, 0);
	else if(key == GLFW_KEY_K)
		rotation += glm::vec3(1, 0, 0);
	else if(key == GLFW_KEY_P && action == GLFW_PRESS)
		Control::_gameEngine->pause();

	_gameEngine->moveCamera(translation, rotation);	
}

void Control::setGameEngine(GameEngine * engine)
{
	_gameEngine = engine;
}

void Control::mousePosCallback(GLFWwindow * window, double x, double y)
{
	if(!glfwGetMouseButton(window, GLFW_MOUSE_BUTTON_1) == GLFW_PRESS) 
	{
		_hasPosition = false;
		return;
	}

	if(_hasPosition)
	{
		glm::vec2 deltaPos = glm::vec2(x, y) - _lastPosition;
		glm::vec3 rotation = glm::vec3(-deltaPos[1], deltaPos[0], 0)*_mouseAcceleration;
		_gameEngine->moveCamera(glm::vec3(0, 0, 0), rotation);
		_gameEngine->updateRenderer();
	}
	else
	{
		_hasPosition = true;
	}
	_lastPosition = glm::vec2(x, y);
}

void Control::mouseScrollCallback(GLFWwindow * window, double x, double y)
{
	glm::vec3 translation = glm::vec3(0, 0, -y)*_zoomAcceleration;
	_gameEngine->moveCamera(translation, glm::vec3(0,0,0));
}

void Control::mouseClickCallback(GLFWwindow * window, int button, int action, int mods)
{
	if(button == GLFW_MOUSE_BUTTON_1 && action == GLFW_PRESS)
	{
		double mouseX, mouseY;
		glfwGetCursorPos(window, &mouseX, &mouseY);
		//std::cout << "Clicking mouse positions" << "\t" << mouseX << " " << mouseY << std::endl;
		_gameEngine->pickUp((int)mouseX, (int)mouseY);
	}
}