using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Common.Extensions
{
    public class FileExtensions
    {
        public static async Task<string> ReadLastLineAsync(string path, CancellationToken cancellationToken = default)
        {
            if (File.Exists(path) == false)
                return null;

            return await Task.Run(() =>
            {
                using var sr = new StreamReader(path);

                string line;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (sr.Peek() == -1)
                        return line;
                }

                return null;
            });

            /*

            var encoding = Encoding.ASCII;

            if (resultEncoding == null)
                resultEncoding = Encoding.UTF8;

            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/ff6c07e2-9c36-4490-a989-f24dcff76145/how-to-read-only-the-last-line-out-of-a-very-big-text-file?forum=netfxbcl
            return await Task.Run(() =>
            {
                var newLine = "\n";
                int charSize = encoding.GetByteCount("\n");

                byte[] buffer = encoding.GetBytes(newLine);

                using var stream = new FileStream(path, FileMode.Open);

                long endpos = stream.Length / charSize;

                for (long pos = charSize; pos < endpos; pos += charSize)
                {
                    stream.Seek(-pos, SeekOrigin.End);

                    stream.Read(buffer, 0, buffer.Length);

                    var str = encoding.GetString(buffer);

                    // pos != charSize -> son satır boş ise o satırı almaması için kullanılır

                    if (str == newLine ) 
                    {
                        buffer = new byte[stream.Length - stream.Position];
                        stream.Read(buffer, 0, buffer.Length);

                        return resultEncoding.GetString(buffer);
                    }
                }

                // Eğer sadece 1 satır varsa o satır alınır
                stream.Seek(0, SeekOrigin.Begin);
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return encoding.GetString(buffer); // metnin başında ??? çıkıyor

                return null;

            }, cancellationToken);
            */
        }
    }
}
