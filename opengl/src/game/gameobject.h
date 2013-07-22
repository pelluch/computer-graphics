#ifndef GAME_OBJECT_H_
#define GAME_OBJECT_H_

#include <glm/glm.hpp>
#include <glm/gtc/quaternion.hpp>
#include <glm/gtx/quaternion.hpp>
#include <vector>
#include "model/model.h"
#include "game/physics/physicsengine.h"

class GameObject
{

public:
	GameObject(Model * model);
	~GameObject();
	btRigidBody * initializeRigidBody(BODY_SHAPE shape = SHAPE_CUBE);
	void setPosition(glm::vec3 position);
	void setRotation(glm::vec3 rotation);
	void setScale(glm::vec3 rotation);
	void draw();
	int getIdentifier();
	std::string getName();
private: 
	Model * _model;
	int _objectIdentifier;
	static int _objectCounter;
	btDefaultMotionState * _motionstate;
	btTriangleMesh * _triangleMesh;
	btBvhTriangleMeshShape * _triangleShape;
	btCollisionShape * _boxCollisionShape;
	btRigidBody * _rigidBody;
	BODY_SHAPE _bodyShape;
	glm::vec3 _position;
	glm::quat _orientation;
	glm::vec3 _scale;
	btVector3 glmVecToBullet(const glm::vec3 & glmVec);
};

#endif