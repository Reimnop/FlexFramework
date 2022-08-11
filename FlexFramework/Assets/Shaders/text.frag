#version 150

#extension GL_ARB_explicit_attrib_location : enable
#extension GL_ARB_explicit_uniform_location : enable

layout(location = 0) out vec4 fragColor;

in vec2 UV;
in vec4 Color;
flat in int Index;

layout(location = 1) uniform sampler2D atlas[16];

void main() {
    vec4 outCol = vec4(1.0);
    if (Index != 0) {
        vec4 texCol = texture(atlas[abs(Index) - 1], UV);
        outCol = Index > 0 ? vec4(1.0, 1.0, 1.0, texCol.r) : texCol;
    }

    fragColor = outCol * Color;
}