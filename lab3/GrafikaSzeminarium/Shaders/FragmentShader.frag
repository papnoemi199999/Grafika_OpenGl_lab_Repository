#version 330 core
out vec4 FragColor;

uniform vec3 uLightColor;
uniform vec3 uLightPos;
uniform vec3 uViewPos;

uniform float uShininess;
uniform float uAmbientStrength;
uniform float uSpecularStrength; 
uniform float uDiffuseStrength; 

in vec4 outCol;
in vec3 outNormal;
in vec3 outWorldPosition;

uniform vec3 uTopColor;


void main()
{
    vec3 ambient = uAmbientStrength * uLightColor;
    vec3 finalColor = outCol.rgb;
    if (outNormal.y > 0.9) {
        finalColor = uTopColor;
    }


    float diffuseStrength = 0.3;
    vec3 norm = normalize(outNormal);
    vec3 lightDir = normalize(uLightPos - outWorldPosition);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * uLightColor * uDiffuseStrength; 

    vec3 viewDir = normalize(uViewPos - outWorldPosition);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), uShininess);
    vec3 specular = uSpecularStrength * spec * uLightColor;


    vec3 result = (ambient + diffuse + specular) * finalColor;

    FragColor = vec4(result, outCol.w);
}
