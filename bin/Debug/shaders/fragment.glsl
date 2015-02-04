#version 440

in vec3 Normal;

out vec4 outColor;

void main() {
  outColor = vec4((Normal + 1) / 2, 1.0);
}