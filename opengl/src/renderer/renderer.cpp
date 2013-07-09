#include "renderer/renderer.h"
#include "utils/debugutils.h"

void Renderer::init(GLuint shaderProgramId)
{
	_shaderProgramId = shaderProgramId;
	this->_viewMatrixId = glGetUniformLocation(_shaderProgramId, "V");
	this->_modelViewProjectionMatrixId = glGetUniformLocation(_shaderProgramId, "MVP");
	this->_modelViewMatrix3x3Id = glGetUniformLocation(_shaderProgramId, "MV3x3");
	this->_modelMatrixId = glGetUniformLocation(_shaderProgramId, "M");
}
void Renderer::setViewMatrix(glm::mat4 viewMatrix)
{
	_viewMatrix = viewMatrix;
}

void Renderer::setPerspectiveMatrix(glm::mat4 projectionMatrix)
{
	_perspectiveMatrix = projectionMatrix;
}

void Renderer::setModelMatrix(glm::mat4 modelMatrix)
{
	_modelMatrix = modelMatrix;
}

void Renderer::setUniforms()
{
	glm::mat4 MV = _viewMatrix * _modelMatrix;
	glm::mat3 MV3x3 = glm::mat3(MV);
	glm::mat4 MVP = _perspectiveMatrix * MV;

	
	glUniformMatrix4fv(_viewMatrixId, 1, GL_FALSE, &_viewMatrix[0][0]);
	glUniformMatrix4fv(_modelViewProjectionMatrixId, 1, GL_FALSE, &MVP[0][0]);
	glUniformMatrix4fv(_modelMatrixId, 1, GL_FALSE, &_modelMatrix[0][0]);
	glUniformMatrix4fv(_modelViewMatrix3x3Id, 1, GL_FALSE, &MV3x3[0][0]);
}