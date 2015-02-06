#version 440

in vec3 Normal;

out vec4 outColor;

void main() {
  vec3 color = vec3(0.5, 0.5, 0.5);

  float light = 0.5 + Normal.x * 0.1 + Normal.y * 0.4 + Normal.z * 0.05;

  outColor = vec4(color * light, 1.0);
}