#version 330 core

// Input vertex data, different for all executions of this shader.
layout(location = 6) in vec3 vertexPosition;
layout(location = 7) in vec3 vertexColor;

uniform mat4 MVP;
out vec3 fragmentColor;

void main()
{
	gl_Position = MVP*vec4(vertexPosition,1);
	fragmentColor = vertexColor;

}