#ifndef GAME_OBJECT_H_
#define GAME_OBJECT_H_

#include <glm/glm.hpp>
#include <vector>
#include "model/model.h"
#include "game/physics/physicsengine.h"

class GameObject
{

public:
	GameObject();
	~GameObject();
	btRigidBody * initializeRigidBody();
	void setPosition(glm::vec3 position);
	void setRotation(glm::vec3 rotation);
	void setScale(glm::vec3 rotation);
	void draw();
	int getIdentifier();
private: 
	int _objectIdentifier;
	static int _objectCounter;
	btRigidBody * _rigidBody;
	glm::vec3 _position;
	glm::vec4 _orientation;
	glm::vec3 _scale;
	std::vector<Model> _models;
};

#endif