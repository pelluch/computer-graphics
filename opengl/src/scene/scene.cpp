#include "scene/scene.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>
#include "utils/debug_utils.h"

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

void Scene::generateIds()
{
	_lightsId = glGetUniformLocation(_shaderProgramId, "lights");
	_numLightsId = glGetUniformLocation(_shaderProgramId, "numLights");
	_cameras[0]._id = glGetUniformLocation(_shaderProgramId, "eyePosition");
	_ambientLightId = glGetUniformLocation(_shaderProgramId, "ambientLight");
}

void Scene::bindUniforms()
{
	float lightPositionData[3 * _lights.size()];
	int numLights = _lights.size();
	for(size_t i = 0; i < numLights; ++i)
	{
		glm::vec3 position = _lights[i]._worldPosition; 
		for(int j = 0; j < 3; ++j)
		{
			lightPositionData[3*i+j] = position[j];
			//std::cout << position[j] << std::endl;
		}
	}
	glUniform1i(_numLightsId, numLights);
	glUniform3fv(_lightsId, numLights, lightPositionData);
	glUniform3fv(_cameras[0]._id, 1, &_cameras[0]._eye[0]);
	glUniform3fv(_ambientLightId, 1, &_ambientLight[0]);
}

void Scene::setMaterials()
{
	for(int i = 0; i < _models.size(); ++i)
	{
		_models[i]._mat = _materials[_models[i]._materialName];
		std::cout << "Material is " << _models[i]._materialName << std::endl;

	}
}

Scene::Scene(Camera & cam)
{
	_cameras.push_back(cam);
}

void Scene::addLight(Light & light)
{
	_lights.push_back(light);
}

void Scene::setShaderId(GLuint id)
{
	this->_shaderProgramId = id;
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

Scene::Scene(Camera & cam, std::vector<Model> & models, std::map<std::string, Material> & materials, std::vector<Light> & lights, glm::vec3 backgroundColor, glm::vec3 ambientLight)
{
	this->_cameras.push_back(cam);
	this->_models = models;
	this->_lights = lights;
	this->_backgroundColor = backgroundColor;
	this->_ambientLight = ambientLight;
	this->_materials = materials;
}

void Scene::initModelData()
{
	for(size_t i = 0; i < _models.size(); ++i)
	{
		_models[i].initData();
	}
}

void Scene::draw(GLuint shaderProgramId)
{
	for(size_t i = 0; i < _models.size(); ++i)
	{
		_models[i].draw(shaderProgramId);
	}
}