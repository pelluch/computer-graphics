#include "game/gameengine.h"
#include <glm/glm.hpp>
#include <iostream>
#include "utils/xmlloader.h"
#include "handlers/control.h"
#include "utils/debugutils.h"

void GameEngine::setObjects(std::vector<Model> & models)
{
	for(size_t i = 0; i < models.size(); ++i)
	{
		std::cout << "Creating new object" << std::endl;
		GameObject * newObject = new GameObject(&models[i]);
		std::cout << "Setting rigid body" << std::endl;
		btRigidBody * rigidBody = newObject->initializeRigidBody(SHAPE_TRIANGLE_MESH);
		_physicsEngine->addRigidBody(rigidBody);
		std::cout << "Pushing back" << std::endl;
		_gameObjects.push_back(newObject);
	}
}

void GameEngine::start()
{
	do
	{		
		if(!_paused)
		{
				// Measure speed
			double currentTime = glfwGetTime();
			if(currentTime - _lastUpdate >= _timeBetweenUpdates)
			{

				_numUpdates++;
				_lastUpdate += _timeBetweenUpdates;
				_renderer->draw(_scene, currentTime);
				update();

				if(currentTime - _lastCheck >= 1.0)
				{
					std::cout << _numUpdates << " updates per second" << std::endl;
					_numUpdates = 0;
					_lastCheck += 1.0;
				}
				
			}
    	}
        /* Poll for and process events */
        glfwPollEvents();
	} while( !glfwWindowShouldClose(_window) && 
		!glfwGetKey(_window, GLFW_KEY_ESCAPE) == GLFW_PRESS);

	glfwDestroyWindow(_window);

}

//Game logic. Nothing yet, obviously...
void GameEngine::update()
{


}

GameEngine::GameEngine(int width, int height)
{

	_physicsEngine = NULL;
	_renderer = NULL;
	_paused = false;
	_scene = NULL;
	_timeBetweenUpdates = 1.0/300.0;

	if(!glfwInit())
	{
		std::cerr << "Failed to initialize glfw" << std::endl;
	}
	std::cout << "GLFW initialized" << std::endl;
	_window = glfwCreateWindow(width, height, "Game", NULL, NULL);
	if(!_window)
	{
		std::cerr << "Failed to open GLFW window. If you have an Intel GPU, they are not 3.3 compatible. Try the 2.1 version of the tutorials." << std::endl;
	}

	_renderer = new Renderer(_window);
	Control::setGameEngine(this);
	glfwSetInputMode(_window, GLFW_STICKY_KEYS, GLFW_KEY_ESCAPE);
	glfwSetWindowSizeCallback(_window, Control::windowResized);
	glfwSetKeyCallback(_window, Control::keyCallBack);
	glfwSetCursorPosCallback(_window, Control::mousePosCallback);
	glfwSetScrollCallback(_window, Control::mouseScrollCallback);
	glfwSetMouseButtonCallback(_window, Control::mouseClickCallback);
	glfwSwapInterval(0);

	this->_physicsEngine = new PhysicsEngine();
	
	std::cout << "Loading scene->.." << std::endl;
	_scene = XmlLoader::loadScene("scenes/cornellBoxTarea2c.xml");
	_scene->setShaderId(_renderer->getProgramId());
	_scene->initModelData();
	_scene->setMaterials();
	_scene->generateIds();
	//_scene->generateLineIds();
	_renderer->setRenderingParams();	

	setObjects(_scene->_models);

	_numUpdates = 0;
	_lastUpdate = glfwGetTime();
	_lastCheck = _lastUpdate;
}

void GameEngine::moveCamera(glm::vec3 translation, glm::vec3 rotation)
{
	_scene->moveCamera(translation, rotation);
}

void GameEngine::resizeWindow(int width, int height)
{
	_renderer->setWindowSize(width, height);
}

void GameEngine::pause()
{
	_paused = !_paused;
}

GameEngine::~GameEngine()
{
	delete _renderer;
	delete _scene;
	delete _physicsEngine;
	
	for(size_t i = 0; i < _gameObjects.size(); ++i)
	{
		delete _gameObjects[i];
	}

	glfwTerminate();
}

void GameEngine::updateRenderer()
{
	glm::mat4 perspectiveTransform = _scene->projectionTransform(_renderer->getAspectRatio());
	glm::mat4 viewTransform = _scene->viewTransform();
	
	_renderer->setViewMatrix(viewTransform);
	_renderer->setPerspectiveMatrix(perspectiveTransform);
}

void GameEngine::pickUp(int mouseX, int mouseY)
{

	glm::vec3 worldStart, worldDirection;
	_renderer->screenToWorld(mouseX, mouseY, worldStart, worldDirection);

	worldDirection = worldStart + worldDirection * 1000000.0f;

	btCollisionWorld::ClosestRayResultCallback result = _physicsEngine->shootRay(worldStart, worldDirection);
	if(result.hasHit())
	{
		//std::cout << "Hit object! " << std::endl;
		btVector3 hitPoints = result.m_hitPointWorld;
		glm::vec3 glmHit = glm::vec3(hitPoints[0], hitPoints[1], hitPoints[2]);
		//_scene->drawRay(worldStart, glmHit);
		//Debugger::printInfo(glmHit);
		GameObject * objectHit = (GameObject*)result.m_collisionObject->getUserPointer();
		std::cout <<  "Hit object \t" << objectHit->getName() << "\tin coords\t";
		Debugger::printInfo(glmHit);
		//std::cout << std::endl;
	}
	else
	{
		std::cout << "Did not hit :(" << std::endl;
	}
}