#version 330 core


// Input vertex data, different for all executions of this shader.
layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec2 textureUV;
layout(location = 2) in vec3 vertexNormal;
layout(location = 3) in vec3 vertexTangent;
layout(location = 4) in vec3 vertexBitangent;

uniform mat4 MVP;
uniform mat4 M;
uniform mat4 V;
uniform mat3 MV3x3;

uniform int numLights;
uniform vec3 lights[10];
uniform vec3 eyePosition;
uniform vec3 materialDiffuse;

//out vec3 fragmentNormal;
out vec3 fragmentWorldPosition;
out vec2 fragmentUV;
out vec3 lightDirectionTangents[10];
out vec3 eyeDirectionTangent;
out vec3 eyeDirectionCamera;
out vec3 lightDirectionCameras[10];
out vec3 fragmentNormal;

void main(){

	gl_Position = MVP * vec4(vertexPosition, 1);
	fragmentWorldPosition = (M * vec4(vertexPosition,1)).xyz;

	vec3 vertexPositionCamera = (V * M * vec4(vertexPosition,1)).xyz;
	eyeDirectionCamera = vec3(0,0,0) - vertexPositionCamera;
	fragmentUV = textureUV;

	vec3 vertexTangentCamera = MV3x3 * vertexTangent;
	vec3 vertexBitangentCamera = MV3x3 * vertexBitangent;
	vec3 vertexNormalCamera = MV3x3 * vertexNormal;

	mat3 TBN = transpose(mat3(vertexTangentCamera,
		vertexBitangentCamera,
		vertexNormalCamera));

	
	eyeDirectionTangent = TBN * eyeDirectionCamera;


	vec3 lightPositionCamera = (V * vec4(lights[0],1)).xyz;
	vec3 lightDirectionCamera = lightPositionCamera + eyeDirectionCamera;
	vec3 lightDirectionTangent = TBN * lightDirectionCamera;
	lightDirectionCameras[0] = lightDirectionCamera;
	lightDirectionTangents[0] = lightDirectionTangent;
	fragmentNormal = normalize((transpose(inverse(M)) * vec4(vertexNormal,0)).xyz);
	//fragmentWorldPosition = (modelMatrix * vec4(vertexPosition, 1)).xyz;
	//vec3 transformedNormal = (invModelMatrix * vec4(vertexNormal, 0)).xyz;
	//fragmentNormal = normalize(transformedNormal);
	//fragmentUV = textureUV;
}

