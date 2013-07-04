#include "scene/scene.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>

Scene::Scene(float fov, float near, float far, glm::vec3 eye, glm::vec3 target, glm::vec3 up)
{
	Camera cam;
	cam._eye = eye;
	cam._far = far;
	cam._fov = fov;
	cam._near = near;
	cam._target = target;
	cam._up = up;
	_cameras.push_back(cam);
}

Scene::Scene()
{
	
}

Scene::Scene(Camera & cam)
{
	_cameras.push_back(cam);
}

glm::mat4 Scene::projectionTransform(float aspectRatio, int camIndex)
{
	glm::mat4 projectionMatrix = glm::perspective(_cameras[camIndex]._fov, aspectRatio,
	 _cameras[camIndex]._near, _cameras[camIndex]._far);
	return projectionMatrix;
}

glm::mat4 Scene::viewTransform(int camIndex)
{
	glm::mat4 viewMatrix = _cameras[camIndex].viewTransform();
	return viewMatrix;
}
