#version 440

in vec3 inPosition;
in vec3 inNormal;

uniform mat4 View;
uniform mat4 Projection;

out vec3 Normal;

void main() {
  gl_Position = Projection * View * vec4(inPosition, 1.0);

  Normal = inNormal;
}