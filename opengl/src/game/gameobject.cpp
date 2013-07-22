#include "game/gameobject.h"
#include <iostream>
#include "utils/debugutils.h"

int GameObject::_objectCounter = 0;

int GameObject::getIdentifier()
{
	return _objectIdentifier;
}

btRigidBody * GameObject::initializeRigidBody(BODY_SHAPE shape)
{

	_bodyShape = shape;
	/*if(shape == SHAPE_TRIANGLE_MESH)
	{
		_triangleMesh = new btTriangleMesh();
		std::vector<glm::vec3> worldVertices = _model->getWorldVertices();
		size_t numTriangles = worldVertices.size()/3;
		for(size_t i = 0; i < numTriangles; ++i)
		{
			const btVector3 v1 = glmVecToBullet(worldVertices[i*3]);
			const btVector3 v2 = glmVecToBullet(worldVertices[i*3+1]);
			const btVector3 v3 = glmVecToBullet(worldVertices[i*3+2]);
			_triangleMesh->addTriangle(v1, v2, v3);
		}
		_triangleShape = new btBvhTriangleMeshShape(_triangleMesh, false);

		btVector3 localInertia(0, 0, 0);
		_triangleShape->calculateLocalInertia(1, localInertia);
		_motionstate = new btDefaultMotionState(btTransform(
			btQuaternion(_orientation[0], _orientation[1], _orientation[2], _orientation[3]), 
			btVector3(_position[0], _position[1], _position[2])
			));

		_rigidBody = new btRigidBody(1, _motionstate, _triangleShape, localInertia);      
		_rigidBody->setContactProcessingThreshold(BT_LARGE_FLOAT);
		_rigidBody->setCcdMotionThreshold(.5);
		_rigidBody->setCcdSweptSphereRadius(0);
	}
	*/
	//else if(shape == SHAPE_CUBE)
	//{
		_boxCollisionShape = new btBoxShape(btVector3(_model->_scale[0], _model->_scale[1], _model->_scale[2]));

		_motionstate = new btDefaultMotionState(btTransform(
			btQuaternion(_orientation[0], _orientation[1], _orientation[2], _orientation[3]), 
			btVector3(_position[0], _position[1], _position[2])
			));


		btRigidBody::btRigidBodyConstructionInfo rigidBodyCI(
			0,                  // mass, in kg. 0 -> Static object, will never move.
			_motionstate,
			_boxCollisionShape,  // collision shape of body
			btVector3(0,0,0)    // local inertia
			);

		_rigidBody = new btRigidBody(rigidBodyCI);
		_rigidBody->setUserPointer((void*)this);
	//}

	return _rigidBody;
}

GameObject::GameObject(Model * model)
{
	_bodyShape = SHAPE_CUBE;
	_model = model;
	_triangleMesh = NULL;
	_boxCollisionShape = NULL;
	_rigidBody = NULL;
	_motionstate = NULL;
	_triangleShape = NULL;
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

btVector3 GameObject::glmVecToBullet(const glm::vec3 & glmVec)
{
	btVector3 bulletVec(glmVec[0], glmVec[1], glmVec[2]);
	return bulletVec;
}

GameObject::~GameObject()
{
	std::cout << "Deleting game object" << std::endl;
	delete _rigidBody;
	delete _motionstate;
	delete _boxCollisionShape;
	//delete _triangleMesh;
	//delete _triangleShape;
}

