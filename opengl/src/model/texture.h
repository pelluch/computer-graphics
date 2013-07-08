#ifndef TEXTURE_H_
#define TEXTURE_H_

#include <GL/glew.h>
#include <string>

class Texture
{

private:
	GLuint _textureUniformId;
	GLuint _textureId;
	std::string _texturePath;
public:
	Texture(const std::string & name);
	~Texture();
	void initTexture(GLuint shaderProgramId);
	void bindUniform();
};


#endif