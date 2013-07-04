#version 330 core

in vec3 fragmentColor;
out vec3 color;

void main()
{

	// Output color = red 
	//gl_FragColor = gl_Color;
	color = fragmentColor;

}