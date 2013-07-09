#include "game/gameengine.h"
#include <glm/glm.hpp>
#include <iostream>

void GameEngine::initializeRigidObjects()
{
	for(size_t i = 0; i < _gameObjects.size(); ++i)
	{
		
	}
}

GameEngine::GameEngine()
{
	this->_physicsEngine = new PhysicsEngine();
}

GameEngine::~GameEngine()
{
	delete _physicsEngine;
}

void GameEngine::pickUp(int mouseX, int mouseY)
{
	glm::vec3 worldStart, worldDirection;
	_renderer.screenToWorld(mouseX, mouseY, worldStart, worldDirection);
	btCollisionWorld::ClosestRayResultCallback result = _physicsEngine->shootRay(worldStart, worldDirection);
	if(result.hasHit())
	{
		std::cout << result.m_collisionObject->getUserPointer();
	}
}