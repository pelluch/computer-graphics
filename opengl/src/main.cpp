
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include <iostream>
#include <string>
#include <map>

#include "scene/scene.h"
#include "handlers/control.h"
#include "shader/shader.h"
#include "scene/camera.h"
#include "model/model.h"
#include "utils/xmlloader.h"
#include "utils/debugutils.h"
#include "utils/settings.h"
#include "game/gameengine.h"
#include <memory>

using namespace std;


int main(int argc, char ** argv)
{
	std::cout << "Initializing GLFW, GLEW" << std::endl;

	//std::string sceneFile, vertexShader, fragmentShader;
	//menu(sceneFile, vertexShader, fragmentShader);

	int width = 800;
	int height = 800;
	bool showFPS = true;
	RenderingParams::setWindowSize(width, height);

	RenderingParams::mode = PER_PIXEL;


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
	
	std::shared_ptr<GameEngine>  gameEngine(new GameEngine());
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
				if(showFPS)
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