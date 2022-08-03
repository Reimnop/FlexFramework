#version 430 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aUv;

layout(location = 0) uniform mat4 mvp;

layout(location = 0) out vec2 Uv;

void main() {
    Uv = aUv;
    gl_Position = vec4(aPos, 1.0) * mvp;
}
