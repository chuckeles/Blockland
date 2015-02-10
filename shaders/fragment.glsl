#version 440

// input
in vec3 TexCoord;
in float Depth;
in float Light;

// texture
uniform sampler2DArray uTexture;

// output
out vec4 outColor;

void main() {
  // calculate fog
  const float e = 2.71828182845904523536028747135266249;
  vec4 fogColor = vec4(0.7, 0.9, 1.0, 1.0);
  float fogDensity = 0.0025;
  
  // get texture color
  vec4 texColor = texture(uTexture, TexCoord);
  
  // calculate output
  outColor = mix(fogColor, texColor * Light, pow(e, -pow(Depth * fogDensity, 2)));
}