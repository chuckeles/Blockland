#version 440

// input
in vec3 inPosition;
in vec3 inNormal;
in vec3 inTexCoord;

// matrices
uniform mat4 uModelView;
uniform mat4 uModelViewProjection;
uniform mat3 uNormal;

// output
out vec3 TexCoord;
out float Depth;
out float Light;

void main() {
  // sun direction
  vec3 sun = normalize(vec3(0.1, 1.0, 0.2));

  // calculate output
  gl_Position = uModelViewProjection * vec4(inPosition, 1.0);
  TexCoord = inTexCoord;
  Depth = length((uModelView * vec4(inPosition, 1.0)).xyz);
  
  // calculate light
  vec3 eyeNormal = normalize(uNormal * inNormal);
  vec3 eyeSun = normalize(uNormal * sun);
  Light = 0.2 + 0.8 * max(dot(eyeSun, eyeNormal), 0.0);
}