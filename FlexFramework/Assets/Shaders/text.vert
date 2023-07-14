#version 430
layout(location = 0) in vec2 aPos;
layout(location = 1) in vec2 aUv;

out vec2 UV;

uniform mat4 mvp;

void main() {
    UV = aUv;
    gl_Position = vec4(aPos, 0.0, 1.0) * mvp;
}