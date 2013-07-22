#ifndef GAME_ENGINE_H_
#define GAME_ENGINE_H_

#include <vector>
#include <glm/glm.hpp>
#include "renderer/renderer.h"
#include "scene/scene.h"
#include "game/gameobject.h"

class GameEngine
{

private:
	Renderer  * _renderer;
	Scene * _scene;
	GLFWwindow * _window;
	std::vector< GameObject * > _gameObjects;
	PhysicsEngine * _physicsEngine;
	bool _paused;
	double _lastUpdate;
	double _lastCheck;
	double _timeBetweenUpdates;
	int _numUpdates;

public:
	GameEngine(int width = 800, int height = 600);
	~GameEngine();
	void draw();
	void setObjects(std::vector<Model> & models);
	void pickUp(int mouseX, int mouseY);
	void updateRenderer();
	void start();
	void update();
	void setCallbacks();
	void pause();
	void resizeWindow(int width, int height);
	void moveCamera(glm::vec3 translation, glm::vec3 rotation);
};

#endif