using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

public class FileSystemDataStreamProvider : MultipartFormDataStreamProvider
{
    public FileSystemDataStreamProvider(string path) : base(path) { }

    public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
    {
        //TODO: pull from config or pass as a parameter
        var extensions = new[] { "png", "gif", "jpg", "zip", "xml" };
        var filename = headers.ContentDisposition.FileName.Replace("\"", string.Empty);

        if (filename.IndexOf('.') < 0)
            return Stream.Null;

        var extension = filename.Split('.').Last();

        return extensions.Any(i => i.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
            ? base.GetStream(parent, headers) : Stream.Null;
    }

    public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
    {
        return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
    }
}