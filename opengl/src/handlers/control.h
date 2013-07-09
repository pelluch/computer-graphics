#ifndef CONTROL_H_
#define CONTROL_H_

#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include "scene/scene.h"
#include "animation/spline.h"
#include "game/gameengine.h"
//#include "game/physics/physicsengine.h"


class Control
{
private:
	static Scene * _scene;
	static GameEngine * _gameEngine;
	static float _mouseAcceleration;
	static float _zoomAcceleration;
	static glm::vec2 _lastPosition;
	static bool _hasPosition;
public:
	static void setGameEngine(GameEngine * engine);
	static void windowResized(GLFWwindow* window, int width, int height);
	static void keyCallBack(GLFWwindow* window, int key, int scancode, int action, int mods);
	static void setScene(Scene * scene);
	static void mousePosCallback(GLFWwindow * window, double x, double y);
	static void mouseScrollCallback(GLFWwindow * window, double x, double y);
	static void mouseClickCallback(GLFWwindow * window, int button, int action, int mods);
	static Spline spline;

};

#endif