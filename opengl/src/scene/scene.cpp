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

Scene::~Scene()
{
	glDeleteBuffers(1, &_rayBufferId);
	glDeleteBuffers(1, &_rayColorBufferId);
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
	std::vector<glm::vec3> lightPositionData;
	//float lightPositionData[3 * _lights.size()];
	size_t numLights = _lights.size();
	for(size_t i = 0; i < numLights; ++i)	{
		glm::vec3 position = _lights[i]._worldPosition; 
		lightPositionData.push_back(position);
	}
	glUniform1i(_numLightsId, numLights);
	glUniform3fv(_lightsId, numLights, &lightPositionData[0][0]);
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

void Scene::draw(GLuint shaderProgramId, float aspectRatio)
{
	glUseProgram(shaderProgramId);
	bindUniforms();

	GLuint viewMatrixId = glGetUniformLocation(shaderProgramId, "V");
	GLuint projectionMatrixId = glGetUniformLocation(shaderProgramId, "P");

	glm::mat4 V = _cameras[0].viewTransform();
	glm::mat4 P = _cameras[0].perspectiveTransform(aspectRatio);

	glUniformMatrix4fv(viewMatrixId, 1, GL_FALSE, &V[0][0]);
	glUniformMatrix4fv(projectionMatrixId, 1, GL_FALSE, &P[0][0]);

	for(size_t i = 0; i < _models.size(); ++i)
	{
		_models[i].draw(shaderProgramId, V, P);
	}
}

void Scene::drawBoundingBoxes(GLuint shaderProgramId, float aspectRatio)
{
	glUseProgram(shaderProgramId);
	
	glm::mat4 V = _cameras[0].viewTransform();
	glm::mat4 P = _cameras[0].perspectiveTransform(aspectRatio);

	if(_rayExists)
	{
	    glm::mat4 M = glm::mat4(1.0);
	    GLuint MVPMatrixId = glGetUniformLocation(shaderProgramId, "MVP");
	    glm::mat4 MVP = P*V*M;
	    glUniformMatrix4fv(MVPMatrixId, 1, GL_FALSE, &MVP[0][0]);
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

		_models[i].drawBoundingBox(shaderProgramId, V, P);
	}
}