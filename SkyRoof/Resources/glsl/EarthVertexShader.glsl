#version 330 core

#define Pi 3.1415926538
#define TwoPi 2 * Pi
#define HalfPi Pi / 2

uniform float in_aspect;
uniform float in_zoom;

in vec2 in_RoPhi;
out vec2 pass_RoPhi;

void main(void) 
{
	float ro = in_RoPhi[0];
  float phi = in_RoPhi[1];

	// ro, phi -> x, y
	float x = ro * cos(phi);
	float y = ro * sin(phi);
	if (in_aspect < 1) y *= in_aspect; else x /= in_aspect;
	gl_Position = vec4(x * in_zoom, y * in_zoom, 0, 1);

	// pass to fragment shader
	pass_RoPhi = in_RoPhi;
}