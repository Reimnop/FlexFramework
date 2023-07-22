#version 430
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aUv;
layout(location = 2) in vec4 aColor;

uniform mat4 mvp;

out vec2 Uv;
out vec4 Color;

void main() {
    Uv = aUv;
    Color = aColor;
    gl_Position = vec4(aPos, 1.0) * mvp;
}
