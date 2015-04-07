#version 440

// input
in vec3 TexCoord;
in vec3 EyeSun;
in vec3 EyeSun2;
in vec3 EyePosition;
in vec3 EyeNormal;
in float BlockLight;

// textures
uniform sampler2DArray uTexture;
uniform sampler2DArray uNormalTexture;

// output
out vec4 outColor;

// ambient lighting model
float Ambient() {
  return 0.1;
}

// diffuse lighting model
float Diffuse(vec3 normal) {
  float sunDotNormal = max(dot(EyeSun, normal), 0.0);
  float sunDotNormal2 = max(dot(EyeSun2, normal), 0.0);
  
  float diffuse = 0.8 * sunDotNormal + 0.2 * sunDotNormal2;
  
  return Ambient() + diffuse * BlockLight;
}

// phong lighting model
float PhongModel(vec3 position, vec3 normal) {
  vec3 v = normalize(-position);
  vec3 h = normalize(v + EyeSun);
  
  float specular = pow(max(dot(v, h), 0.0), 40.0) * 0.4;
  
  return Diffuse(normal) + specular;
}

void main() {
  // calculate fog
  const float e = 2.71828182845904523536028747135266249;
  vec4 fogColor = vec4(0.7, 0.9, 1.0, 1.0);
  float fogDensity = 0.0025;
  float fog = pow(e, -pow(length(EyePosition) * fogDensity, 2));
  
  // calculate light
  float light = Diffuse(EyeNormal);
  
  // get texture color
  vec4 texColor = texture(uTexture, TexCoord);
  
  // calculate output
  outColor = mix(fogColor, texColor * light, fog);
}