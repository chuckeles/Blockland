#version 440

in vec3 Normal;
in float Type;

out vec4 outColor;

void main() {
  vec3 grass = vec3(0.2, 0.7, 0.2);
  vec3 dirt = vec3(0.4, 0.2, 0.0);
  vec3 stone = vec3(0.4, 0.4, 0.4);

  vec3 color = stone;
  if (abs(Type - 1.0) < 0.01)
    color = grass;
  if (abs(Type - 2.0) < 0.01)
    color = dirt;

  float light = 0.5 + Normal.x * 0.1 + Normal.y * 0.4 + Normal.z * 0.05;

  outColor = vec4(color * light, 1.0);
}