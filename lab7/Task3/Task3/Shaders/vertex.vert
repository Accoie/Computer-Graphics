#version 330 core

layout(location = 0) in vec3 position;

uniform float progress;
uniform mat4 model, view, projection;

out vec3 fragNormal;
out vec3 fragPos;

vec3 calcNormalParaboloid(vec2 p) { return normalize(vec3(-2.0 * p.x, -2.0 * p.y, 1.0)); }
vec3 calcNormalSaddle(vec2 p)     { return normalize(vec3(-2.0 * p.x,  2.0 * p.y, 1.0)); }

void main()
{
    vec3 posParaboloid = vec3(position.x, position.y, position.x * position.x + position.y * position.y);
    vec3 posSaddle = vec3(position.x, position.y, position.x * position.x - position.y * position.y);
    vec3 morphedPos = mix(posParaboloid, posSaddle, progress);
    
    vec3 n1 = calcNormalParaboloid(position.xy);
    vec3 n2 = calcNormalSaddle(position.xy);
    fragNormal = mix(n1, n2, progress);
    
    fragPos = (model * vec4(morphedPos, 1.0)).xyz;
    gl_Position = projection * view * model * vec4(morphedPos, 1.0);
}
