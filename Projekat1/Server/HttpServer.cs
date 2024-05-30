using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Projekat1.Cache;
using Projekat1.LogDir;
using Projekat1.Services;
using FileInfo = Projekat1.Models.FileInfo;

namespace Projekat1.Server
{
    public class HttpServer
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private HttpListener _listener;
        private Dictionary<string, List<FileInfo>> _dictionary;
        private CacheMemory cacheMemory = new CacheMemory();
        private byte[][] responses;
        public HttpServer(string prefix)
        {
            this._listener = new HttpListener();
            this._listener.Prefixes.Add(prefix);
            this.responses = new[] { Encoding.UTF8.GetBytes("This isn't file"),Encoding.UTF8.GetBytes("The file is not found"),Encoding.UTF8.GetBytes("The file is found but extension is bad") };
        }

        public async Task start()
        {
            _listener.Start();
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync();
                _ = handleRequest(context);
                //ThreadPool.QueueUserWorkItem(handleRequest, _listener.GetContext());
            }
            _listener.Close();
        }

        public async Task stop()
        {
            _cancellationTokenSource.Cancel();
        }
        private async Task handleRequest(object data)
        {
            try
            {
                HttpListenerContext listenerContext = (HttpListenerContext)data;
                var request = listenerContext.Request;
                var respone = listenerContext.Response;
                var url = request.Url.AbsolutePath;
                var cacheLog = cacheMemory.getLog(url);
                if (cacheLog.Item1)
                {
                    Console.WriteLine("Koristi se vrednost iz hasha");
                    await sendResponseWithoutSave(respone, cacheLog.Item2.statusCode, cacheLog.Item2.contentLength,
                        cacheLog.Item2.content, cacheLog.Item2.contentType);
                    return;
                }

                var urlPath = request.Url.AbsolutePath.Substring(1);
                var ext = FileService.GetFileExtension(urlPath);
                if (ext == "")
                {
                    await sendResponse(respone, HttpStatusCode.BadRequest, responses[0].Length, responses[0],
                        "text/plain", url);
                }
                else
                {
                    var fileName = urlPath.Substring(0, urlPath.Length - ext.Length);
                    try
                    {
                        var fileInfo = FileService.findFile(fileName, ext);
                        if (fileInfo == null)
                        {
                            await sendResponse(respone, HttpStatusCode.NotFound, responses[1].Length, responses[1],
                                "text/plain", url);
                        }
                        else
                        {
                            var fileBytes = File.ReadAllBytes(fileInfo.Path + "/" + fileInfo.Name + fileInfo.Extension);
                            await sendResponse(respone, HttpStatusCode.OK, fileBytes.Length, fileBytes, "image/jpg",
                                url);
                        }
                    }
                    catch (BadExtensionExcpetion e)
                    {
                        await sendResponse(respone, HttpStatusCode.BadRequest, responses[2].Length, responses[2],
                            "text/plain", url);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Desila se greska");
            }
        }

        
        private async Task sendResponseWithoutSave(HttpListenerResponse response, HttpStatusCode code, long length, byte[] bytes,string contentType)
        {
            response.StatusCode = (int)code;
            response.ContentLength64 = length;
            response.ContentType = contentType;
            await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            response.OutputStream.Close();
            response.Close();
        }

        private async Task sendResponse(HttpListenerResponse response, HttpStatusCode code, long length, byte[] bytes,string contentType,string url)
        {
            var t=sendResponseWithoutSave(response,code,length,bytes,contentType);
            if (code == HttpStatusCode.OK)
            {
                cacheMemory.writeResponse(url, new Log(code, length, contentType, bytes));
            }
            await t;
        }
    }
}