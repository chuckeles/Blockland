#version 440

in vec2 inPosition;

uniform mat4 projection;

void main() {
  gl_Position = projection * vec4(inPosition, 0.0, 1.0);
}