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
	void beginDraw();
	void init();
	void setRenderingParams();
	GLuint getProgramId();
	void setViewMatrix(glm::mat4 viewMatrix);
	void setPerspectiveMatrix(glm::mat4 projectionMatrix);
	void setModelMatrix(glm::mat4 modelMatrix);
	void setUniforms();
	void screenToWorld(int mouseX, int mouseY,
		glm::vec3 & outOrigin, glm::vec3 & outDirection);
};

#endif