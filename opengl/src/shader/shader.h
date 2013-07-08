#ifndef SHADER_H
#define SHADER_H

#include "renderer/params.h"
#include <GL/glew.h>
#include <GLFW/glfw3.h>

class Shader
{
	public:
		GLuint LoadShaders(const char * vertex_file_path, const char * fragment_file_path);
		GLuint LoadShaders();
};



#endif