#include "game/gameobject.h"

int GameObject::_objectCounter = 0;

btRigidBody * GameObject::initializeRigidBody()
{
	//This is a box
	btCollisionShape * boxCollisionShape = new btBoxShape(btVector3(1.0f, 1.0f, 1.0f));

	btDefaultMotionState * motionstate = new btDefaultMotionState(btTransform(
		btQuaternion(_orientation[0], _orientation[1], _orientation[2], _orientation[3]), 
		btVector3(_position[0], _position[1], _position[2])
		));


	btRigidBody::btRigidBodyConstructionInfo rigidBodyCI(
		0,                  // mass, in kg. 0 -> Static object, will never move.
		motionstate,
		boxCollisionShape,  // collision shape of body
		btVector3(0,0,0)    // local inertia
		);

	_rigidBody = new btRigidBody(rigidBodyCI);
	_rigidBody->setUserPointer((void*)this);
	return _rigidBody;
}

GameObject::GameObject()
{
	_objectIdentifier = _objectCounter;
	_objectCounter++;
}

GameObject::~GameObject()
{
	delete _rigidBody;
}