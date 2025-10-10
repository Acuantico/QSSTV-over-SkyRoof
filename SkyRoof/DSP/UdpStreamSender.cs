using System.Net;
using System.Net.Sockets;
using MathNet.Numerics;
using VE3NEA;

namespace SkyRoof
{
  public class UdpStreamSender
  {
    private const int PacketSize = 1472; // Maximum UDP packet size
    private const int FloatSize = sizeof(float);
    private const int ComplexSize = FloatSize * 2; 

    public Context ctx;
    private UdpClient UdpClient = new();
    private byte[] bytes = new byte[PacketSize];
    private int byteCount = 0;

    public void Send(float[] data)
    {
      if (!ctx.Settings.OutputStream.Enabled) return;

      for (int i = 0; i < data.Length; i++)
      {
        BitConverter.GetBytes(data[i]).CopyTo(bytes, byteCount);

        byteCount += FloatSize;
        if (byteCount == PacketSize)
        {
          UdpClient.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Loopback, ctx.Settings.OutputStream.UdpPort));
          byteCount = 0;
        }
      }
    }

    public void Send(Complex32[] data)
    {
      if (!ctx.Settings.OutputStream.Enabled) return;

      for (int i = 0; i < data.Length; i++)
      {
        BitConverter.GetBytes(data[i].Real).CopyTo(bytes, byteCount);
        BitConverter.GetBytes(data[i].Imaginary).CopyTo(bytes, byteCount + FloatSize);

        byteCount += ComplexSize;
        if (byteCount == PacketSize)
        {
          UdpClient.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Loopback, ctx.Settings.OutputStream.UdpPort));
          byteCount = 0;
        }
      }
    }
  }
}
