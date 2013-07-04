
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

using namespace std;
static int HEIGHT;
static int WIDTH;

Scene scene;

static const GLfloat vertexBuffer[] = { 

		 //Vertices
		 -1.0f, -1.0f, 0.0f,
		 1.0f, -1.0f, 0.0f,
		 0.0f,  1.0f, 0.0f,
		 //Colors
		 1.0f, 0.0f, 0.0f,
		 1.0f, 0.0f, 0.0f,
		 1.0f, 0.0f, 0.0f,
		 //Normals
		 0.0f, 0.0f, -1.0f,
		 0.0f, 0.0f, -1.0f,
		 0.0f, 0.0f, -1.0f

	};

static GLuint shaderProgramId;
static GLuint vertexBufferId;
static GLuint matrixId;
static GLuint lightId;
static GLuint eyeId;
static glm::vec3 lightPosition;
static Shader shader;

static void initBuffers()
{
	//Create the buffer to be used in the GPU
	glGenBuffers(1, &vertexBufferId);

	//Bind the buffer
	glBindBuffer(GL_ARRAY_BUFFER, vertexBufferId);

	//Assign the data
	glBufferData(GL_ARRAY_BUFFER, sizeof(vertexBuffer), vertexBuffer, GL_STATIC_DRAW);
}

static void setRenderingParameters()
{
	// Enable depth test
	glEnable(GL_DEPTH_TEST);
	// Accept fragment if it closer to the camera than the former one

}

static void loadShaders()
{
	ShaderParams params;
	params.mode = PER_VERTEX;
	shaderProgramId = shader.LoadShaders( params );
	matrixId = glGetUniformLocation(shaderProgramId, "MVP");
	lightId = glGetUniformLocation(shaderProgramId, "lightPosition");
	eyeId = glGetUniformLocation(shaderProgramId, "eyePosition");
}

static glm::vec3 eyePosition;

static void loadScene()
{
	Camera cam;
	cam._eye = glm::vec3(1.2, 1.2,-1);
	cam._target = glm::vec3(-1,-1, -0.2);
	cam._up = glm::vec3(0, 1, 0);
	cam._fov = 45;
	cam._near = 0.035;
	cam._far = 1500;
	eyePosition = cam._eye;
	scene = Scene(cam);
	lightPosition = glm::vec3(0, 0, -0.1);
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

	glm::mat4 modelTransform = glm::mat4(1.0f);
	glm::mat4 perspectiveTransform = scene.projectionTransform(aspectRatio);
	glm::mat4 viewTransform = scene.viewTransform();
	glm::mat4 MVP = perspectiveTransform*viewTransform*modelTransform;

	glUniformMatrix4fv(matrixId, 1, GL_FALSE, &MVP[0][0]);
	glUniform3fv(lightId, 1, &lightPosition[0]);
	glUniform3fv(eyeId, 1, &eyePosition[0]);

	//First attribute buffer: vertices
	glEnableVertexAttribArray(0);
	glEnableVertexAttribArray(1);
	glEnableVertexAttribArray(2);

	glBindBuffer(GL_ARRAY_BUFFER, vertexBufferId);
	glVertexAttribPointer(
		0,	//Attrib 0 (must much layout in shader)
		3,	//size
		GL_FLOAT,	//type
		GL_FALSE,	//normalized?
		0,	//stride
		(void*)0	//Array buffer offset
	);
	glVertexAttribPointer(
		1,	//Attrib 0 (must much layout in shader)
		3,	//size
		GL_FLOAT,	//type
		GL_FALSE,	//normalized?
		0,	//stride
		(void*)(9*sizeof(float))	//Array buffer offset
	);
	glVertexAttribPointer(
		2,	//Attrib 0 (must much layout in shader)
		3,	//size
		GL_FLOAT,	//type
		GL_FALSE,	//normalized?
		0,	//stride
		(void*)(18*sizeof(float))	//Array buffer offset
	);

	//Draw triangle
	glDrawArrays(GL_TRIANGLES, 0, 3); //Starting from 0, 3 vertices

	glDisableVertexAttribArray(0);
	glDisableVertexAttribArray(1);
	glEnableVertexAttribArray(2);
}

int main(int argc, char ** argv)
{
	std::cout << "Initializing GLFW, GLEW" << std::endl;
	std::string caption = "Options";
	utils::CmdLine cmdLine;

	cmdLine.init(caption);
	cmdLine.add_option("width,w", utils::ARG_INT, "Window width", true);
	cmdLine.add_option("height,h", utils::ARG_INT, "Window height", true);

	
	int result = cmdLine.parse(argc, argv);
	if(result != 0)
		return -1;

	WIDTH = cmdLine.get_value<int>("width");
	HEIGHT = cmdLine.get_value<int>("height");

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