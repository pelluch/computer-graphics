#include "utils/debug_utils.h"
#include <iostream>
#include <glm/gtc/matrix_access.hpp>

using namespace std;

void Debugger::printInfo(Camera & cam)
{
	cout << "Printing camera information" << endl; 
	printInfo(cam._eye);
	printInfo(cam._target);
	printInfo(cam._up);
	cout << "near: " << cam._near << endl;
	cout << "far: " << cam._far << endl;
}

void Debugger::printInfo(Scene & scene)
{ 
	cout << "Nah :)" << endl;
}

void Debugger::printInfo(Material & material)
{ 
	cout << "Printing material info" << endl;
	printInfo(material._diffuseColor);
	printInfo(material._reflectiveColor);
	printInfo(material._refractiveColor);
	printInfo(material._specularColor);
	cout << material._texturePath << endl;
	cout << material._normalTexturePath << endl;
}

void Debugger::printInfo(Light & light)
{
	cout << "Printing light information" << endl;
	printInfo(light._color);
	printInfo(light._worldPosition);	
	cout << light._constantAttenuation << endl;
	cout << light._linearAttenuation << endl;
	cout << light._quadraticAttenuation << endl;
}
void Debugger::printInfo(Model & model)
{ 
	cout << "Model has " << model._vertex.size() << " vertices" << endl;
	cout << "Model has " << model._normals.size() << " normals" << endl;
	cout << "Model has " << model._textureCoords.size() << " texture coords" << endl;
	printInfo(model._worldPosition);
	printInfo(model._worldRotation);
	printInfo(model._scale);
}
void Debugger::printInfo(glm::vec3 vec)
{ 
	cout << " [ ";
	for(int i = 0; i < 3; ++i)
	{
		cout << vec[i] << " ";
	}
	cout << " ] " << endl;
}
void Debugger::printInfo(glm::mat4 mat)
{ 
	cout << " [ ";
	for(int i = 0; i < 4; ++ i)
	{
		glm::vec4 col = glm::row(mat, i);
		printInfo(col);
	}
	cout << " ] " << endl;
}
void Debugger::printInfo(glm::vec4 vec)
{ 
	cout << " [ ";
	for(int i = 0; i < 4; ++i)
	{
		cout << vec[i] << " ";
	}
	cout << " ] " << endl;
}
void Debugger::printInfo(glm::vec2 vec)
{ 
	cout << " [ ";
	for(int i = 0; i < 2; ++i)
	{
		cout << vec[i] << " ";
	}
	cout << " ] " << endl;
}