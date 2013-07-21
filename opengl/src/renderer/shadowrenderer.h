#ifndef SHADOW_RENDERER_H_
#define SHADOW_RENDERER_H_

#include "GL/glew.h"
#include "shader/shader.h"

class ShadowRenderer
{
private:
	GLuint _depthTexture;
	GLuint _frameBufferName;
	GLuint _depthMatrixId;
	Shader * _shader;
public:
	ShadowRenderer();
	~ShadowRenderer();
	void init();
	void render();
};

#endif