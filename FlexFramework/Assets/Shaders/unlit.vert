#version 150

#extension GL_ARB_explicit_attrib_location : enable
#extension GL_ARB_explicit_uniform_location : enable

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aUv;

layout(location = 0) uniform mat4 mvp;

out vec2 Uv;

void main() {
    Uv = aUv;
    gl_Position = vec4(aPos, 1.0) * mvp;
}
