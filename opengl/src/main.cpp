
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include <iostream>
#include <string>

#include "scene/scene.h"
#include "handlers/control.h"
#include "general_utils.h"
#include "shader/shader.h"
#include "scene/camera.h"
#include "model/model.h"

using namespace std;
static int HEIGHT;
static int WIDTH;

Scene scene;


static GLuint shaderProgramId;
static GLuint vertexBufferId;
static GLuint matrixId;
static GLuint eyeId;
static Shader shader;

ShaderParams params;

std::vector<Model> models;
static void initBuffers()
{
	Model model;
	//std::vector<glm::vec3> vertices 
	model._vertex.push_back(glm::vec3(-1, -1, 1));
	model._vertex.push_back(glm::vec3(1, -1, 0));
	model._vertex.push_back(glm::vec3(0, 1, 0));

	model._diffuseColors.push_back(glm::vec3(1, 0, 0));
	model._diffuseColors.push_back(glm::vec3(1, 0, 0));
	model._diffuseColors.push_back(glm::vec3(1, 0, 0));

	model._normals.push_back(glm::vec3(0, 0, -1));
	model._normals.push_back(glm::vec3(0, 0, -1));
	model._normals.push_back(glm::vec3(0, 0, -1));

	model._worldPosition = glm::vec3(0, 0, 0);
	model._worldRotation = glm::vec3(0, 0, 0);
	model._scale = glm::vec3(1, 1, 1);

	model.initData();
	models.push_back(model);

	model = Model();
	model._vertex.push_back(glm::vec3(-1, -1, 1));
	model._vertex.push_back(glm::vec3(1, -1, 0));
	model._vertex.push_back(glm::vec3(0, 1, 0));

	model._diffuseColors.push_back(glm::vec3(1, 0, 0));
	model._diffuseColors.push_back(glm::vec3(0, 1, 0));
	model._diffuseColors.push_back(glm::vec3(0, 0, 1));

	model._normals.push_back(glm::vec3(0, 0, 1));
	model._normals.push_back(glm::vec3(0, 0, 1));
	model._normals.push_back(glm::vec3(0, 0, 1));

	model._worldPosition = glm::vec3(-1, 0, 0);
	model._worldRotation = glm::vec3(45, 60, 0);
	model._scale = glm::vec3(0.8, 0.8, 0.8);

	model.initData();
	models.push_back(model);

	//Create the buffer to be used in the GPU
	//glGenBuffers(1, &vertexBufferId);

	//Bind the buffer
	//glBindBuffer(GL_ARRAY_BUFFER, vertexBufferId);

	//Assign the data
	//glBufferData(GL_ARRAY_BUFFER, sizeof(vertexBuffer), vertexBuffer, GL_STATIC_DRAW);
}

static void setRenderingParameters()
{
	// Enable depth test
	glEnable(GL_DEPTH_TEST);
	// Accept fragment if it closer to the camera than the former one

}

static void loadShaders()
{
	shaderProgramId = shader.LoadShaders( params );
	matrixId = glGetUniformLocation(shaderProgramId, "viewProjectionMatrix");
	eyeId = glGetUniformLocation(shaderProgramId, "eyePosition");
}

static glm::vec3 eyePosition;

static void loadScene()
{
	Camera cam;
	cam._eye = glm::vec3(0, 0, -4);
	cam._target = glm::vec3(0, 0, 0);
	cam._up = glm::vec3(0, 1, 0);
	cam._fov = 45;
	cam._near = 0.035;
	cam._far = 1500;
	eyePosition = cam._eye;
	scene = Scene(cam);
	Light l1(glm::vec3(1, -1, -0.2), glm::vec3(1,1,1));
	Light l2(glm::vec3(0, 0, -0.2), glm::vec3(1,1,1));
	scene.addLight(l1);
	//scene.addLight(l2);
	scene.setShaderId(shaderProgramId);
	scene.generateIds();
}

static void init()
{

	initBuffers();
	//setRenderingParameters();
	loadShaders();
	loadScene();

}

static void printVector(glm::vec4 vector)
{
	for(int i = 0; i < 4; ++i)
	{
		std::cout << vector[i] << std::endl;
	}
}

static void draw()
{
	// Dark blue background
	glClearColor(0.0f, 0.0f, 0.3f, 0.0f);

	// Clear the screen
	glClear( GL_COLOR_BUFFER_BIT );

	//Use the shader
	glUseProgram(shaderProgramId);

	
	float aspectRatio = (float)WIDTH/(float)HEIGHT;

	glm::mat4 perspectiveTransform = scene.projectionTransform(aspectRatio);
	glm::mat4 viewTransform = scene.viewTransform();
	glm::mat4 viewProjectionMatrix = perspectiveTransform*viewTransform;

	glUniformMatrix4fv(matrixId, 1, GL_FALSE, &viewProjectionMatrix[0][0]);
	scene.bindUniforms();
	glUniform3fv(eyeId, 1, &eyePosition[0]);
	for(int i = 0; i < models.size(); ++i)
	{
		models[i].draw(shaderProgramId);
	}
	
}

int main(int argc, char ** argv)
{
	std::cout << "Initializing GLFW, GLEW" << std::endl;
	std::string caption = "Options";
	utils::CmdLine cmdLine;
	
	cmdLine.init(caption);
	cmdLine.add_option("width,w", utils::ARG_INT, "Window width", true);
	cmdLine.add_option("height,h", utils::ARG_INT, "Window height", true);
	cmdLine.add_option("pixel,p", "Use per pixel shading");
	cmdLine.add_option("vertex,v", "Use per vertex shading");

	int result = cmdLine.parse(argc, argv);
	if(result != 0)
		return -1;

	WIDTH = cmdLine.get_value<int>("width");
	HEIGHT = cmdLine.get_value<int>("height");
	if(cmdLine.exists_value("pixel"))
	{
		std::cout << "Using per pixel mode" << std::endl;
		params.mode = PER_PIXEL;
	}
	else if(cmdLine.exists_value("vertex"))
	{
		std::cout << "Using per vertex mode" << std::endl;
		params.mode = PER_VERTEX;
	}
	else
	{
		std::cerr << "Error: Must choose between vertex and pixel shading" << std::endl;
		return -1;
	}

	//Init GLFW
	if(!glfwInit())
	{
		std::cerr << "Failed to initialize glfw" << std::endl;
		return -1;
	}

	//Window creation
	GLFWwindow * window;
	window = glfwCreateWindow(WIDTH, HEIGHT, "Game", NULL, NULL);
	if(!window)
	{
		std::cerr << "Failed to open GLFW window. If you have an Intel GPU, they are not 3.3 compatible. Try the 2.1 version of the tutorials." << std::endl;
		glfwTerminate();
		return -1;
	}

	//Make current context
	glfwMakeContextCurrent(window);

	//Initialize GLEW
	glewExperimental = true; //Needed for core profile
	GLenum glewError = glewInit(); 
	if ( glewError != GLEW_OK) {
		std::cerr << "Failed to initialize GLEW" << std::endl;
		std::cerr << glewGetErrorString(glewError) << std::endl;
		return -1;
	}

	glfwSetInputMode(window, GLFW_STICKY_KEYS, GLFW_KEY_ESCAPE);
	glfwSetWindowSizeCallback(window, windowResized);

	std::cout << "Initializing" << std::endl;
	init();

	// For speed computation
	double lastTime = glfwGetTime();
	int nbFrames = 0;

	do
	{		

		// Measure speed
		double currentTime = glfwGetTime();
		nbFrames++;
		if ( currentTime - lastTime >= 1.0 ){ // If last prinf() was more than 1sec ago
			// printf and reset
			printf("%f ms/frame\n", 1000.0/double(nbFrames));
			//printf("%d frame/s\n", nbFrames);
			nbFrames = 0;
			lastTime += 1.0;
		}

		/* Render here */
		draw();

        /* Swap front and back buffers */
        glfwSwapBuffers(window);

        /* Poll for and process events */
        glfwPollEvents();
	} while( glfwGetKey(window, GLFW_KEY_ESCAPE) != GLFW_PRESS && 
		   !glfwWindowShouldClose(window));

	glDeleteBuffers(1, &vertexBufferId);
	glfwTerminate();

	return 0;
}