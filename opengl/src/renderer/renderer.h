#ifndef RENDERER_H_
#define RENDERER_H_

#include "GL/glew.h"
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include "shader/shader.h"
#include "scene/scene.h"

class Renderer
{
private:
	GLuint _viewMatrixId;
	GLuint _modelViewProjectionMatrixId;
	GLuint _modelViewMatrix3x3Id;
	GLuint _modelMatrixId;
	Shader * _mainShader;
	Shader * _lineShader;
	GLuint _lineMVPId;
	glm::mat4 _viewMatrix;
	glm::mat4 _perspectiveMatrix;
	glm::mat4 _modelMatrix;
	GLFWwindow * _window;
	bool _showFPS;
	int _width;
	int _height;
	bool _antiAlias;
	bool _verticalSync;
	double _lastDraw;
	double _lastFPS;
	int _framesDrawn;
public:
	Renderer(GLFWwindow * window);
	~Renderer();
	void beginDraw();
	void setRenderingParams();
	void draw(Scene * scene, double currentTime);
	GLuint getProgramId();
	GLuint getLineProgramId();
	void setViewMatrix(glm::mat4 viewMatrix);
	void setPerspectiveMatrix(glm::mat4 projectionMatrix);
	void setModelMatrix(glm::mat4 modelMatrix);
	void drawRay(glm::vec3 start, glm::vec3 end);
	void screenToWorld(int mouseX, int mouseY,
		glm::vec3 & outOrigin, glm::vec3 & outDirection);
	void setWindowSize(int width, int height);	
	float getAspectRatio();	
};

#endif