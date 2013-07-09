#ifndef RENDERER_H_
#define RENDERER_H_

#include "GL/glew.h"
#include <glm/glm.hpp>

class Renderer
{
private:
	GLuint _shaderProgramId;
	GLuint _viewMatrixId;
	GLuint _modelViewProjectionMatrixId;
	GLuint _modelViewMatrix3x3Id;
	GLuint _modelMatrixId;
	glm::mat4 _viewMatrix;
	glm::mat4 _perspectiveMatrix;
	glm::mat4 _modelMatrix;
public:
	void init(GLuint shaderProgramId);
	void setViewMatrix(glm::mat4 viewMatrix);
	void setPerspectiveMatrix(glm::mat4 projectionMatrix);
	void setModelMatrix(glm::mat4 modelMatrix);
	void setUniforms();
};

#endif