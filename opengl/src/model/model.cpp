#include <GL/glew.h>
#include "model/model.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>
#include "utils/debugutils.h"

Model::Model()
{
	_numBuffers = 2;
	_bufferIds.resize(_numBuffers);
}

void Model::initData()
{
	glGenBuffers(_numBuffers, _bufferIds.data());
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[0]);
	glBufferData(GL_ARRAY_BUFFER, this->_vertex.size()*sizeof(glm::vec3), _vertex.data(), GL_STATIC_DRAW);
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[1]);
	glBufferData(GL_ARRAY_BUFFER, this->_normals.size()*sizeof(glm::vec3), _normals.data(), GL_STATIC_DRAW);
}


void Model::generateUniforms(GLuint shaderProgramId)
{
	this->_modelMatrixId = glGetUniformLocation(shaderProgramId, "modelMatrix");
	this->_transposedInvModelId = glGetUniformLocation(shaderProgramId, "invModelMatrix");
}

void Model::draw(GLuint shaderProgramId)
{
	
	_mat.generateUniformIds(shaderProgramId);

	glEnableVertexAttribArray(0);
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[0]);
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);

	glEnableVertexAttribArray(1);
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[1]);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);

	glm::mat4 modelMatrix = glm::translate(glm::mat4(1.0f), this->_worldPosition) *
	glm::rotate(glm::mat4(1.0f), _worldRotation[0], glm::vec3(1.0f, 0.0f, 0.0f)) *
	glm::rotate(glm::mat4(1.0f), _worldRotation[1], glm::vec3(0.0f, 1.0f, 0.0f)) *
	glm::rotate(glm::mat4(1.0f), _worldRotation[2], glm::vec3(0.0f, 0.0f, 1.0f)) *	
	glm::scale(glm::mat4(1.0f), _scale);
	glm::mat4 invModelMatrix = glm::transpose(glm::inverse(modelMatrix));

	//Debugger::printInfo(modelMatrix);
	glUniform3fv(_mat._diffuseColorId, 1, &_mat._diffuseColor[0]);
	glUniform4fv(_mat._specularColorId, 1, &_mat._specularColor[0]);
	
	//Debugger::printInfo(_mat._specularColor);
	glUniformMatrix4fv(_modelMatrixId, 1, GL_FALSE, &modelMatrix[0][0]);
	glUniformMatrix4fv(_transposedInvModelId, 1, GL_FALSE, &invModelMatrix[0][0]);

	//std::cout << "Model matrix:" << std::endl;
	//Debugger::printInfo(modelMatrix);
	//std::cout << "Inversed: " << std::endl;
	//Debugger::printInfo(invModelMatrix);

	glDrawArrays( GL_TRIANGLES, 0, this->_vertex.size());
	
	glDisableVertexAttribArray(0);
	glDisableVertexAttribArray(1);
}

Model::~Model()
{
	
}