#version 430
layout(location = 0) out vec4 fragColor;

layout(location = 1) uniform bool hasTexture;
layout(location = 2) uniform sampler2D _texture;
layout(location = 3) uniform vec4 color;

in vec2 Uv;
in vec4 Color;

void main() {
    vec4 outColor = vec4(1.0);
    if (hasTexture) {
        outColor = texture(_texture, Uv);
    }
    outColor *= Color;
    
    if (outColor.a < 0.01)
        discard;
    
    fragColor = outColor * color;
}
