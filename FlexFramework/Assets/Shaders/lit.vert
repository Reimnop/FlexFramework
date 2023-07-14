#version 430
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aUv;
layout(location = 3) in vec4 aColor;

uniform mat4 mvp;
uniform mat4 model;

out vec2 Uv;
out vec3 Normal;
out vec4 Color;
out vec3 WorldPos;

void main() {
    Uv = aUv;
    Normal = normalize(aNormal * mat3(transpose(inverse(model))));
    Color = aColor;
    WorldPos = vec3(vec4(aPos, 1.0) * model);
    gl_Position = vec4(aPos, 1.0) * mvp;
}
