#version 330 core

in vec3 fragmentColor;
out vec3 color;

uniform mat4 MVP;
uniform vec3 eyePosition;



void main()
{

	// Output color = red 
	//gl_FragColor = gl_Color;
	color = fragmentColor;

}