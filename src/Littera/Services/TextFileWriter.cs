namespace Littera.Services;

/// <summary>Grava texto no encoding escolhido sem nunca deixar o arquivo original pela metade.</summary>
internal static class TextFileWriter
{
    public static async Task SaveAsync(string path, string text, TextEncodingKind encoding)
    {
        string temp = Path.Combine(Path.GetDirectoryName(path)!, "." + Path.GetFileName(path) + ".notas-tmp");
        try
        {
            await using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 16, FileOptions.Asynchronous))
            await using (var writer = new StreamWriter(stream, encoding.ToEncoding(), 1 << 16))
                await writer.WriteAsync(text.AsMemory());

            // Replace preserva atributos e permissões do original; Move cobre o arquivo novo.
            if (File.Exists(path))
                File.Replace(temp, path, destinationBackupFileName: null);
            else
                File.Move(temp, path);
        }
        finally
        {
            if (File.Exists(temp))
                File.Delete(temp);
        }
    }
}
