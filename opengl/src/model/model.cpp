#include <GL/glew.h>
#include "model/model.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>
#include "utils/debugutils.h"

Model::Model()
{
	_numBuffers = 3;
	_bufferIds.resize(_numBuffers);
}

void Model::initData()
{
	glGenBuffers(_numBuffers, _bufferIds.data());
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[0]);
	glBufferData(GL_ARRAY_BUFFER, this->_vertices.size()*sizeof(glm::vec3), _vertices.data(), GL_STATIC_DRAW);
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[1]);
	glBufferData(GL_ARRAY_BUFFER, this->_normals.size()*sizeof(glm::vec3), _normals.data(), GL_STATIC_DRAW);
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[2]);
	glBufferData(GL_ARRAY_BUFFER, this->_uvs.size()*sizeof(glm::vec2), _uvs.data(), GL_STATIC_DRAW);
}


void Model::generateUniforms(GLuint shaderProgramId)
{
	this->_modelMatrixId = glGetUniformLocation(shaderProgramId, "modelMatrix");
	this->_transposedInvModelId = glGetUniformLocation(shaderProgramId, "invModelMatrix");
}

void Model::draw(GLuint shaderProgramId)
{
	
	_mat.generateUniformIds(shaderProgramId);
	_mat.setActiveTexture();
	
	glEnableVertexAttribArray(0);
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[0]);
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);

	glEnableVertexAttribArray(1);
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[1]);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);

	glEnableVertexAttribArray(2);
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[2]);
	glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);

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

	glDrawArrays( GL_TRIANGLES, 0, this->_vertices.size());
	
	glDisableVertexAttribArray(0);
	glDisableVertexAttribArray(1);
	glDisableVertexAttribArray(2);
}

Model::~Model()
{
	
}

void Model::computeTangentBasis(std::vector<glm::vec3> & tangents,	std::vector<glm::vec3> & bitangents)
{
	for ( int i=0; i<_vertices.size(); i+=3){

    // Shortcuts for vertices
		glm::vec3 & v0 = _vertices[i+0];
		glm::vec3 & v1 = _vertices[i+1];
		glm::vec3 & v2 = _vertices[i+2];

    // Shortcuts for UVs
		glm::vec2 & uv0 = _uvs[i+0];
		glm::vec2 & uv1 = _uvs[i+1];
		glm::vec2 & uv2 = _uvs[i+2];

    // Edges of the triangle : postion delta
		glm::vec3 deltaPos1 = v1-v0;
		glm::vec3 deltaPos2 = v2-v0;

    // UV delta
		glm::vec2 deltaUV1 = uv1-uv0;
		glm::vec2 deltaUV2 = uv2-uv0;

		float r = 1.0f / (deltaUV1.x * deltaUV2.y - deltaUV1.y * deltaUV2.x);
		glm::vec3 tangent = (deltaPos1 * deltaUV2.y   - deltaPos2 * deltaUV1.y)*r;
		glm::vec3 bitangent = (deltaPos2 * deltaUV1.x   - deltaPos1 * deltaUV2.x)*r;

		  // Set the same tangent for all three vertices of the triangle.
         // They will be merged later, in vboindexer.cpp
		tangents.push_back(tangent);
		tangents.push_back(tangent);
		tangents.push_back(tangent);

  	    // Same thing for binormals
		bitangents.push_back(bitangent);
		bitangents.push_back(bitangent);
		bitangents.push_back(bitangent);

	}
}