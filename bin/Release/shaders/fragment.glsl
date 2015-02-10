#version 440

// input
in vec3 TexCoord;
in vec3 EyeSun;
in vec3 EyePosition;
in vec3 EyeNormal;

// texture
uniform sampler2DArray uTexture;

// output
out vec4 outColor;

void main() {
  // calculate fog
  const float e = 2.71828182845904523536028747135266249;
  vec4 fogColor = vec4(0.7, 0.9, 1.0, 1.0);
  float fogDensity = 0.0025;
  float fog = pow(e, -pow(length(EyePosition) * fogDensity, 2));
  
  // calculate light
  vec3 v = normalize(-EyePosition);
  vec3 r = reflect(-EyeSun, EyeNormal);
  float sunDotNormal = max(dot(EyeSun, EyeNormal), 0.0);
  
  float ambient = 0.2;
  float diffuse = 0.8 * sunDotNormal;
  float specular = pow(max(dot(v, r), 0.0), 100.0) * 0.6;
  
  float light = ambient + diffuse + specular;
  
  // get texture color
  vec4 texColor = texture(uTexture, TexCoord);
  
  // calculate output
  outColor = mix(fogColor, texColor * light, fog);
}