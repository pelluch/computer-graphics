#ifndef GAME_ENGINE_H_
#define GAME_ENGINE_H_

#include <vector>
#include "renderer/renderer.h"
#include "model/model.h"
#include "scene/scene.h"
#include "game/gameobject.h"
#include <map>

class GameEngine
{

private:
	Renderer _renderer;
	Scene * _scene;
	std::vector<GameObject * > _gameObjects;
	PhysicsEngine * _physicsEngine;
public:
	GameEngine();
	~GameEngine();
	void draw();
	void setObjects(std::vector<Model> & models);
	void pickUp(int mouseX, int mouseY);
	void updateRenderer();
};

#endif