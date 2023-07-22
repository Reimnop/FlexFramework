#version 430
layout(location = 0) out vec4 fragColor;

uniform bool hasTexture;
uniform sampler2D tex;
uniform vec4 color;

in vec2 Uv;
in vec4 Color;

void main() {
    vec4 outColor = vec4(1.0);
    if (hasTexture) {
        outColor = texture(tex, Uv);
    }
    outColor *= Color;
    
    if (outColor.a < 0.01)
        discard;
    
    fragColor = outColor * color;
}
