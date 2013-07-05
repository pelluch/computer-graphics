#version 330 core

// Input vertex data, different for all executions of this shader.
layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexNormal;
layout(location = 2) in vec3 vertexColor;

uniform mat4 viewProjectionMatrix;
uniform mat4 modelMatrix;

uniform int numLights;
uniform vec3 lights[10];
uniform vec3 eyePosition;

out vec3 fragmentNormal;
out vec3 fragmentColor;
out vec3 fragmentPosition;

void main(){

	gl_Position = viewProjectionMatrix * modelMatrix * vec4(vertexPosition, 1);
	fragmentPosition = vertexPosition;
	fragmentNormal = vertexNormal;
	fragmentColor = vertexColor;
}

