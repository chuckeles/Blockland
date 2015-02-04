#version 440

in vec3 Normal;

out vec4 outColor;

void main() {
  vec3 color = vec3(0.2, 0.6, 1.0);

  float light = 0.75 + Normal.x / 8 + Normal.y / 4 + Normal.z / 10;

  outColor = vec4(color * light, 1.0);
}