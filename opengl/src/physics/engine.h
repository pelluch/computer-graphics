#ifndef ENGINE_H_
#define ENGINE_H_

#include <bullet/btBulletDynamicsCommon.h>

class Engine
{

public:
	Engine();
private:
	btBroadphaseInterface * _broadphase;
	btDefaultCollisionConfiguration* _collisionConfiguration;
	btCollisionDispatcher * _dispatcher;
	btSequentialImpulseConstraintSolver * _solver;
	btDiscreteDynamicsWorld* _dynamicsWorld;

};

#endif