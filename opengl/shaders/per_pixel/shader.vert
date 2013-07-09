#version 330 core

// Input vertex data, different for all executions of this shader.
layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec2 textureUV;
layout(location = 2) in vec3 vertexNormal;
layout(location = 3) in vec3 vertexTangent;
layout(location = 4) in vec3 vertexBitangent;

uniform mat4 M;
uniform mat4 V;
uniform mat4 MV3x3;
uniform mat4 MVP;
uniform int numLights;
uniform vec3 lights[10];
uniform vec3 eyePosition;
uniform vec3 materialDiffuse;

out vec3 fragmentNormal;
out vec3 fragmentWorldPosition;
out vec2 fragmentUV;

void main(){

	gl_Position = MVP * vec4(vertexPosition, 1);
	fragmentWorldPosition = (M * vec4(vertexPosition, 1)).xyz;
	vec3 transformedNormal = normalize((M * vec4(vertexNormal, 0)).xyz);
	fragmentNormal = transformedNormal;
	fragmentUV = textureUV;
}

