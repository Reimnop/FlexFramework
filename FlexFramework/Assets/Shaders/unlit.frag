#version 150

#extension GL_ARB_explicit_attrib_location : enable
#extension GL_ARB_explicit_uniform_location : enable

layout(location = 0) out vec4 fragColor;

layout(location = 1) uniform bool hasTexture;
layout(location = 2) uniform sampler2D _texture;
layout(location = 3) uniform vec4 color;

in vec2 Uv;

void main() {
    vec4 outColor = vec4(1.0);
    if (hasTexture) {
        outColor = texture(_texture, Uv);
    }
    
    fragColor = outColor * color;
}
