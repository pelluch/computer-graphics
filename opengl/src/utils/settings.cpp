#include "utils/settings.h"
#include "renderer/params.h"
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>

using std::cout;
using std::endl;
using std::cin;
using std::cerr;

void Settings::setAntiAlias()
{
	int choice;
	cout << "Choose antialias mode (0 = none, 1 = AAx4" << endl;
	cin >> choice;
	RenderingParams::antiAlias = choice;
	if(choice == 0)
		glDisable(GL_MULTISAMPLE);	
	else
		glEnable(GL_MULTISAMPLE);
}

void Settings::addLight()
{

}

void Settings::removeLight()
{

}

void Settings::setInterval()
{
	int choice;
	cout << "Set vertical interval" << endl;
	cin >> choice;
	RenderingParams::verticalInterval = choice;
	glfwSwapInterval(choice);

}

void Settings::addModel()
{

}

void Settings::setCameraPosition()
{

}

void Settings::settingsMenu()
{
	bool isPaused = RenderingParams::paused;
	RenderingParams::paused = true;
	cout << "1. Set antialias" << endl;
	cout << "2. Set interval" << endl;
	cout << "3. Add model" << endl;
	cout << "4. Add light" << endl;
	cout << "5. Set camera position" << endl;
	cout << "6. Remove light" << endl;

	int choice;
	cin >> choice;

	switch(choice)
	{
		case 1:
			setAntiAlias();
			break;
		case 2:
			setInterval();
			break;
		case 3:
			addModel();
			break;
		case 4:
			addLight();
			break;
		case 5:
			setCameraPosition();
			break;
		case 6:
			removeLight();
			break;
		default:
			cout << "Invalid option" << endl;
			break;
	}
	RenderingParams::paused = isPaused;
}