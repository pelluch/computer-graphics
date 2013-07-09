#ifndef GAME_ENGINE_H_
#define GAME_ENGINE_H_

#include <vector>
#include "renderer/renderer.h"
#include "game/gameobject.h"
#include <map>

class GameEngine
{

private:
	Renderer _renderer;
	std::vector<GameObject> _gameObjects;
	PhysicsEngine * _physicsEngine;
public:
	GameEngine();
	~GameEngine();
	void initializeRigidObjects();
	void pickUp(int mouseX, int mouseY);
	

};

#endif