#include "game/physics/physicsengine.h"
#include <iostream>

PhysicsEngine::PhysicsEngine()
{
	std::cout << "Physics engine initializing" << std::endl;
	//Initialize the broadphase
	_broadphase = new btDbvtBroadphase();
	//Default configuration
	_collisionConfiguration = new btDefaultCollisionConfiguration();	

	_dispatcher = new btCollisionDispatcher(_collisionConfiguration);
	// The actual physics solver
	_solver = new btSequentialImpulseConstraintSolver;

	_dynamicsWorld = new btDiscreteDynamicsWorld(_dispatcher,_broadphase,_solver,_collisionConfiguration);
	// Create dynamic world
	_dynamicsWorld->setGravity(btVector3(0,-9.81f,0));

	std::cout << "Physics engine created" << std::endl;

}

PhysicsEngine::~PhysicsEngine()
{
	delete _dynamicsWorld;
	delete _solver;
	delete _dispatcher;
	delete _collisionConfiguration;
	delete _broadphase;
}

void PhysicsEngine::addRigidBody(btRigidBody * body)
{
	//Add new rigid body
	_dynamicsWorld->addRigidBody(body);
}

btCollisionWorld::ClosestRayResultCallback PhysicsEngine::shootRay(glm::vec3 worldPosition, glm::vec3 worldDirection)
{
	btCollisionWorld::ClosestRayResultCallback RayCallback(btVector3(worldPosition[0], worldPosition[1], worldPosition[2]), 
		btVector3(worldDirection[0], worldDirection[1], worldDirection[2]));
	_dynamicsWorld->rayTest(btVector3(worldPosition[0], worldPosition[1], worldPosition[2]), 
		btVector3(worldDirection[0], worldDirection[1], worldDirection[2]), RayCallback);

	return RayCallback;
}