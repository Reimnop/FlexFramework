#version 430
const float PI = 3.14159265359;
const int MAX_LIGHTS = 16;

layout(location = 0) out vec4 fragColor;
layout(location = 1) out vec4 fragNormal;
layout(location = 2) out vec4 fragPosition;

uniform sampler2D albedoTex;
uniform sampler2D metallicTex;
uniform sampler2D roughnessTex;
uniform vec3 cameraPos;
uniform vec3 ambientColor;

// Directional light
uniform vec3 lightDirection;
uniform vec3 lightColor;

// Point lights
uniform int pointLightsCount;
uniform vec3 pointLightPositions[MAX_LIGHTS];
uniform vec3 pointLightColors[MAX_LIGHTS];

layout(binding = 0, std140) uniform Material {
    bool useAlbedoTex; // 0
    bool useMetallicTex; // 4
    bool useRoughnessTex; // 8
    vec3 albedoValue; // 16 // ignored if useAlbedoTex is true
    vec3 emissiveValue; // 32
    float metallicValue; // 48 // ignored if useMetallicTex is true
    float roughnessValue; // 52 // ignored if useRoughnessTex is true
};

in vec2 Uv;
in vec3 Normal;
in vec4 Color;
in vec3 WorldPos;

float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a      = roughness*roughness;
    float a2     = a*a;
    float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;

    float num   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float num   = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return num / denom;
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2  = GeometrySchlickGGX(NdotV, roughness);
    float ggx1  = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

vec3 fresnelSchlick(float cosTheta, vec3 F0) {
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

vec3 getAlbedo() {
    return (useAlbedoTex ? texture(albedoTex, Uv).rgb : albedoValue.rgb) * Color.rgb;
}

float getMetallic() {
    return useMetallicTex ? texture(metallicTex, Uv).r : metallicValue;
}

float getRoughness() {
    return useRoughnessTex ? texture(roughnessTex, Uv).r : roughnessValue;
}

vec3 pbr(vec3 normalVec, vec3 cameraDir, vec3 lightDir, vec3 albedo, float metallic, float roughness) {
    vec3 N = normalVec;
    vec3 V = cameraDir;
    vec3 L = lightDir;
    vec3 H = normalize(V + L);

    // Cook-Torrance BRDF
    float NDF = DistributionGGX(N, H, roughness);
    float G = GeometrySmith(N, V, L, roughness);
    vec3 F = fresnelSchlick(max(dot(H, V), 0.0), vec3(0.04)); // F0 = 0.04 (default)
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - metallic;

    // Cook-Torrance specular reflection
    vec3 nominator = NDF * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
    vec3 specular = nominator / max(denominator, 0.001); // prevent division by zero

    // Diffuse lighting
    float NdotL = max(dot(N, L), 0.0);
    vec3 diffuse = NdotL * lightColor.rgb;

    // Specular lighting
    vec3 reflection = reflect(-L, N);
    float specularStrength = max(pow(max(dot(reflection, V), 0.0), 5.0), 0.0);
    vec3 specularLight = lightColor.rgb * specularStrength;

    // Final color
    return (kD * diffuse + specularLight) * albedo + specular;
}

void main() {
    vec3 albedo = getAlbedo();
    float metallic = getMetallic();
    float roughness = getRoughness();
    vec3 normal = normalize(Normal);
    vec3 cameraDir = normalize(cameraPos - WorldPos);
    vec3 lightDir = normalize(-lightDirection);
    
    // Directional light
    vec3 totalLight = pbr(normal, cameraDir, lightDir, albedo, metallic, roughness);
    
    // Point lights
    for (int i = 0; i < pointLightsCount; i++) {
        vec3 pointLightDir = normalize(pointLightPositions[i] - WorldPos);
        float pointLightDistance = length(pointLightPositions[i] - WorldPos);
        
        // Inverse square falloff
        float attenuation = 1.0 / (pointLightDistance * pointLightDistance);
        
        totalLight += pbr(normal, cameraDir, pointLightDir, albedo, metallic, roughness) * attenuation * pointLightColors[i];
    }
    
    vec3 ambient = ambientColor * albedo;
    
    fragColor = vec4(totalLight + emissiveValue + ambient, 1.0);
    fragNormal = vec4(normal, 1.0);
    fragPosition = vec4(WorldPos, 1.0);
}