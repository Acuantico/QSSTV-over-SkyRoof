#version 330 core

layout(location = 0) in vec4 in_Vertex;
uniform mat4 transform;
out vec2 pass_TexCoords;

void main()
{
    pass_TexCoords = in_Vertex.zw;
    gl_Position = transform * vec4(in_Vertex.xy, 0.0, 1.0);
}