#ifndef RENDERER_H_
#define RENDERER_H_

#include "GL/glew.h"
#include <glm/glm.hpp>

class Renderer
{
private:
	GLuint _lineProgramId;
	GLuint _shaderProgramId;
	GLuint _viewMatrixId;
	GLuint _modelViewProjectionMatrixId;
	GLuint _modelViewMatrix3x3Id;
	GLuint _modelMatrixId;

	GLuint _lineMVPId;
	glm::mat4 _viewMatrix;
	glm::mat4 _perspectiveMatrix;
	glm::mat4 _modelMatrix;
public:
	void beginDraw();
	void init();
	void setRenderingParams();
	GLuint getProgramId();
	GLuint getLineProgramId();
	void setViewMatrix(glm::mat4 viewMatrix);
	void setPerspectiveMatrix(glm::mat4 projectionMatrix);
	void setModelMatrix(glm::mat4 modelMatrix);
	void setUniforms();
	void setBoxUniforms();
	void drawRay(glm::vec3 start, glm::vec3 end);
	void screenToWorld(int mouseX, int mouseY,
		glm::vec3 & outOrigin, glm::vec3 & outDirection);
};

#endif