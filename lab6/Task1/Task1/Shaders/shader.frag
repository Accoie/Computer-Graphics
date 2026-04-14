#version 330 core

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;

out vec4 FragColor;

uniform bool useTexture;
uniform sampler2D mainTexture;
uniform vec3 ambientColor;
uniform vec3 diffuseColor;

void main()
{
    if (useTexture)
    {
        FragColor = texture(mainTexture, TexCoord) * vec4(diffuseColor, 1.0);
    }
    else
    {
        FragColor = vec4(diffuseColor, 1.0);
    }
}