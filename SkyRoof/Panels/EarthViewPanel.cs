using System.Text;
using SharpGL.VertexBuffers;
using SharpGL;
using WeifenLuo.WinFormsUI.Docking;
using SharpGL.Shaders;
using VE3NEA;
using System.Diagnostics;
using System.Drawing.Imaging;
using Serilog;
using static System.Windows.Forms.AxHost;

namespace SkyRoof
{
  public partial class EarthViewPanel : DockContent
  {
    private readonly Context ctx;
    private OpenGL gl;
    private ShaderProgram ShaderProgram;
    private VertexBufferArray VertexBufferArray;
    private uint[] TextureIds = new uint[1];
    private int VertexCount;
    private SpriteRenderer SpriteRenderer;
    private Sprite SatelliteSprite, HomeSprite, NorthSprite, SouthSprite;
    private double Footprint;
    private double Azimuth;

    public GeoPoint Home, Center;
    public double Zoom = 1;
    public SatnogsDbSatellite Satellite;


    public EarthViewPanel() { InitializeComponent(); }

    public EarthViewPanel(Context ctx)
    {
      this.ctx = ctx;
      Log.Information("Creating EarthViewPanel");
      InitializeComponent();

      ctx.EarthViewPanel = this;
      ctx.MainForm.EarthViewMNU.Checked = true;

      SetGridSquare();
      SetSatellite();

      openglControl1.MouseWheel += OpenglControl1_MouseWheel;
    }

    public void SetGridSquare()
    {
      Home = GridSquare.ToGeoPoint(ctx.Settings.User.Square);
    }

    private void EarthViewPanel_FormClosing(object sender, FormClosingEventArgs e)
    {
      Log.Information("Closing EarthViewPanel");
      ctx.EarthViewPanel = null;
      ctx.MainForm.EarthViewMNU.Checked = false;
    }

    private void openglControl1_Resize(object sender, EventArgs e)
    {
      openglControl1.Invalidate();
      ValidateZoom();
    }

    private void OpenglControl1_MouseWheel(object? sender, MouseEventArgs e)
    {
      Zoom *= 1 + e.Delta / 1200f;
      ValidateZoom();
    }

    private void ValidateZoom()
    {
      var diam = Math.Min(openglControl1.ClientSize.Width, openglControl1.ClientSize.Height);
      if (diam > 0) Zoom = Math.Max(1f, Math.Min(25000 / diam, Zoom));
    }

    internal void SetSatellite(SatnogsDbSatellite? satellite = null)
    {
      Satellite = satellite ?? ctx.SatelliteSelector.SelectedSatellite;
      label1.Text = Satellite.name;

      ComputeSatLocation();
      Zoom = 0.9 * Math.PI / Footprint;
      Invalidate();
    }
    public void Advance()
    {
      if (Satellite == null) return;

      ComputeSatLocation();
      Invalidate();
    }

    private void ComputeSatLocation()
    {
      var p = Satellite.Tracker.Predict();
      var nextP = Satellite.Tracker.Predict(DateTime.UtcNow.AddSeconds(10));

      if (p == null || nextP == null)
      {
        Center = Home;
        Footprint = Math.PI;
        Azimuth = 0;
        SatelliteSprite.Enabled = false;
      }
      else
      {
        Center = new(p.Latitude.Degrees, p.Longitude.Degrees);
        var nextCenter = new GeoPoint(nextP.Latitude.Degrees, nextP.Longitude.Degrees);
        Footprint = p.GetFootprintAngle().Radians;
        Azimuth = (nextCenter - Center).AzimuthRad;
        SatelliteSprite.Enabled = true;
      }
    }
    




    //----------------------------------------------------------------------------------------------
    //                                 init opengl objects
    //----------------------------------------------------------------------------------------------
    private void openglControl1_OpenGLInitialized(object sender, EventArgs e)
    {
      gl = openglControl1.OpenGL;
      CheckError(false);

      SpriteRenderer = new(gl);
      CheckError(false);
      SatelliteSprite = new Sprite(gl, Properties.Resources.satellite);
      HomeSprite = new Sprite(gl, Properties.Resources.ant);
      NorthSprite = new Sprite(gl, Properties.Resources.N);
      SouthSprite = new Sprite(gl, Properties.Resources.S);

      gl.Disable(OpenGL.GL_DEPTH_TEST);
      CheckError();

      LoadShaders();
      CreateVba();
      LoadTexture();

      gl.Uniform1(ShaderProgram.GetUniformLocation(gl, "worldMapTexture"), 0);
      CheckError();
    }


    private void LoadShaders()
    {
      ShaderProgram = new ShaderProgram();
      CheckError();

      ShaderProgram.Create(gl,
        Encoding.ASCII.GetString(Properties.Resources.EarthVertexShader),
        Encoding.ASCII.GetString(Properties.Resources.EarthFragmentShader),
        null);
      CheckError();

      ShaderProgram.AssertValid(gl);
      CheckError();

      ShaderProgram.Bind(gl);
      CheckError();
    }


    private const int PHI_STEPS = 192;
    private const int RO_STEPS = 96;
    private const float TwoPi = (float)(2 * Math.PI);

    private void CreateVba()
    {
      CheckError(false);

      VertexBufferArray = new VertexBufferArray();
      CheckError();
      VertexBufferArray.Create(gl);
      CheckError();
      VertexBufferArray.Bind(gl);
      CheckError();

      // generate a grid of polar coordinates
      VertexCount = RO_STEPS * (PHI_STEPS + 1) * 2;
      var data = new float[VertexCount * 2];
      int idx = 0;

      for (int ro = 0; ro < RO_STEPS; ro++)
        for (int phi = 0; phi <= PHI_STEPS; phi++)
        {
          data[idx++] = ro / (float)RO_STEPS;
          data[idx++] = phi * TwoPi / PHI_STEPS;
          data[idx++] = (ro + 1) / (float)RO_STEPS;
          data[idx++] = phi * TwoPi / PHI_STEPS;
        }

      var dataBuffer = new VertexBuffer();
      CheckError();
      dataBuffer.Create(gl);
      CheckError();
      dataBuffer.Bind(gl);
      CheckError();
      dataBuffer.SetData(gl, 0, data, false, 2);
      CheckError();

      VertexBufferArray.Unbind(gl);
      CheckError();
    }

    private void LoadTexture()
    {
      CheckError(false);

      gl.GenTextures(1, TextureIds);
      CheckError();

      gl.BindTexture(OpenGL.GL_TEXTURE_2D, TextureIds[0]);
      CheckError(false); // produces false positives

      gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);    // shrink
      gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);    // stretch
      gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE); // lat
      gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);        // lon


      //using (Bitmap bitmap = new Bitmap(@"C:\Users\Alex\Desktop\sat\maps\worldMapTexture.bmp"))
      //using (Bitmap bitmap = new Bitmap(Properties.Resources.NaturalEarth))
      //using (Bitmap bitmap = new Bitmap(Properties.Resources.dxcc))
      using (Bitmap bitmap = Utils.BitmapFromBytes(Properties.Resources.dxcc))
      {
        BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
          ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

        gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB, bitmap.Width, bitmap.Height, 0,
          OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);

        bitmap.UnlockBits(data);
      }
      CheckError();
    }




    //----------------------------------------------------------------------------------------------
    //                                        draw
    //----------------------------------------------------------------------------------------------
    private void openglControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
    {
      var gl = openglControl1.OpenGL;
      CheckError(false);

      ShaderProgram.Bind(gl);
      CheckError();

      gl.ActiveTexture(OpenGL.GL_TEXTURE0);
      CheckError();
      gl.BindTexture(OpenGL.GL_TEXTURE_2D, TextureIds[0]);
      CheckError();

      VertexBufferArray.Bind(gl);
      CheckError();

      ShaderProgram.SetUniform1(gl, "in_homeLat", (float)Center.LatitudeRad);
      CheckError();
      ShaderProgram.SetUniform1(gl, "in_homeLon", (float)Center.LongitudeRad);
      CheckError();
      ShaderProgram.SetUniform1(gl, "in_footprint", (float)Footprint);
      CheckError();
      ShaderProgram.SetUniform1(gl, "in_aspect", openglControl1.ClientSize.Width / (float)openglControl1.ClientSize.Height);
      CheckError();
      ShaderProgram.SetUniform1(gl, "in_zoom", (float)Zoom);
      CheckError();

      gl.ClearColor(0.7f, 0.7f, 0.7f, 1);
      CheckError();
      gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT);
      CheckError();
      gl.DrawArrays(OpenGL.GL_TRIANGLE_STRIP, 0, VertexCount);
      CheckError();

      VertexBufferArray.Unbind(gl);
      CheckError();
      ShaderProgram.Unbind(gl);
      CheckError();

      SetSpriteAttributes();
      SpriteRenderer.DrawSprites([SatelliteSprite, HomeSprite, NorthSprite, SouthSprite]);
      CheckError();
    }

    private void SetSpriteAttributes()
    {
      // control size, pixels
      var size = openglControl1.ClientSize;
      // diameter of the inscribed circle, pixels
      var diam = Math.Min(size.Width, size.Height);

      // scale sprites to look nice at all control sizes
      var scale = 0.6f + diam * 0.0007f;
      SatelliteSprite.Scale = scale;
      HomeSprite.Scale = scale;
      NorthSprite.Scale = 0.4f * scale;
      SouthSprite.Scale = 0.4f * scale;

      // satellite points in the direction of the next position
      SatelliteSprite.Angle = (float)(-Azimuth + Math.PI / 4);

      // home location in the azimuthal projection
      HomeSprite.Location = ComputeLocation(Home);

      // N and S at the poles
      NorthSprite.Location = ComputeLocation(GeoPoint.NorthPole);
      SouthSprite.Location = ComputeLocation(GeoPoint.SouthPole);
    }

    private PointF ComputeLocation(GeoPoint point)
    {
      var path = point - Center;
      var ro = path.DistanceRad / Math.PI * Zoom;
      var phi = Geo.HalfPi - path.AzimuthRad;
      var size = openglControl1.ClientSize;
      var diam = Math.Min(size.Width, size.Height);

      return new PointF(
        (float)(ro * Math.Cos(phi) * diam / size.Width),
        (float)(ro * Math.Sin(phi) * diam / size.Height));
    }



    //----------------------------------------------------------------------------------------------
    //                                   error handling
    //----------------------------------------------------------------------------------------------
    private void CheckError(bool log = true)
    {
      ExceptionLogger.CheckOpenglError(gl, log);
    }
  }
}
