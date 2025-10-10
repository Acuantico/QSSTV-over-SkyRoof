#version 330 core

in vec2 pass_PixCoord;

uniform sampler2D indexedTexture;
uniform sampler2D paletteTexture;

uniform float in_zoom;
uniform int   in_ScreenWidth;
uniform float in_ScrollPos;
uniform float in_ScrollHeight;
uniform float in_Brightness;
uniform float in_Contrast;
uniform int   in_TextureFold;
uniform int   in_TextureWidth;
uniform int   in_SpectraHeight;

out vec4 out_Color;

float sample_texture(int x, int y)
{
  // a scpectrum is stored in 4 rows of texture.
  // convert index-into-spectra to index-into-texture
  int texelX = x % in_TextureWidth;
	int texelY = y * in_TextureFold + x / in_TextureWidth;

  return texelFetch(indexedTexture, ivec2(texelX, texelY), 0).r;
}

void main(void) 
{
  int spectraWidth = in_TextureWidth * in_TextureFold;

  // y coordinate in the spectra after scrolling
  float spectraYfloat = fract(2 - pass_PixCoord.t * in_ScrollHeight + in_ScrollPos);
  int spectraY = int(in_SpectraHeight * spectraYfloat);	
	
	// texels per pixel: stretch or shrink factor
	float scale = spectraWidth / float(in_ScreenWidth * in_zoom);	
	float luminance = 0;

	if (scale > 1)
	{
    // shrink: take the max of all texels in the range
	  int pixelX = int(pass_PixCoord.s * in_ScreenWidth * in_zoom);
  	int spectraXstart = int(pixelX * scale);
	  int spectraXstop = int((pixelX + 1) * scale);
	  for (int spectraX = spectraXstart; spectraX < spectraXstop; spectraX++)
	    luminance = max(luminance, sample_texture(spectraX, spectraY));	
	}
	else
	{
		// stretch: interpolate between 2 texels
		float spectraX = pass_PixCoord.s * spectraWidth;
		int spectraXint = int(spectraX);
		float a = spectraX - spectraXint;
		float v1 = sample_texture(spectraXint, spectraY);
	  float v2 = sample_texture(spectraXint+1, spectraY);
		luminance = v1 * (1-a) + v2 * a;
	}

	// apply brightness and contrast	
	luminance = in_Brightness + luminance * in_Contrast;

	// get color from palette
	out_Color = texture(paletteTexture, vec2(luminance, 0));
}