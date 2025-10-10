using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Numerics;
using System.Runtime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;
using VE3NEA;

namespace SkyRoof
{
  public class Sprite
  {
    private OpenGL gl;
    private uint[] TextureIds = new uint[1];
    private Size ImageSize;
    private float scale = 1;

    public bool Enabled = true;
    public PointF Location;
    public float Angle;
    public float Scale { get => scale; set => SetScale(value); }
    public uint TextureId => TextureIds[0];
    public Matrix4x4 Transform => GetTransform();


    public Sprite(OpenGL gl, byte[] imageBytes)
    {
      this.gl = gl;
      LoadTexture(imageBytes);
    }

    private void LoadTexture(byte[] imageBytes)
    {
      gl.ActiveTexture(OpenGL.GL_TEXTURE0);

      gl.GenTextures(1, TextureIds);
      gl.BindTexture(OpenGL.GL_TEXTURE_2D, TextureId);

      gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);    // shrink
      gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);    // stretch
      gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE); // tex x
      gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE); // tex y

      using (Bitmap bitmap = Utils.BitmapFromBytes(imageBytes))
      {
        ImageSize = bitmap.Size;

        BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, bitmap.Width, bitmap.Height, 0,
          OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);

        bitmap.UnlockBits(data);
      }      
    }  
    

    private void SetScale(float value) 
    { 
      scale = Math.Max(0.25f, Math.Min(1, value)); 
    }

    private Matrix4x4 GetTransform()
    {      
      var viewport = new int[4]; // 0, 0, W, H
      gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);

      float scaleX = Scale * ImageSize.Width / viewport[2];
      float scaleY = Scale * ImageSize.Height / viewport[3];

      return
        Matrix4x4.CreateTranslation(Location.X / scaleX, Location.Y / scaleY, 0) *
        Matrix4x4.CreateRotationZ(Angle) *
        Matrix4x4.CreateScale(scaleX, scaleY, 1);
    }
  }
}
