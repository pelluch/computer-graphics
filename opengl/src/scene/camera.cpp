#include "scene/camera.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>

Camera::Camera()
{
	_fov = 45.0f;
	_near = 0.01f;
	_far = 100.0f;
	_eye = glm::vec3(0, 0, 0);
}

glm::mat4 Camera::viewTransform()
{
	glm::mat4 transformMatrix = glm::lookAt(_eye, _target, _up);
	return transformMatrix;
}

