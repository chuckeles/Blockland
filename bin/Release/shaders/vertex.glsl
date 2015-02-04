#version 440

in vec3 inPosition;

uniform mat4 view;
uniform mat4 projection;

void main() {
  gl_Position = projection * view * vec4(inPosition, 1.0);
}