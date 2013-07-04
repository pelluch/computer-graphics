
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include <iostream>
#include <string>

#include "scene.h"
#include "event_handlers.h"
#include "general_utils.h"
#include "shader.h"

using namespace std;
static int HEIGHT;
static int WIDTH;

Scene scene;

static const GLfloat vertexBuffer[] = { 
		//Vertices
		-1.0f, -1.0f, 0.1f,
		 1.0f, -1.0f, 0.2f,
		 0.0f,  1.0f, 0.3f,
		 //Colors
		 1.0f, 0.0f, 0.0f,
		 0.0f, 1.0f, 0.0f,
		 0.0f, 0.0f, 1.0f,
		 //Normals
		 1.0f, 0.0f, 0.0f,
		 0.0f, 1.0f, 0.0f,
		 0.0f, 0.0f, 1.0f
	};

static GLuint shaderProgramId;
static GLuint vertexBufferId;



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
	glDepthFunc(GL_LESS);

}

static void loadShaders()
{
	shaderProgramId = LoadShaders( "basic.vert", "basic.frag" );
}

static void init()
{
	initBuffers();
	//setRenderingParameters();
	loadShaders();

}

static void draw()
{
	// Dark blue background
	glClearColor(0.0f, 0.0f, 0.4f, 0.0f);

	// Clear the screen
	glClear( GL_COLOR_BUFFER_BIT );

	//Use the shader
	glUseProgram(shaderProgramId);

	glm::mat4 modelTransform = glm::mat4(1.0f);
	
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