#ifndef CAMERA_H_
#define CAMERA_H_

#include <glm/glm.hpp>


class Camera
{
public:
	Camera();
	glm::mat4 viewTransform();
	float _fov;	//Must be in degrees
	float _near;
	float _far;
	glm::vec3 _eye;
	glm::vec3 _target;
	glm::vec3 _up;
};

#endif