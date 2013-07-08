#include "scene/camera.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>
#include "utils/debugutils.h"

Camera::Camera()
{
	_fov = 45.0f;
	_near = 0.01f;
	_far = 100.0f;
	_eye = glm::vec3(0, 0, 0);
	_target = glm::vec3(0, 0, 1);
	_up = glm::vec3(0, 1, 0);
}

Camera::Camera(float fov, float near, float far,
	glm::vec3 eye, glm::vec3 target, glm::vec3 up)
{
	_fov = fov;
	_near = near;
	_far = far;
	_eye = eye;
	_target = target;
	_up = up;
}
void Camera::printInfo()
{
	Debugger::printInfo(_eye);
	Debugger::printInfo(_target);
	Debugger::printInfo(_up);
	std::cout << "near: " << _near << std::endl;
	std::cout << "far: " << _far << std::endl;
}

void Camera::generateId(GLuint shaderProgramId)
{
	_id = glGetUniformLocation(shaderProgramId, "eyePosition");
}

glm::mat4 Camera::perspectiveTransform(float aspectRatio)
{
	glm::mat4 perspectiveMatrix = glm::perspective(_fov, aspectRatio,
	_near, _far);
	return perspectiveMatrix;
}

glm::mat4 Camera::viewTransform()
{
	glm::mat4 transformMatrix = glm::lookAt(_eye, _target, _up);
	return transformMatrix;
}

void Camera::assignUniformData()
{
	glUniform3fv(_id, 1, &_eye[0]);
}

void Camera::moveCamera(glm::vec3 translation, glm::vec3 rotation)
{
	_eye += translation;
	_target += translation;

	glm::mat4 rotationMatrix = glm::rotate(glm::mat4(1.0f), rotation[0], glm::vec3(1, 0, 0)) *
		glm::rotate(glm::mat4(1.0f), rotation[1], glm::vec3(0, 1, 0)) *
		glm::rotate(glm::mat4(1.0f), rotation[2], glm::vec3(0, 0, 1));

	Debugger::printInfo(rotationMatrix);
	glm::vec4 translatedTarget = rotationMatrix * glm::vec4(_target[0], _target[1],
		_target[2], 1.0f);
	_target = glm::vec3(translatedTarget[0], translatedTarget[1], translatedTarget[2]);
}