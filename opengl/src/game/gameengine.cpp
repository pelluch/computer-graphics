#include "game/gameengine.h"
#include <glm/glm.hpp>
#include <iostream>
#include "utils/xmlloader.h"
#include "handlers/control.h"
#include "renderer/params.h"
#include "utils/debugutils.h"

void GameEngine::setObjects(std::vector<Model> & models)
{
	for(int i = 0; i < models.size(); ++i)
	{
		std::cout << "Creating new object" << std::endl;
		boost::shared_ptr<GameObject> newObject(new GameObject(&models[i]));
		std::cout << "Setting rigid body" << std::endl;
		btRigidBody * rigidBody = newObject->initializeRigidBody();
		_physicsEngine->addRigidBody(rigidBody);
		std::cout << "Pushing back" << std::endl;
		_gameObjects.push_back(newObject);
	}
}

void GameEngine::draw()
{
	_renderer.beginDraw();	

	glm::mat4 perspectiveTransform = _scene->projectionTransform(RenderingParams::getAspectRatio());
	glm::mat4 viewTransform = _scene->viewTransform();

	_renderer.setViewMatrix(viewTransform);
	_renderer.setPerspectiveMatrix(perspectiveTransform);
	//glm::mat4 viewProjectionMatrix = perspectiveTransform*viewTransform;

	//glUniformMatrix4fv(matrixId, 1, GL_FALSE, &viewProjectionMatrix[0][0]);
	_scene->bindUniforms();
	_scene->draw(_renderer.getProgramId(), _renderer);
}

GameEngine::GameEngine()
{
	this->_physicsEngine = boost::shared_ptr<PhysicsEngine>(new PhysicsEngine());
	_renderer.init();

	std::cout << "Loading scene->.." << std::endl;
	_scene = boost::shared_ptr<Scene>(XmlLoader::loadScene("scenes/cornellBoxTarea2c.xml"));
	Control::setScene(_scene);
	_scene->initModelData();
	_renderer.setRenderingParams();	
	std::cout << "Scene loaded, setting shader id" << std::endl;
	_scene->setShaderId(_renderer.getProgramId());
	std::cout << "Generating scene ids" << std::endl;
	_scene->generateIds();
	_scene->setMaterials();
	setObjects(_scene->_models);
}



GameEngine::~GameEngine()
{
	for(int i = 0; i < _gameObjects.size(); i++)
	{
		delete _gameObjects[i];
	}
	delete _physicsEngine;
}

void GameEngine::updateRenderer()
{
	glm::mat4 perspectiveTransform = _scene->projectionTransform(RenderingParams::getAspectRatio());
	glm::mat4 viewTransform = _scene->viewTransform();

	_renderer.setViewMatrix(viewTransform);
	_renderer.setPerspectiveMatrix(perspectiveTransform);
}

void GameEngine::pickUp(int mouseX, int mouseY)
{
	
	for(int i = 0; i < _gameObjects.size(); ++i)
	{
		std::cout <<  _gameObjects[i]->getIdentifier() << std::endl;
	}
	std::cout << "Game object getting " << mouseX << std::endl;
	//std::cout << "Trying to pick object up" << std::endl;
	
	//std::cout << "Transforming screen coords to world" << std::endl;
	glm::vec3 worldStart, worldDirection;
	_renderer.screenToWorld(mouseX, mouseY, worldStart, worldDirection);

	//worldStart = _scene->_cameras[0].getPosition();
	std::cout << " world start: " << std::endl;
	Debugger::printInfo(worldStart);
	std::cout << " world end: " << std::endl;
	//worldStart = glm::vec3(0, 0, -400);
	//worldDirection = glm::vec3(0,0,1);
	worldDirection[1]*=-1;
	worldDirection = worldStart + worldDirection * 1000000.0f;
	//worldDirection = _scene->_cameras[0].getPosition() + worldDirection * 1000.0f;
	Debugger::printInfo(worldDirection);

	//worldDirection *= 1000;
	//std::cout << "Shooting ray" << std::endl;
	btCollisionWorld::ClosestRayResultCallback result = _physicsEngine->shootRay(worldStart, worldDirection);
	if(result.hasHit())
	{
		std::cout << "Hit object! " << std::endl;
		btVector3 hitPoints = result.m_hitPointWorld;
		glm::vec3 glmHit = glm::vec3(hitPoints[0], hitPoints[1], hitPoints[2]);
		Debugger::printInfo(glmHit);
		GameObject * objectHit = (GameObject*)result.m_collisionObject->getUserPointer();
		std::cout <<  objectHit->getName() << std::endl;
	}
	else
	{
		std::cout << "Did not hit :(" << std::endl;
	}
}