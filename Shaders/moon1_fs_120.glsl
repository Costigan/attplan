#version 120

uniform sampler2D tex;
varying vec3 normal, eyeVec, lightDir;
void main (void)
{
  vec4 MaterialDiffuseColor = texture2D(tex, gl_TexCoord[0].st);
  vec4 MaterialAmbientColor = MaterialDiffuseColor;
  vec4 MaterialSpecularColor = gl_FrontMaterial.specular;

  vec3 N = normalize(normal);
  vec3 L = normalize(lightDir);
  float lambertTerm = dot(N,L);

  if (lambertTerm >= 0)
    gl_FragColor = MaterialDiffuseColor;
  else
    gl_FragColor = vec4(0, 0, 0, 1);


}
