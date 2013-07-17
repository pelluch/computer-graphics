#include <GL/glew.h>
#include "model/model.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>
#include "utils/debugutils.h"
#include "utils/vboindexer.h"
#include "renderer/renderer.h"


Model::Model()
{

	_numBuffers = 8;
	_bufferIds.resize(_numBuffers);
	std::cout << "adding box points" << std::endl;
	for(int i = 0; i <= 2; i+=2)
	{
		for(int j = 0; j <= 2; j +=2)
		{
			for(int k = 0; k <=2; k+=2)
			{
				_boxPoints.push_back(glm::vec3(i,j,k));
				_boxPoints.push_back(glm::vec3(i,(j+1)%3,k));

				_boxPoints.push_back(glm::vec3(i,j,k));
				_boxPoints.push_back(glm::vec3(i,j,(k+1)%3));

				_boxPoints.push_back(glm::vec3(i,j,k));
				_boxPoints.push_back(glm::vec3((i+1)%3,j,k));

				//Debugger::printInfo(glm::vec3(i,j,k));
			}
		}
	}

	for(size_t i = 0; i < _boxPoints.size(); i++)
	{
		_boxPointColors.push_back(glm::vec3(1,0,0));
		_boxPoints[i] = _boxPoints[i] - glm::vec3(1, 1, 1);
	}

}


void Model::initData()
{
	computeTangentBasis();
	VBOIndexer::indexVBO_TBN(
		_vertices, _uvs, _normals, _tangents, _bitangents, 
		_indices, _indexedVertices, _indexedUvs, _indexedNormals, _indexedTangents, _indexedBitangents
	);

	glGenBuffers(_numBuffers, _bufferIds.data());
	//Vertices
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[0]);
	glBufferData(GL_ARRAY_BUFFER, this->_indexedVertices.size()*sizeof(glm::vec3), _indexedVertices.data(), GL_STATIC_DRAW);
	//uvs
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[1]);
	glBufferData(GL_ARRAY_BUFFER, this->_indexedUvs.size()*sizeof(glm::vec2), _indexedUvs.data(), GL_STATIC_DRAW);
	//normals
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[2]);
	glBufferData(GL_ARRAY_BUFFER, this->_indexedNormals.size()*sizeof(glm::vec3), _indexedNormals.data(), GL_STATIC_DRAW);
	//tangents
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[3]);
	glBufferData(GL_ARRAY_BUFFER, this->_indexedTangents.size()*sizeof(glm::vec3), _indexedTangents.data(), GL_STATIC_DRAW);
	//bitangents
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[4]);
	glBufferData(GL_ARRAY_BUFFER, this->_indexedBitangents.size()*sizeof(glm::vec3), _indexedBitangents.data(), GL_STATIC_DRAW);
	//elementbuffer
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[5]);
	glBufferData(GL_ARRAY_BUFFER, this->_indices.size()*sizeof(unsigned int short), _indices.data(), GL_STATIC_DRAW);
	//linesbuffer
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[6]);
	glBufferData(GL_ARRAY_BUFFER, this->_boxPoints.size()*sizeof(glm::vec3), _boxPoints.data(), GL_STATIC_DRAW);
	//linesbuffer
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[7]);
	glBufferData(GL_ARRAY_BUFFER, this->_boxPointColors.size()*sizeof(glm::vec3), _boxPointColors.data(), GL_STATIC_DRAW);
}

void Model::generateUniforms(GLuint shaderProgramId)
{
	this->_modelMatrixId = glGetUniformLocation(shaderProgramId, "modelMatrix");
	this->_transposedInvModelId = glGetUniformLocation(shaderProgramId, "invModelMatrix");
}

void Model::drawBoundingBox(GLuint shaderProgramId, Renderer & renderer)
{
	//glUseProgram(shaderProgramId);
	//generateUniforms(shaderProgramId);
	//std::cout << "Drawing bounding box" << std::endl;

	glm::mat4 modelMatrix = glm::translate(glm::mat4(1.0f), this->_worldPosition) *
	glm::rotate(glm::mat4(1.0f), _worldRotation[0], glm::vec3(1.0f, 0.0f, 0.0f)) *
	glm::rotate(glm::mat4(1.0f), _worldRotation[1], glm::vec3(0.0f, 1.0f, 0.0f)) *
	glm::rotate(glm::mat4(1.0f), _worldRotation[2], glm::vec3(0.0f, 0.0f, 1.0f)) *	
	glm::scale(glm::mat4(1.0f), _scale);

	//glm::mat4 invModelMatrix = glm::transpose(glm::inverse(modelMatrix));

	//Debugger::printInfo(modelMatrix);
	//glUniform3fv(_mat._diffuseColorId, 1, &_mat._diffuseColor[0]);
	//glUniform4fv(_mat._specularColorId, 1, &_mat._specularColor[0]);
	renderer.setModelMatrix(modelMatrix);
	renderer.setBoxUniforms();


	glEnableVertexAttribArray(6);
	glEnableVertexAttribArray(7);

	//std::cout << "dsadsadsa" << std::endl;
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[6]);
	glVertexAttribPointer(6, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
	//glDrawArrays(GL_LINES, 0, _boxPoints.size());


	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[7]);
	glVertexAttribPointer(7, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
	glDrawArrays(GL_LINES, 0, _boxPoints.size());

	glDisableVertexAttribArray(6);
	glDisableVertexAttribArray(7);
}

void Model::draw(GLuint shaderProgramId, Renderer & renderer)
{

	//glUseProgram(shaderProgramId);
	
	_mat.generateUniformIds(shaderProgramId);
	_mat.setActiveTexture();
	
	glEnableVertexAttribArray(0);
	glEnableVertexAttribArray(1);
	glEnableVertexAttribArray(2);
	glEnableVertexAttribArray(3);
	glEnableVertexAttribArray(4);


	//Vertices
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[0]);
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
	//uvs
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[1]);
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);
	//normals
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[2]);
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
	//tangents
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[3]);
	glVertexAttribPointer(3, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
	//bitangents
	glBindBuffer(GL_ARRAY_BUFFER, _bufferIds[4]);
	glVertexAttribPointer(4, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);

	glm::mat4 modelMatrix = glm::translate(glm::mat4(1.0f), this->_worldPosition) *
	glm::rotate(glm::mat4(1.0f), _worldRotation[0], glm::vec3(1.0f, 0.0f, 0.0f)) *
	glm::rotate(glm::mat4(1.0f), _worldRotation[1], glm::vec3(0.0f, 1.0f, 0.0f)) *
	glm::rotate(glm::mat4(1.0f), _worldRotation[2], glm::vec3(0.0f, 0.0f, 1.0f)) *	
	glm::scale(glm::mat4(1.0f), _scale);

	//glm::mat4 invModelMatrix = glm::transpose(glm::inverse(modelMatrix));

	//Debugger::printInfo(modelMatrix);
	glUniform3fv(_mat._diffuseColorId, 1, &_mat._diffuseColor[0]);
	glUniform4fv(_mat._specularColorId, 1, &_mat._specularColor[0]);
	renderer.setModelMatrix(modelMatrix);
	renderer.setUniforms();
	//Debugger::printInfo(_mat._specularColor);
	//glUniformMatrix4fv(_modelMatrixId, 1, GL_FALSE, &modelMatrix[0][0]);
	//glUniformMatrix4fv(_transposedInvModelId, 1, GL_FALSE, &invModelMatrix[0][0]);

	// Index buffer
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, _bufferIds[5]);

	// Draw the triangles !
	glDrawElements(GL_TRIANGLES, _indices.size(), GL_UNSIGNED_SHORT, (void*)0);
	//glDrawArrays( GL_TRIANGLES, 0, this->_vertices.size());
	
	glDisableVertexAttribArray(0);
	glDisableVertexAttribArray(1);
	glDisableVertexAttribArray(2);
	glDisableVertexAttribArray(3);
	glDisableVertexAttribArray(4);

}

Model::~Model()
{
	
}

void Model::computeTangentBasis()
{
	for ( size_t i=0; i<_vertices.size(); i+=3){

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
		_tangents.push_back(tangent);
		_tangents.push_back(tangent);
		_tangents.push_back(tangent);

  	    // Same thing for binormals
		_bitangents.push_back(bitangent);
		_bitangents.push_back(bitangent);
		_bitangents.push_back(bitangent);

	}
}