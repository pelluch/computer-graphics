#version 330 core

// Input vertex data, different for all executions of this shader.
layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexNormal;

uniform mat4 viewProjectionMatrix;
uniform mat4 modelMatrix;
uniform mat4 invModelMatrix;
uniform int numLights;
uniform vec3 lights[10];
uniform vec3 eyePosition;
uniform vec3 materialDiffuse;

out vec3 fragmentNormal;
out vec3 fragmentWorldPosition;

void main(){

	gl_Position = viewProjectionMatrix * modelMatrix * vec4(vertexPosition, 1);
	fragmentWorldPosition = (modelMatrix * vec4(vertexPosition, 1)).xyz;
	vec3 transformedNormal = (invModelMatrix * vec4(vertexNormal, 0)).xyz;
	fragmentNormal = normalize(transformedNormal);
}

