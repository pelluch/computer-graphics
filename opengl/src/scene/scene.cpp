#include "scene/scene.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>
#include "utils/debugutils.h"


Scene::Scene(Camera & cam, std::vector<Model> & models, std::map<std::string, Material> & materials, std::vector<Light> & lights, glm::vec3 backgroundColor, glm::vec3 ambientLight)
{
	currentCam = 0;
	this->_cameras.push_back(cam);
	this->_models = models;
	this->_lights = lights;
	this->_backgroundColor = backgroundColor;
	this->_ambientLight = ambientLight;
	this->_materials = materials;
	_rayExists = false;
	glGenBuffers(1, &_rayBufferId);
	glGenBuffers(1, &_rayColorBufferId);
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
	size_t numLights = _lights.size();
	for(size_t i = 0; i < numLights; ++i)
	{
		glm::vec3 position = _lights[i]._worldPosition; 
		for(size_t j = 0; j < 3; ++j)
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
	for(size_t i = 0; i < _models.size(); ++i)
	{
		_models[i]._mat = _materials[_models[i]._materialName];
		//std::cout << "Material is " << _models[i]._materialName << std::endl;

	}
}

void Scene::drawRay(glm::vec3 start, glm::vec3 end)
{
	_rayExists = true;
	_ray.clear();
	_ray.push_back(start);
	_ray.push_back(end);

	_ray.push_back(glm::vec3(-100, end[1], end[2]));
	_ray.push_back(glm::vec3(100, end[1], end[2]));

	glBindBuffer(GL_ARRAY_BUFFER, _rayBufferId);
	glBufferData(GL_ARRAY_BUFFER, this->_ray.size()*sizeof(glm::vec3), _ray.data(), GL_STATIC_DRAW);

	std::vector<glm::vec3> rayColors;
	rayColors.push_back(glm::vec3(0,0,1));
	rayColors.push_back(glm::vec3(1,0,0));

	rayColors.push_back(glm::vec3(0,0,1));
	rayColors.push_back(glm::vec3(1,0,0));

	glBindBuffer(GL_ARRAY_BUFFER, _rayColorBufferId);
	glBufferData(GL_ARRAY_BUFFER, rayColors.size()*sizeof(glm::vec3), rayColors.data(), GL_STATIC_DRAW);
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
	for(size_t i = 0; i < _models.size(); ++i)
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

void Scene::initModelData()
{
	for(size_t i = 0; i < _models.size(); ++i)
	{
		_models[i].initData();
	}
}

void Scene::draw(GLuint shaderProgramId, Renderer & renderer)
{
	glUseProgram(shaderProgramId);
	for(size_t i = 0; i < _models.size(); ++i)
	{
		_models[i].draw(shaderProgramId, renderer);
	}

}

void Scene::drawBoundingBoxes(GLuint shaderProgramId, Renderer & renderer)
{

	glUseProgram(shaderProgramId);

	if(_rayExists)
	{

		//std::cout << "Drawing ray" << std::endl;
		renderer.setModelMatrix(glm::mat4(1.0f));
		renderer.setBoxUniforms();

		glEnableVertexAttribArray(6);
		glEnableVertexAttribArray(7);

		glBindBuffer(GL_ARRAY_BUFFER, _rayBufferId);
		glVertexAttribPointer(6, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
		//glDrawArrays(GL_LINES, 0, _ray.size());

	
		glBindBuffer(GL_ARRAY_BUFFER, _rayColorBufferId);
		glVertexAttribPointer(7, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);

		glDrawArrays(GL_LINES, 0, _ray.size());

		glDisableVertexAttribArray(6);
		glDisableVertexAttribArray(7);	
	}
	
	for(size_t i = 0; i < _models.size(); ++i)
	{

		_models[i].drawBoundingBox(shaderProgramId, renderer);
	}
}