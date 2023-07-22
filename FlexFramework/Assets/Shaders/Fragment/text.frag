#version 430
layout(location = 0) out vec4 fragColor;

in vec2 UV;

uniform sampler2D atlas;
uniform vec4 overlayColor;
uniform float distanceRange = 4.0;

void main() {
    vec3 msdf = texture(atlas, UV).rgb;
    float dist = max(min(msdf.r, msdf.g), min(max(msdf.r, msdf.g), msdf.b));
    float alpha = clamp((dist - 0.5) * distanceRange + 0.5, 0.0, 1.0);
    vec4 outCol = vec4(vec3(1.0), alpha);

    fragColor = outCol * overlayColor;
}