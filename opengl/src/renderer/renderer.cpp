#include <GL/glew.h>
#include "renderer/renderer.h"
#include "utils/debugutils.h"
#include "shader/shader.h"
#include <iostream>


Renderer::Renderer(GLFWwindow * window)
{
	_mainShader = NULL;
	_lineShader = NULL;
	_window = NULL;
	_showFPS = true;
	_framesDrawn = 0;
	_lastDraw = glfwGetTime();
	_lastFPS = _lastDraw;
	glfwGetWindowSize(window, &_width, &_height);
	_antiAlias = true;
	_verticalSync = true;

	glfwWindowHint(GLFW_SAMPLES, 4);

	_window  = window;
	//Make current context
	glfwMakeContextCurrent(_window);
	//Initialize GLEW
	glewExperimental = true; //Needed for core profile
	GLenum glewError = glewInit(); 
	if ( glewError != GLEW_OK) {
		std::cerr << "Failed to initialize GLEW" << std::endl;
		std::cerr << glewGetErrorString(glewError) << std::endl;
	}

	std::cout << "GLEW initialized" << std::endl;

	_mainShader = new Shader("shaders/per_pixel/shader.vert", "shaders/per_pixel/shader.frag");
	_lineShader = new Shader("shaders/per_pixel/line.vert", "shaders/per_pixel/line.frag");

	this->_modelMatrixId = glGetUniformLocation(_mainShader->getId(), "M");
	this->_viewMatrixId = glGetUniformLocation(_mainShader->getId(), "V");
	this->_modelViewProjectionMatrixId = glGetUniformLocation(_mainShader->getId(), "MVP");
	this->_modelViewMatrix3x3Id = glGetUniformLocation(_mainShader->getId(), "MV3x3");
	this->_lineMVPId = glGetUniformLocation(_lineShader->getId(), "MVP");


	//RenderingParams::setWindowSize(width, height);
	std::cout << "Rendering engine initialized" << std::endl;
}

void Renderer::draw(Scene * scene, double currentTime)
{

	
	if ( !_verticalSync || currentTime - _lastDraw >= 1.0/60.0 ){ // If last prinf() was more than 1sec ago
		// printf and reset
		//std::cout << "whatwhat" << std::endl;
		//printf("%f ms/frame\n", 1000.0/double(nbFrames));

        beginDraw();
        scene->draw(_mainShader->getId(), getAspectRatio());
        scene->drawBoundingBoxes(_lineShader->getId(), getAspectRatio());
        glfwSwapBuffers(_window);
        ++_framesDrawn;
        _lastDraw += 1.0/60.0;
        
	}
	if(currentTime - _lastFPS >= 1.0)
	{				
		if(_showFPS)
			printf("%d frame/s\n", _framesDrawn);		
		_framesDrawn = 0;
		_lastFPS += 1.0;
		/* Render here */
		//gameEngine->draw();
        /* Swap front and back buffers */        
	}
	
}
Renderer::~Renderer()
{
	std::cout << "Deleting programs " << std::endl;
	if(_mainShader)
		delete _mainShader;
	if(_lineShader)
		delete _lineShader;
}

void Renderer::setViewMatrix(glm::mat4 viewMatrix)
{

	_viewMatrix = viewMatrix;
}

float Renderer::getAspectRatio()
{
	//std::cout << "Getting AR " << width << " " << height << std::endl;
	float aspectRatio = ((float)_width)/((float)_height);
	return aspectRatio;
}

void Renderer::setWindowSize(int width, int height)
{
	glViewport(0, 0, width, height);
	_width = width;
	_height = height;
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
	return _mainShader->getId();
}

void Renderer::beginDraw()
{
	// Dark blue background
	glClearColor(0.0f, 0.0f, 0.3f, 0.0f);

	// Clear the screen
	glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );

	//Use the shader
	//glUseProgram(_shaderProgramId);
}


GLuint Renderer::getLineProgramId()
{
	return _lineShader->getId();
}

void Renderer::screenToWorld(int mouseX, int mouseY,
		glm::vec3 & outOrigin, glm::vec3 & outDirection)
{

	//std::cout << "Setting initial vectors" << std::endl;
	// The ray Start and End positions, in Normalized Device Coordinates (Have you read Tutorial 4 ?)
	glm::vec4 lRayStart_NDC(
		((float)mouseX/(float)_width  - 0.5f) * 2.0f, // [0,1024] -> [-1,1]
		-((float)mouseY/_height - 0.5f) * 2.0f, // [0, 768] -> [-1,1]
		-1.0, // The near plane maps to Z=-1 in Normalized Device Coordinates
		1.0f
	);
	//std::cout << "Ray start in normalized device coordinates:\t";
	//Debugger::printInfo(lRayStart_NDC);

	glm::vec4 lRayEnd_NDC(
		((float)mouseX/(float)_width  - 0.5f) * 2.0f,
		-((float)mouseY/(float)_height - 0.5f) * 2.0f,
		0.0,
		1.0f
	);
	//std::cout << "Ray end in normalized device coordinates:\t";
	//Debugger::printInfo(lRayEnd_NDC);

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


/*

	std::cout << "Ray start in camera coordinates:\t";
	Debugger::printInfo(lRayStart_camera);
		std::cout << "Ray end in camera coordinates:\t";
	Debugger::printInfo(lRayEnd_camera);



	std::cout << "Ray start in world coordinates:\t";
	Debugger::printInfo(lRayStart_world);
	std::cout << "Ray end in world coordinates:\t";
	Debugger::printInfo(lRayEnd_world);

*/


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