#version 430 core
layout(location = 0) out vec4 fragColor;

layout(location = 0) in vec2 UV;
layout(location = 1) in vec4 Color;
layout(location = 2) flat in int Index;

layout(location = 1) uniform sampler2D atlas[16];

void main() {
    vec4 outCol = vec4(1.0);
    if (Index != 0) {
        vec4 texCol = texture(atlas[abs(Index) - 1], UV);
        outCol = Index > 0 ? vec4(1.0, 1.0, 1.0, texCol.r) : texCol;
    }

    fragColor = outCol * Color;
}