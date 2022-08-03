#version 430 core
layout(location = 0) out vec4 fragColor;

layout(location = 1) uniform bool hasTexture;
layout(location = 2) uniform sampler2D _texture;

layout(location = 0) in vec2 Uv;

void main() {
    vec4 outColor = vec4(1.0);
    if (hasTexture) {
        outColor = texture(_texture, Uv);
    }
    
    fragColor = outColor;
}
