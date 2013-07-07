#ifndef MODEL_H_
#define MODEL_H_

#include <GL/glew.h>
#include <glm/glm.hpp>
#include <vector>
#include "model/material.h"

class Model
{
	public:
		Model();
		~Model();
		void initData();
		void draw(GLuint shaderProgramId);
		//private:		
		glm::vec3 _worldPosition;
		glm::vec3 _worldRotation;
		glm::vec3 _scale;
		Material _mat;
		std::string _materialName;
		//std::vector< std::vector<float> > _buffers;
		std::vector<GLuint> _bufferIds;
		GLuint _modelMatrixId;
		//Maybe all of this is unnecessary?
		std::vector<glm::vec3> _vertex;
		std::vector<glm::vec3> _normals;
		std::vector<glm::vec2> _textureCoords;
		//std::vector<glm::vec3> _specularColors;
		//std::vector<float> _shininess;
		//std::vector<glm::vec2> _uv;
		
		int _numBuffers;
	
};

#endif