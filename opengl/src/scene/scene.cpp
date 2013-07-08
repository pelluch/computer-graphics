#include "scene/scene.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>
#include "utils/debugutils.h"

Scene::Scene(float fov, float near, float far, glm::vec3 eye, glm::vec3 target, glm::vec3 up)
{
	currentCam = 0;
	Camera cam(fov, near, far, eye, target, up);	
	_cameras.push_back(cam);

}

Scene::Scene()
{
	currentCam = 0;
}

void Scene::generateIds()
{
	_lightsId = glGetUniformLocation(_shaderProgramId, "lights");
	_numLightsId = glGetUniformLocation(_shaderProgramId, "numLights");
	std::cout << "Current cam: " << currentCam << std::endl;
	_cameras[currentCam].generateId(_shaderProgramId);
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
	_cameras[currentCam].assignUniformData();
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
	currentCam = 0;
	_cameras.push_back(cam);
}

void Scene::addLight(Light & light)
{
	_lights.push_back(light);
}

void Scene::moveCamera(glm::vec3 translation, glm::vec3 rotation)
{
	_cameras[currentCam].moveCamera(translation, rotation);
}

void Scene::setShaderId(GLuint id)
{
	this->_shaderProgramId = id;
	for(int i = 0; i < _models.size(); ++i)
	{
		_models[i].generateUniforms(id);
	}
}

glm::mat4 Scene::projectionTransform(float aspectRatio, int camIndex)
{
	glm::mat4 projectionMatrix = _cameras[camIndex].perspectiveTransform(aspectRatio);
	return projectionMatrix;
}

glm::mat4 Scene::viewTransform(int camIndex)
{
	glm::mat4 viewMatrix = _cameras[camIndex].viewTransform();
	return viewMatrix;
}

Scene::Scene(Camera & cam, std::vector<Model> & models, std::map<std::string, Material> & materials, std::vector<Light> & lights, glm::vec3 backgroundColor, glm::vec3 ambientLight)
{
	currentCam = 0;
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