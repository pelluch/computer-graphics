#ifndef GAME_ENGINE_H_
#define GAME_ENGINE_H_

#include <vector>
#include "renderer/renderer.h"
#include "model/model.h"
#include "scene/scene.h"
#include "game/gameobject.h"
#include <map>
#include <boost/shared_ptr.hpp>

class GameEngine
{

private:
	Renderer _renderer;
	boost::shared_ptr<Scene> _scene;
	std::vector< boost::shared_ptr<GameObject> > _gameObjects;
	boost::shared_ptr<PhysicsEngine> _physicsEngine;
	double _lastUpdate;
	int _numUpdates;

public:
	GameEngine();
	void draw();
	void setObjects(std::vector<Model> & models);
	void pickUp(int mouseX, int mouseY);
	void updateRenderer();
	void start();
	void update();
};

#endif