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
	btRigidBody * initializeRigidBody();
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
	btCollisionShape * _boxCollisionShape;
	btRigidBody * _rigidBody;
	glm::vec3 _position;
	glm::quat _orientation;
	glm::vec3 _scale;
};

#endif