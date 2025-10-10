using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;
using VE3NEA;

namespace SkyRoof
{
  public class SpriteRenderer
  {
    private OpenGL gl;
    private ShaderProgram ShaderProgram;
    private VertexBufferArray VertexBufferArray;

    public SpriteRenderer(OpenGL gl)
    {
      this.gl = gl;

      gl.Enable(OpenGL.GL_BLEND);
      gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

      CreateVba();
      LoadShaders();
    }

    private void LoadShaders()
    {
      ShaderProgram = new ShaderProgram();

      ShaderProgram.Create(gl,
        Encoding.ASCII.GetString(Properties.Resources.SpriteVertexShader),
        Encoding.ASCII.GetString(Properties.Resources.SpriteFragmentShader),
        null);

      ShaderProgram.AssertValid(gl);
      ShaderProgram.Bind(gl);

      gl.Uniform1(ShaderProgram.GetUniformLocation(gl, "image"), 0);
    }

    private void CreateVba()
    {
      VertexBufferArray = new VertexBufferArray();
      VertexBufferArray.Create(gl);
      VertexBufferArray.Bind(gl);

      // x, y, s, t
      var vertices = new float[] { -1, -1, 0, 1,    -1, 1, 0, 0,    1, -1, 1, 1,    1, 1, 1, 0 };

      var vertexDataBuffer = new VertexBuffer();
      vertexDataBuffer.Create(gl);
      vertexDataBuffer.Bind(gl);
      vertexDataBuffer.SetData(gl, 0, vertices, false, 4);

      VertexBufferArray.Unbind(gl);
    }

    internal void DrawSprites(Sprite[] sprites)
    {
      ShaderProgram.Bind(gl);
      CheckError();

      VertexBufferArray.Bind(gl);
      CheckError();

      gl.ActiveTexture(OpenGL.GL_TEXTURE0);
      CheckError();

      foreach (var sprite in sprites) DrawSprite(sprite);

      VertexBufferArray.Unbind(gl);
      CheckError();

      ShaderProgram.Unbind(gl);
      CheckError();
    }

    private void DrawSprite(Sprite sprite)
    {
      if (!sprite.Enabled) return;

      gl.BindTexture(OpenGL.GL_TEXTURE_2D, sprite.TextureId);
      CheckError();

      var matrixId = ShaderProgram.GetUniformLocation(gl, "transform");
      gl.UniformMatrix4(matrixId, 1, false, MatrixToFloats(sprite.Transform));
      CheckError();

      gl.DrawArrays(OpenGL.GL_TRIANGLE_STRIP, 0, 4);
      CheckError();
    }

    private float[] MatrixToFloats(Matrix4x4 mx)
    {
      return
      [
        mx.M11, mx.M12, mx.M13, mx.M14,
        mx.M21, mx.M22, mx.M23, mx.M24,
        mx.M31, mx.M32, mx.M33, mx.M34,
        mx.M41, mx.M42, mx.M43, mx.M44
      ];
    }

    internal void CheckError(bool log = true)
    {
      ExceptionLogger.CheckOpenglError(gl, log);
    }
  }
}
