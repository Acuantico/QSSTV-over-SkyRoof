#version 330 core
in vec2 pass_TexCoords;
out vec4 out_color;

uniform sampler2D image;

void main()
{    
    out_color = texture(image, pass_TexCoords);
}