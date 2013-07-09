#include "physics/engine.h"


Engine::Engine()
{
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

}