#ifndef MODEL_H_
#define MODEL_H_

#include <GL/glew.h>
#include <glm/glm.hpp>
#include <vector>
#include "model/model.h"
#include "model/material.h"
#include "renderer/renderer.h"

class Model
{
	public:
		Model();
		~Model();
		void initData();
		void draw(GLuint shaderProgramId, Renderer & renderer);
		void generateUniforms(GLuint shaderProgramId);
		glm::vec3 _worldPosition;
		glm::vec3 _worldRotation;
		glm::vec3 _scale;
		Material _mat;
		std::string _materialName;
		//Maybe all of this is unnecessary?
		std::vector<glm::vec3> _vertices;
		std::vector<glm::vec3> _normals;
		std::vector<glm::vec2> _uvs;
		std::vector<glm::vec3> _tangents;
		std::vector<glm::vec3> _bitangents;		
		//std::vector<glm::vec3> _specularColors;
		//std::vector<float> _shininess;
	private:
		GLuint _transposedInvModelId;
		GLuint _modelMatrixId;
		int _numBuffers;
		std::vector<GLuint> _bufferIds;
		void computeTangentBasis();
		std::vector<glm::vec3> _indexedVertices;
		std::vector<glm::vec2> _indexedUvs;
		std::vector<glm::vec3> _indexedNormals;
		std::vector<glm::vec3> _indexedTangents;
		std::vector<glm::vec3> _indexedBitangents;
		std::vector<unsigned short> _indices;

};

#endif