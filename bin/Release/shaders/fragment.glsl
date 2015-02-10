#version 440

in vec3 Normal;
in vec3 TexCoord;
in float Depth;

uniform sampler2DArray Texture;

out vec4 outColor;

void main() {
  const float e = 2.71828182845904523536028747135266249;

  vec4 fogColor = vec4(0.7, 0.9, 1.0, 1.0);
  float fogDensity = 0.0025;

  float light = 0.5 + Normal.x * 0.1 + Normal.y * 0.4 + Normal.z * 0.05;
  
  vec4 texColor = texture(Texture, TexCoord);
  outColor = mix(fogColor, texColor * light, pow(e, -pow(Depth * fogDensity, 2)));
}