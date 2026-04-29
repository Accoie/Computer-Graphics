#version 330 core

in vec3 fragNormal;
in vec3 fragPos;

uniform vec3 lightDir;

out vec4 color;

void main()
{
    vec3 N = normalize(fragNormal);
    vec3 L = normalize(lightDir);
    
    float diff = max(dot(N, L), 0.0);
    
    vec3 baseColor = vec3(0.8, 0.9, 1.0);
    vec3 litColor = baseColor * (0.2 + 0.8 * diff);
    
    color = vec4(litColor, 1.0);
}
