
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

using namespace std;

Scene * scene;

static GLuint shaderProgramId;
static GLuint matrixId;
static Shader shader;


static void initBuffers()
{
	/*Model model;
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
	*/
	scene->initModelData();
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
	glEnable(GL_MULTISAMPLE);
	// Accept fragment if it closer to the camera than the former one

}

static void loadShaders()
{
	shaderProgramId = shader.LoadShaders();
	matrixId = glGetUniformLocation(shaderProgramId, "viewProjectionMatrix");
}


static void loadScene()
{
	Camera cam(45, 0.035, 1500, glm::vec3(0,0,-4), glm::vec3(0,0,0), glm::vec3(0,1,0));
	scene = new Scene(cam);
	Light l1(glm::vec3(1, -1, -0.2), glm::vec3(1,1,1));
	Light l2(glm::vec3(0, 0, -0.2), glm::vec3(1,1,1));
	scene->addLight(l1);
	//scene->addLight(l2);
	scene->setShaderId(shaderProgramId);
	scene->generateIds();
}

static void init()
{

	//loadScene();
	std::cout << "Loading scene->.." << std::endl;
	scene = XmlLoader::loadScene("scenes/cornellBoxTarea2c.xml");
	Control::setScene(scene);
	initBuffers();
	setRenderingParameters();
	loadShaders();
	
	std::cout << "Scene loaded, setting shader id" << std::endl;
	scene->setShaderId(shaderProgramId);
	std::cout << "Generating scene ids" << std::endl;
	scene->generateIds();
	scene->setMaterials();

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
	glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );

	//Use the shader
	glUseProgram(shaderProgramId);

	
	float aspectRatio = RenderingParams::getAspectRatio();
	glm::mat4 perspectiveTransform = scene->projectionTransform(aspectRatio);
	glm::mat4 viewTransform = scene->viewTransform();
	glm::mat4 viewProjectionMatrix = perspectiveTransform*viewTransform;

	glUniformMatrix4fv(matrixId, 1, GL_FALSE, &viewProjectionMatrix[0][0]);
	scene->bindUniforms();
	scene->draw(shaderProgramId);
	
}



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
	std::cout << "Initializing" << std::endl;
	init();

	// For speed computation
	double lastTime = glfwGetTime();
	int nbFrames = 0;
	glfwSwapInterval(1);
	do
	{		
		if(!RenderingParams::paused)
		{
				// Measure speed
			double currentTime = glfwGetTime();
			nbFrames++;
			if ( currentTime - lastTime >= 1.0 ){ // If last prinf() was more than 1sec ago
				// printf and reset
				//printf("%f ms/frame\n", 1000.0/double(nbFrames));
				printf("%d frame/s\n", nbFrames);
				nbFrames = 0;
				lastTime += 1.0;
			}

			/* Render here */
			draw();

	        /* Swap front and back buffers */
	        glfwSwapBuffers(window);
    	}

        /* Poll for and process events */
        glfwPollEvents();
	} while( !glfwWindowShouldClose(window));


	glfwTerminate();

	delete scene;
	return 0;
}