#version 330 core
layout(location = 0) in float x;

uniform mat4 projection;
uniform float yOffset;

float getCanabolaCoef(float x)
{
    return (1.0 + sin(x)) * (1.0 + 0.9 * cos(8.0 * x)) * (1.0 + 0.1 * cos(24.0 * x)) * (0.5 + 0.3 * cos(140.0 * x));
}

void main()
{
    vec4 position = vec4(x, 0.0, 0.0, 1.0);
    float canabolaCoef = getCanabolaCoef(position.x);

    position.y = canabolaCoef * sin(position.x);
    position.x = canabolaCoef * cos(position.x);
    
    position.y += yOffset;

    gl_Position = projection * position;
}
