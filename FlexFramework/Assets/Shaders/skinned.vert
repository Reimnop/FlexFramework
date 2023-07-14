#version 430
const int MAX_BONES = 100;

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aUv;
layout(location = 3) in vec4 aColor;
layout(location = 4) in ivec4 aBoneIds;
layout(location = 5) in vec4 aWeights;

uniform mat4 mvp;
uniform mat4 model;
uniform mat4 bones[MAX_BONES];

out vec2 Uv;
out vec3 Normal;
out vec4 Color;
out vec3 WorldPos;

void main() {
    mat4 boneTransform = mat4(1.0);
    float weightSum = aWeights[0] + aWeights[1] + aWeights[2] + aWeights[3];
    if (weightSum != 0) {
        boneTransform  = bones[aBoneIds[0]] * (aWeights[0] / weightSum);
        boneTransform += bones[aBoneIds[1]] * (aWeights[1] / weightSum);
        boneTransform += bones[aBoneIds[2]] * (aWeights[2] / weightSum);
        boneTransform += bones[aBoneIds[3]] * (aWeights[3] / weightSum);
    }
    
    mat4 finalModel = boneTransform * model;
    
    Uv = aUv;
    Color = aColor;
    Normal = normalize(aNormal * mat3(transpose(inverse(finalModel))));
    WorldPos = vec3(vec4(aPos, 1.0) * finalModel);
    gl_Position = vec4(aPos, 1.0) * boneTransform * mvp;
}
