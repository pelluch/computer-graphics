
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include <iostream>
#include <string>
#include <map>

#include "scene/scene.h"
#include "handlers/control.h"
#include "general_utils.h"
#include "shader/shader.h"
#include "scene/camera.h"
#include "model/model.h"
#include "utils/xmlloader.h"
#include "utils/debugutils.h"
#include "utils/settings.h"
#include "game/gameengine.h"

using namespace std;



void menu(std::string & sceneFile, std::string & vertexShader, std::string & fragmentShader)
{
	std::map<int, std::string> sceneMap;
	std::map<int, std::string> shaderMap;

	std::vector<std::string> sceneFiles = utils::getFileList("scenes", true);
	std::vector<std::string> shaders = utils::getFileList("shaders", true);
	std::cout << "Pick a scene to load: " << std::endl;

	int sceneChoice;
	for(size_t i = 0; i < sceneFiles.size(); ++i)
	{
		sceneMap[i] = sceneFiles[i];
		std::cout << i << ". " << sceneFiles[i] << std::endl;
	}
	std::cin >> sceneChoice;
	sceneFile = sceneMap[sceneChoice];

	int vertexChoice, fragmentChoice;
	for(size_t i = 0; i < shaders.size(); ++i)
	{
		shaderMap[i] = shaders[i];
		std::cout << i << ". " << shaders[i] << std::endl;
	}
	std::cout << "Choose vertex shader" << std::endl;
	std::cin >> vertexChoice;
	std::cout << "Choose fragment shader" << std::endl;
	std::cin >> fragmentChoice;

	vertexShader = shaderMap[vertexChoice];
	fragmentShader = shaderMap[fragmentChoice];
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

	//std::string sceneFile, vertexShader, fragmentShader;
	//menu(sceneFile, vertexShader, fragmentShader);


	int result = cmdLine.parse(argc, argv);
	if(result != 0)
		return -1;

	int width = cmdLine.get_value<int>("width");
	int height = cmdLine.get_value<int>("height");
	RenderingParams::setWindowSize(width, height);

	if(cmdLine.exists_value("pixel"))
	{
		std::cout << "Using per pixel mode" << std::endl;
		RenderingParams::mode = PER_PIXEL;
	}
	else if(cmdLine.exists_value("vertex"))
	{
		std::cout << "Using per vertex mode" << std::endl;
		RenderingParams::mode = PER_VERTEX;
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
	glfwWindowHint(GLFW_SAMPLES, 4);
	window = glfwCreateWindow(width, height, "Game", NULL, NULL);
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
	glfwSetWindowSizeCallback(window, Control::windowResized);
	glfwSetKeyCallback(window, Control::keyCallBack);
	glfwSetCursorPosCallback(window, Control::mousePosCallback);
	glfwSetScrollCallback(window, Control::mouseScrollCallback);
	glfwSetCursorPos(window, width/2, height/2);
	glfwSetMouseButtonCallback(window, Control::mouseClickCallback);
	
	boost::shared_ptr<GameEngine>  gameEngine(new GameEngine());
	Control::setGameEngine(gameEngine);
	
	std::cout << "Initializing" << std::endl;

	// For speed computation
	double lastFrameSwap = glfwGetTime();
	double lastCounter = glfwGetTime();
	double lastTime = lastCounter;
	int nbFrames = 0;
	glfwSwapInterval(0);
	do
	{		
		if(!RenderingParams::paused)
		{
				// Measure speed
			double currentTime = glfwGetTime();
			
			if ( currentTime - lastFrameSwap >= 1.0/60.0 ){ // If last prinf() was more than 1sec ago
				// printf and reset
				//std::cout << "whatwhat" << std::endl;
				//printf("%f ms/frame\n", 1000.0/double(nbFrames));
				nbFrames++;
				lastFrameSwap = currentTime;;
				gameEngine->draw();
		        glfwSwapBuffers(window);
			}
			if(currentTime - lastCounter >= 1.0)
			{				
				printf("%d frame/s\n", nbFrames);
				nbFrames = 0;
				lastCounter += 1.0;
				/* Render here */
				//gameEngine->draw();
		        /* Swap front and back buffers */
			}
			Control::step(currentTime - lastTime);
			lastTime = currentTime;
			gameEngine->update();
    	}

        /* Poll for and process events */
        glfwPollEvents();
	} while( !glfwWindowShouldClose(window));


	glfwTerminate();

	return 0;
}