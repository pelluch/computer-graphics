#ifndef TEXTURE_H_
#define TEXTURE_H_

#include <GL/glew.h>
#include <string>

enum TEXTURE_TYPE { TEX_DIFFUSE, TEX_SPECULAR, TEX_NORMAL };

class Texture
{


private:
	GLuint _textureUniformId;
	GLuint _textureId;
	std::string _texturePath;
	TEXTURE_TYPE _type;
public:
	Texture(const std::string & name, TEXTURE_TYPE type = TEX_DIFFUSE);
	~Texture();
	void initTexture(GLuint shaderProgramId);
	void bindUniform();
};


#endif