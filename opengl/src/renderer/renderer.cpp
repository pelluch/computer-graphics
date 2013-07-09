#include "renderer/renderer.h"
#include "utils/debugutils.h"
#include "renderer/params.h"
#include "shader/shader.h"
#include <iostream>

void Renderer::init()
{
	Shader shader;
	_shaderProgramId = shader.LoadShaders("shaders/per_pixel/shader.vert", "shaders/per_pixel/shader.frag");
	this->_modelMatrixId = glGetUniformLocation(_shaderProgramId, "M");
	this->_viewMatrixId = glGetUniformLocation(_shaderProgramId, "V");
	this->_modelViewProjectionMatrixId = glGetUniformLocation(_shaderProgramId, "MVP");
	this->_modelViewMatrix3x3Id = glGetUniformLocation(_shaderProgramId, "MV3x3");
}

void Renderer::setViewMatrix(glm::mat4 viewMatrix)
{

	_viewMatrix = viewMatrix;
}

void Renderer::setRenderingParams()
{
	// Enable depth test
	glEnable(GL_DEPTH_TEST);
	glEnable(GL_MULTISAMPLE);
	// Accept fragment if it closer to the camera than the former one
}

void Renderer::setPerspectiveMatrix(glm::mat4 projectionMatrix)
{
	_perspectiveMatrix = projectionMatrix;
}

void Renderer::setModelMatrix(glm::mat4 modelMatrix)
{
	_modelMatrix = modelMatrix;
}

GLuint Renderer::getProgramId()
{
	return _shaderProgramId;
}

void Renderer::beginDraw()
{
	// Dark blue background
	glClearColor(0.0f, 0.0f, 0.3f, 0.0f);

	// Clear the screen
	glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );

	//Use the shader
	glUseProgram(_shaderProgramId);
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


void Renderer::screenToWorld(int mouseX, int mouseY,
		glm::vec3 & outOrigin, glm::vec3 & outDirection)
{

	//std::cout << "Setting initial vectors" << std::endl;
	// The ray Start and End positions, in Normalized Device Coordinates (Have you read Tutorial 4 ?)
	glm::vec4 lRayStart_NDC(
		((float)mouseX/(float)RenderingParams::getWidth()  - 0.5f) * 2.0f, // [0,1024] -> [-1,1]
		((float)mouseY/(float)RenderingParams::getHeight() - 0.5f) * 2.0f, // [0, 768] -> [-1,1]
		-1.0, // The near plane maps to Z=-1 in Normalized Device Coordinates
		1.0f
	);
	glm::vec4 lRayEnd_NDC(
		((float)mouseX/(float)RenderingParams::getWidth()  - 0.5f) * 2.0f,
		((float)mouseY/(float)RenderingParams::getHeight() - 0.5f) * 2.0f,
		0.0,
		1.0f
	);
	

	//std::cout << "Setting inverse projection matrix" << std::endl;
	// The Projection matrix goes from Camera Space to NDC.
	// So inverse(ProjectionMatrix) goes from NDC to Camera Space.
	//Debugger::printInfo(_perspectiveMatrix);
	glm::mat4 inverseProjectionMatrix = glm::inverse(_perspectiveMatrix);
	
	//std::cout << "Setting inverse view matrix" << std::endl;
	// The View Matrix goes from World Space to Camera Space.
	// So inverse(ViewMatrix) goes from Camera Space to World Space.
	glm::mat4 inverseViewMatrix = glm::inverse(_viewMatrix);
	
	glm::vec4 lRayStart_camera = inverseProjectionMatrix * lRayStart_NDC;    lRayStart_camera/=lRayStart_camera.w;
	glm::vec4 lRayStart_world  = inverseViewMatrix       * lRayStart_camera; lRayStart_world /=lRayStart_world .w;
	glm::vec4 lRayEnd_camera   = inverseProjectionMatrix * lRayEnd_NDC;      lRayEnd_camera  /=lRayEnd_camera  .w;
	glm::vec4 lRayEnd_world    = inverseViewMatrix       * lRayEnd_camera;   lRayEnd_world   /=lRayEnd_world   .w;


	// Faster way (just one inverse)
	//glm::mat4 M = glm::inverse(ProjectionMatrix * ViewMatrix);
	//glm::vec4 lRayStart_world = M * lRayStart_NDC; lRayStart_world/=lRayStart_world.w;
	//glm::vec4 lRayEnd_world   = M * lRayEnd_NDC  ; lRayEnd_world  /=lRayEnd_world.w;

	//std::cout << "Setting outputs" << std::endl;

	glm::vec3 lRayDir_world(lRayEnd_world - lRayStart_world);
	lRayDir_world = glm::normalize(lRayDir_world);

	outOrigin = glm::vec3(lRayStart_world);
	outDirection = glm::normalize(lRayDir_world);
}