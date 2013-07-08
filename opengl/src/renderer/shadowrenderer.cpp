#include "renderer/shadowrenderer.h"
#include "shader/shader.h"
#include <iostream>

void ShadowRenderer::init()
{
	Shader shader;
	_depthProgramId = shader.LoadShaders( "shaders/shadow/shadow.vert", "shaders/shadow/shadow.frag" );
	_depthMatrixId = glGetUniformLocation(_depthProgramId, "depthMVP");
	_frameBufferName = 0;

	glGenBuffers(1, &_frameBufferName);
	glBindBuffer(GL_FRAMEBUFFER, _frameBufferName);
	glGenTextures(1, &_depthTexture);
	glBindTexture(GL_TEXTURE_2D, _depthTexture);

	glTexImage2D(GL_TEXTURE_2D, 0,GL_DEPTH_COMPONENT16, 1024, 1024, 0,GL_DEPTH_COMPONENT, GL_FLOAT, 0);

	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR); 
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_COMPARE_FUNC, GL_LEQUAL);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_COMPARE_MODE, GL_COMPARE_R_TO_TEXTURE);
		 
	glFramebufferTexture(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, _depthTexture, 0);

	//No color output in the bound framebuffer, only depth.
	glDrawBuffer(GL_NONE);
	// Always check that our framebuffer is ok
	if(glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
	{
		std::cerr << "Error loading depth buffer" << std::endl;
		return;
	}
}

void ShadowRenderer::render()
{
	glBindFramebuffer(GL_FRAMEBUFFER, _frameBufferName);
	glViewport(0,0,1024,1024); //Render on whole framebuffer

	// We don't use bias in the shader, but instead we draw back faces, 
	// which are already separated from the front faces by a small distance 
	// (if your geometry is made this way)
	glEnable(GL_CULL_FACE);
	glCullFace(GL_BACK); // Cull back-facing triangles -> draw only front-facing triangles

	// Clear the screen
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	// Use our shader
	glUseProgram(_depthProgramId);


}