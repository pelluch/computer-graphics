#include <GL/glew.h>
#include "control.h"
#include "renderer/params.h"
#include <iostream>
#include <glm/glm.hpp>
#include <scene/scene.h>
#include <scene/camera.h>
#include "utils/settings.h"
#include "animation/spline.h"


std::shared_ptr<Scene> Control::_scene;
float Control::_zoomAcceleration = 10.0f;
float Control::_mouseAcceleration = 0.1f;
glm::vec2 Control::_lastPosition = glm::vec2(0, 0);
bool Control::_hasPosition = false;
std::shared_ptr<GameEngine> Control::_gameEngine;
Spline Control::spline;

void Control::windowResized(GLFWwindow* window, int width, int height)
{	
	glViewport(0, 0, width, height);
	RenderingParams::setWindowSize(width, height);
}

void Control::step(float deltaT)
{
	_scene->_cameras[0].setAll(Control::spline.evaluate(deltaT), glm::vec3(370,120,370));

}
void Control::keyCallBack(GLFWwindow* window, int key, int scancode, int action, int mods)
{

	if(!_scene)
	{
		return;
	}
	glm::vec3 rotation, translation;
	if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
		Settings::settingsMenu();

	else if(key == GLFW_KEY_A)
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
	else if(key == GLFW_KEY_N)
		_scene->_cameras[0].setAll(spline.evaluate(0.2f), glm::vec3(370,120,370));
	else if(key == GLFW_KEY_P && action == GLFW_PRESS)
		RenderingParams::paused = !RenderingParams::paused;

	_scene->moveCamera(translation, rotation);	
}

void Control::setScene(std::shared_ptr<Scene> scene)
{
	_scene = scene;
	spline.generateSpline(glm::vec3(370, 500, 370), 500);
	_scene->_cameras[0].setAll(spline.evaluate(0.2f), glm::vec3(370,120,370));

}

void Control::setGameEngine(std::shared_ptr<GameEngine> engine)
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
		_scene->moveCamera(glm::vec3(0, 0, 0), rotation);
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
	_scene->moveCamera(translation, glm::vec3(0,0,0));
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