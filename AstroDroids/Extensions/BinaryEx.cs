using System.IO;
using System.Text;

namespace AstroDroids.Extensions
{
    public static class BinaryEx
    {
        public static void WriteFixedString(this BinaryWriter writer, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            writer.Write(bytes);
        }

        public static string ReadFixedString(this BinaryReader reader, int length)
        {
            byte[] bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
