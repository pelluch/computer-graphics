#ifndef SHADER_H
#define SHADER_H

#include <GL/glew.h>
#include <GLFW/glfw3.h>

enum SHADER_TYPE { SHADER_NO_TEX, SHADER_TEX, SHADER_TEX_NORMAL };
class Shader
{
	public:
		Shader(const char * vertex_file_path, const char * fragment_file_path);
		~Shader();
		GLuint getId();
	private:
		GLuint _shaderProgramId;
		GLuint LoadShaders(const char * vertex_file_path, const char * fragment_file_path);
};



#endif