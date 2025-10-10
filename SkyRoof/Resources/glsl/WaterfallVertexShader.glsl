#version 330 core

uniform float in_zoom;
uniform float in_pan;

layout(location = 0) in vec2 in_Position;
layout(location = 1) in vec2 in_PixCoord;

out vec2 pass_PixCoord;

void main(void) 
{
  float x = in_pan + in_Position.x * in_zoom;
	gl_Position = vec4(x, in_Position.y, 0, 1);
	pass_PixCoord = in_PixCoord;
}