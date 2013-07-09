#include "renderer/params.h"
#include <iostream>

std::string RenderingParams::fragmentShaderPath = "";
std::string RenderingParams::vertexShaderPath = "";
SHADER_MODE RenderingParams::mode = PER_PIXEL;
bool RenderingParams::antiAlias = true;
int RenderingParams::verticalInterval = 1;
bool RenderingParams::paused = false;
int RenderingParams::_width;
int RenderingParams::_height;

float RenderingParams::getAspectRatio()
{
	//std::cout << "Getting AR " << width << " " << height << std::endl;
	float aspectRatio = ((float)_width)/((float)_height);
	return aspectRatio;
}

void RenderingParams::setWindowSize(int width, int height)
{
	_width = width;
	_height = height;
}

int RenderingParams::getHeight()
{
	return _height;
}

int RenderingParams::getWidth()
{
	return _width;
}