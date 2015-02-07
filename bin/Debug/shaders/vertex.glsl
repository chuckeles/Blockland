#version 440

in vec3 inPosition;
in vec3 inNormal;
in vec2 inTexCoord;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

out vec3 Normal;
out vec2 TexCoord;

void main() {
  gl_Position = Projection * View * Model * vec4(inPosition, 1.0);
  
  Normal = inNormal;
  TexCoord = inTexCoord;
}