using System.Text;
using SharpGL.VertexBuffers;
using SharpGL;
using VE3NEA;
using SharpGL.Shaders;
using System.Buffers;
using Serilog;
using System.Windows.Forms;
using System.Diagnostics;

namespace SkyRoof
{
  // OpenGL Extensions Viewer: https://geeks3d.com/dl/show/10097


  public partial class WaterfallControl : UserControl
  {
    static bool OpenglInfoNeeded = true;

    private int TextureWidth;
    private int TextureFold;
    private int SpectraHeight;
    public int SpectraWidth;
    private int TextureHeight;
    
    
    private VertexBufferArray VertexBufferArray;
    private ShaderProgram ShaderProgram;
    public IndexedTexture IndexedTexture;
    private Palette Palette = new();
    internal double VisibleBandwidth = SdrConst.MAX_BANDWIDTH;

    public float Fps;

    public float Brightness = 0;
    public float Contrast = 0.5f;
    public double Zoom = 1.95f;
    public double Pan = 0;
    public double ScrollSpeed = 1;




    //----------------------------------------------------------------------------------------------
    //                                        init
    //----------------------------------------------------------------------------------------------
    public WaterfallControl()
    {
      InitializeComponent();
    }

    private void OpenglControl_OpenGLInitialized(object sender, EventArgs e)
    {
      OpenGL gl = OpenglControl.OpenGL;

      CheckError(gl, false);

      LogOpenglInformation();
      ChooseTextureSize();

    gl.Disable(OpenGL.GL_DEPTH_TEST);
      CheckError(gl);

      ShaderProgram = new ShaderProgram();
      CheckError(gl);
      ShaderProgram.Create(gl,
        Encoding.ASCII.GetString(Properties.Resources.WaterfallVertexShader),
        Encoding.ASCII.GetString(Properties.Resources.WaterfallFragmentShader),
        null);
      CheckError(gl);
      ShaderProgram.AssertValid(gl);
      CheckError(gl);
      ShaderProgram.Bind(gl);
      CheckError(gl);

      gl.Uniform1(ShaderProgram.GetUniformLocation(gl, "indexedTexture"), 0);
      CheckError(gl);
      gl.Uniform1(ShaderProgram.GetUniformLocation(gl, "paletteTexture"), 1);
      CheckError(gl);

      IndexedTexture = new(OpenglControl.OpenGL, TextureWidth, TextureHeight);
      SetPalette(Palette);

      CreateVba(gl);
    }

    private void LogOpenglInformation()
    {
      if (!OpenglInfoNeeded) return;
      OpenglInfoNeeded = false;

      OpenGL gl = OpenglControl.OpenGL;

      string str = gl.GetString(OpenGL.GL_VERSION);
      if (gl.GetError() != OpenGL.GL_NO_ERROR) str = "<Error>";
      Log.Information($"OpenGL version: {str}");

      str = gl.GetString(OpenGL.GL_VENDOR);
      if (gl.GetError() != OpenGL.GL_NO_ERROR) str = "<Error>";
      Log.Information($"OpenGL vendor: {str}");

      str = gl.GetString(OpenGL.GL_RENDERER);
      if (gl.GetError() != OpenGL.GL_NO_ERROR) str = "<Error>";
      Log.Information($"OpenGL renderer: {str}");

      str = gl.GetString(OpenGL.GL_SHADING_LANGUAGE_VERSION​);
      if (gl.GetError() != OpenGL.GL_NO_ERROR) str = "<Error>";
      Log.Information($"OpenGL shading language: {str}");

      int[] maxTextureSize = new int[1];
      gl.GetInteger(OpenGL.GL_MAX_TEXTURE_SIZE, maxTextureSize);
      Log.Information("OpenGL max. texture size: " + maxTextureSize[0]);
    }

    internal void ChooseTextureSize()
    {
      // max texture size
      Size maxTextureSize = GetMaxTextureSize();

      // screen hight
      int screenHeight = Screen.AllScreens.Max(s => s.Bounds.Height);

      // set texture size
      TextureWidth = maxTextureSize.Width;
      SpectraHeight = screenHeight > 1280 ? 2048 : 1024;
      int maxTextureFold = maxTextureSize.Height / SpectraHeight;

      SpectraWidth = Math.Min(1<<17, TextureWidth * maxTextureFold); // spectrum width up to 128K
      TextureFold = SpectraWidth / TextureWidth;
      TextureHeight = SpectraHeight * TextureFold;

      Log.Information($"Waterfall textue: max: {maxTextureSize.Width}x{maxTextureSize.Height}  " +
        $"used: {TextureWidth}x{TextureHeight}, " +
        $"spectra: {SpectraWidth}x{SpectraHeight}, fold: {TextureFold}");
    }

    private Size GetMaxTextureSize()
    {
      int[] intArray = new int[1];
      OpenglControl.OpenGL.GetInteger(OpenGL.GL_MAX_TEXTURE_SIZE, intArray);
      Size size = new Size(intArray[0], intArray[0]);

      while (size.Height > 64 && !TryAllocateTexture(size))
          size.Height /= 2;

      return size;
    }

    private bool TryAllocateTexture(Size size)
    {
      OpenGL gl = OpenglControl.OpenGL;

      uint[] texture = new uint[1];
      gl.GenTextures(1, texture);
      gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture[0]);

      while (gl.GetError() != OpenGL.GL_NO_ERROR) { }

      gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_R32F, size.Width, size.Height, 0,
        OpenGL.GL_RED, OpenGL.GL_BYTE, IntPtr.Zero);

      var error = gl.GetError();
      gl.DeleteTextures(1, texture);

      return error == OpenGL.GL_NO_ERROR;
    }

    internal void SetPalette(Palette palette)
    {
      Palette = palette;
      IndexedTexture.SetPalette(Palette.Colors);
    }

    private void CreateVba(OpenGL gl)
    {
      CheckError(gl, false);

      VertexBufferArray = new VertexBufferArray();
      CheckError(gl);
      VertexBufferArray.Create(gl);
      CheckError(gl);
      VertexBufferArray.Bind(gl);
      CheckError(gl);

      var vertices = new float[] { -1, -1, -1, 1, 1, -1, 1, 1 };
      var vertexDataBuffer = new VertexBuffer();
      CheckError(gl);
      vertexDataBuffer.Create(gl);
      CheckError(gl);
      vertexDataBuffer.Bind(gl);
      CheckError(gl);
      vertexDataBuffer.SetData(gl, 0, vertices, false, 2);
      CheckError(gl);

      var texCoords = new float[] { 0, 1, 0, 0, 1, 1, 1, 0 };
      var texCoordsBuffer = new VertexBuffer();
      CheckError(gl);
      texCoordsBuffer.Create(gl);
      CheckError(gl);
      texCoordsBuffer.Bind(gl);
      CheckError(gl);
      texCoordsBuffer.SetData(gl, 1, texCoords, false, 2);
      CheckError(gl);

      VertexBufferArray.Unbind(gl);
      CheckError(gl);
    }

    internal static void CheckError(OpenGL gl, bool log = true)
    {
      ExceptionLogger.CheckOpenglError(gl, log);
    }





    //----------------------------------------------------------------------------------------------
    //                                      draw
    //----------------------------------------------------------------------------------------------
    private void OpenglControl_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
    {
      var gl = OpenglControl.OpenGL;

      CheckError(gl, false);

      ShaderProgram.Bind(gl);
      CheckError(gl);
      IndexedTexture.Bind(gl);
      CheckError(gl);
      VertexBufferArray.Bind(gl);
      CheckError(gl);

      //UpdateScrollPosOnDraw();
      float scrollHeight = OpenglControl.Size.Height / (float)SpectraHeight;

      // a bug in SharpGL prevents ShaderProgram.SetUniform1 from setting int 
      gl.Uniform1(ShaderProgram.GetUniformLocation(gl, "in_ScreenWidth"), OpenglControl.Size.Width);
      CheckError(gl);

      ShaderProgram.SetUniform1(gl, "in_zoom", (float)Zoom);
      CheckError(gl);
      ShaderProgram.SetUniform1(gl, "in_pan", (float)Pan);
      CheckError(gl);

      ShaderProgram.SetUniform1(gl, "in_ScrollPos", (float)ScrollPos);
      CheckError(gl);
      ShaderProgram.SetUniform1(gl, "in_ScrollHeight", scrollHeight);
      CheckError(gl);
      ShaderProgram.SetUniform1(gl, "in_Brightness", computeBrightness());
      CheckError(gl);
      ShaderProgram.SetUniform1(gl, "in_Contrast", Contrast);
      CheckError(gl);

      gl.Uniform1(ShaderProgram.GetUniformLocation(gl, "in_TextureFold"), TextureFold);
      CheckError(gl);
      gl.Uniform1(ShaderProgram.GetUniformLocation(gl, "in_TextureWidth"), TextureWidth);
      CheckError(gl);
      gl.Uniform1(ShaderProgram.GetUniformLocation(gl, "in_SpectraHeight"), SpectraHeight);
      CheckError(gl);

      gl.DrawArrays(OpenGL.GL_TRIANGLE_STRIP, 0, 4);
      CheckError(gl);
      VertexBufferArray.Unbind(gl);
      CheckError(gl);

      ShaderProgram.Unbind(gl);
      CheckError(gl);
      UpdateFps();
    }


    // correct brightness for spectrum shrinking, horizontal and vertical
    // the formula is a linear regression of the brightness vs. log resolution
    // where the slope and intercept are functions of the scrolling speed
    private float computeBrightness()
    {
      double pixPerHz = OpenglControl.Size.Width / VisibleBandwidth;
      double a = 0.21821866 * ScrollSpeed + 1.96776626;
      double b = -1.19227144 * ScrollSpeed + 55.37794533;
      double brightness = a * Math.Log(pixPerHz) + b;    // 0..100
      return Brightness + (float)(brightness / 50 - 1);  //  -1..1
    }

    int frameCount;
    DateTime startTime;
    private void UpdateFps()
    {
      DateTime currentTime = DateTime.Now;
      frameCount++;
      var elapsed = (currentTime - startTime).TotalSeconds;
      if (elapsed < 1) return;

      Fps = (float)(frameCount / elapsed);
      startTime = currentTime;
      frameCount = 0;
    }

    ArrayPool<float> ArrayPool = ArrayPool<float>.Shared;
    public int Row;                // write positon, 0..TEXTURE_HEIGHT-1
    private double ScrollPos = 0;  // display position in texture, 0..1

    internal void AppendSpectrum(float[] spectrum)
    {
      if (!IsHandleCreated || !Enabled) return;

      // spectrum may be overwritten after this function returns, make a copy
      var spectrumCopy = ArrayPool.Rent(spectrum.Length);
      Array.Copy(spectrum, spectrumCopy, spectrum.Length);

      BeginInvoke(() =>
      {
        IndexedTexture.SetRows(Row * TextureFold, TextureFold, spectrumCopy);
        ArrayPool.Return(spectrumCopy);

        if (++Row == SpectraHeight) Row = 0;
        ScrollPos = Row / (float)SpectraHeight;
        OpenglControl.Invalidate();
      });
    }
  }
}