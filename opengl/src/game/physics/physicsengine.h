#ifndef PHYSICS_ENGINE_H_
#define PHYSICS_ENGINE_H_

#include <bullet/btBulletDynamicsCommon.h>
#include <glm/glm.hpp>

class PhysicsEngine
{

public:
	PhysicsEngine();
	btCollisionWorld::ClosestRayResultCallback shootRay(glm::vec3 worldPosition, glm::vec3 worldDirection);
	void addRigidBody(btRigidBody * body);
private:
	btBroadphaseInterface * _broadphase;
	btDefaultCollisionConfiguration* _collisionConfiguration;
	btCollisionDispatcher * _dispatcher;
	btSequentialImpulseConstraintSolver * _solver;
	btDiscreteDynamicsWorld* _dynamicsWorld;

};

#endif