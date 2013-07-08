#ifndef SHADOW_RENDERER_H_
#define SHADOW_RENDERER_H_

#include "GL/glew.h"

class ShadowRenderer
{
private:
	GLuint _depthTexture;
	GLuint _frameBufferName;
	GLuint _depthProgramId;
	GLuint _depthMatrixId;
public:
	void init();
	void render();
};

#endif