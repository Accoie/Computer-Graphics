#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D textureFrom;
uniform sampler2D textureTo;
uniform vec2 Center;
uniform float Time;
uniform vec2 ClickPos;

const float WAVE_FREQUENCY = 20.0;
const float TIME_SCALE = 10.0;
const float DISTANCE_DECAY = 5.0;
const float TIME_DECAY = 2.0;
const float WAVE_AMPLITUDE = 0.05;
const float BRIGHTNESS_FACTOR = 0.5;

void main()
{
    vec2 uv = texCoord;

    float dist = distance(uv, ClickPos);
    float wave = sin(dist * WAVE_FREQUENCY - Time * TIME_SCALE) * exp(-dist * DISTANCE_DECAY) * exp(-Time * TIME_DECAY);

    vec2 dir = normalize(uv - ClickPos);
    vec2 distortedUV = uv + dir * wave * WAVE_AMPLITUDE;

    float progress = clamp(Time, 0.0, 1.0);
    vec4 colorFrom = texture(textureFrom, distortedUV);
    vec4 colorTo = texture(textureTo, distortedUV);

    float brightness = 1.0 - wave * BRIGHTNESS_FACTOR;

    FragColor = mix(colorFrom, colorTo, progress) * brightness;
}