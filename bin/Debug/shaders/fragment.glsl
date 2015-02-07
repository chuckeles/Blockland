#version 440

in vec3 Normal;
in vec2 TexCoord;

uniform sampler2D Texture;

out vec4 outColor;

void main() {
  float light = 0.5 + Normal.x * 0.1 + Normal.y * 0.4 + Normal.z * 0.05;

  outColor = texture(Texture, TexCoord) * vec4(light, light, light, 1.0);
}