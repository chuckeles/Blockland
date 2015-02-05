#version 440

in vec3 inPosition;
in vec3 inNormal;
in float inType;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

out vec3 Normal;
out float Type;

void main() {
  gl_Position = Projection * View * Model * vec4(inPosition, 1.0);

  Normal = inNormal;
  Type = inType;
}