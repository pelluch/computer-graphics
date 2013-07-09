#include "game/gameobject.h"
#include <iostream>
#include "utils/debugutils.h"

int GameObject::_objectCounter = 0;

int GameObject::getIdentifier()
{
	return _objectIdentifier;
}

btRigidBody * GameObject::initializeRigidBody()
{
	//This is a box

	btCollisionShape * boxCollisionShape = new btBoxShape(btVector3(_model->_scale[0], _model->_scale[1], _model->_scale[2]));

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

GameObject::GameObject(Model * model)
{
	_model = model;
	_position = model->_worldPosition;
	_orientation = glm::normalize(glm::quat(glm::vec3(0,0,0)));
	Debugger::printInfo(glm::vec4(_orientation[0], _orientation[1], _orientation[2], _orientation[3]));
	_objectIdentifier = _objectCounter;
	_objectCounter++;
	std::cout << "Created game object" << std::endl;
}

std::string GameObject::getName()
{
	return _model->_modelName;
}
GameObject::~GameObject()
{
	std::cout << "Deleting game object" << std::endl;
	delete _rigidBody;
}