#version 400 core

in vec3 fNormal;
in vec3 fPos;
in vec2 fTexCoords;

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    vec3 viewPosition;
};

uniform Material material;
uniform Light light;
uniform bool useBlinnAlgorithm;

out vec4 FragColor;

vec3 ambientCalculation(vec3 ambient, sampler2D diffuse, vec2 fTexCoords);
vec3 diffuseCalculation(vec3 lightDiffuse,sampler2D materialDiffuse,float diff, vec2 fTexCoords);
vec3 specularCalculation(sampler2D materialSpecular, vec3 LightSpecular, float shininess, vec3 viewDirection, vec3 reflectionDirection, vec2 fTexCoords);

void main()
{
    vec3 norm = normalize(fNormal);
    vec3 lightDirection = normalize(light.position - fPos);
    float diff = max(dot(norm, lightDirection), 0.0);

    vec3 viewDirection = normalize(light.viewPosition - fPos);
    vec3 reflectDirection = reflect(-lightDirection, norm);
    
    vec3 ambient = ambientCalculation(light.ambient, material.diffuse, fTexCoords);
    vec3 diffuse = diffuseCalculation(light.diffuse, material.diffuse, diff, fTexCoords).rgb;
    vec3 specular = specularCalculation( material.specular, light.specular,  material.shininess,  viewDirection,  reflectDirection, fTexCoords);

    //The resulting colour should be the amount of ambient colour + the amount of additional colour provided by the diffuse of the lamp + the specular amount
    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1.0);
}

vec3 ambientCalculation(vec3 ambient, sampler2D diffuse, vec2 fTexCoords){
    return ambient * texture(diffuse, fTexCoords).rgb;
}

vec3 diffuseCalculation(vec3 lightDiffuse, sampler2D materialDiffuse,float diff,  vec2 fTexCoords){
    return lightDiffuse * (diff * texture(materialDiffuse, fTexCoords).rgb);
}

vec3 specularCalculation(sampler2D materialSpecular, vec3 LightSpecular, float shininess, vec3 viewDirection, vec3 reflectDirection, vec2 fTexCoords)
{
    float spec = pow(max(dot(viewDirection, reflectDirection), 0.0), shininess);
    return LightSpecular * (spec * texture(materialSpecular, fTexCoords).rgb);
}