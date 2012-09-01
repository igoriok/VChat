using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ICSharpCode.SharpZipLib.GZip;
using Microsoft.Phone.Reactive;

namespace VChat.Services.Vkontakte.Requests
{
    public abstract class ObservableWebRequest<TResult> : IObservable<TResult>
    {
        private readonly List<RequestParameter> _parameters = new List<RequestParameter>();

        private readonly Guid _boundary = Guid.NewGuid();

        public ObservableWebRequest<TResult> AddField(string name, string value)
        {
            _parameters.Add(new FieldParameter
            {
                Name = Uri.EscapeDataString(name),
                Value = Uri.EscapeDataString(value)
            });

            return this;
        }

        public ObservableWebRequest<TResult> AddFile(string name, string fileName, Stream stream)
        {
            _parameters.Add(new FileParameter
            {
                Name = Uri.EscapeDataString(name),
                FileName = Uri.EscapeDataString(fileName),
                Stream = stream
            });

            return this;
        }

        #region IObservable<TResult>

        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            var uri = BuildUri();
            var request = WebRequest.CreateHttp(uri);

            request.UserAgent = "VChat";

            // Enable compression
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";

            IObservable<WebResponse> observable;

            if (_parameters.Count == 0)
            {
                request.Method = "GET";

                observable = BuildGetRequest(request);
            }
            else
            {
                request.Method = "POST";
                request.ContentType = string.Format("multipart/form-data; boundary={0}", _boundary);

                observable = BuildPostRequest(request);
            }

            var abort = Disposable.Create(request.Abort);

            var subscribe = observable
                .Catch<WebResponse, WebException>(Catch)
                .Select(ReadResponse)
                .Subscribe(observer);

            return new CompositeDisposable(subscribe, abort);
        }

        #endregion

        protected abstract Uri BuildUri();

        protected abstract TResult ReadResult(Stream stream);

        private IObservable<WebResponse> BuildGetRequest(WebRequest request)
        {
            return Observable
                .Defer(Observable.FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse));
        }

        private IObservable<WebResponse> BuildPostRequest(WebRequest request)
        {
            var observable = BuildGetRequest(request);

            return Observable
                .Defer(Observable.FromAsyncPattern<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream))
                .Do(WriteRequest)
                .SelectMany(observable);
        }

        private void WriteRequest(Stream stream)
        {
            using (stream)
            {
                using (var writer = new StreamWriter(stream) { AutoFlush = true })
                {
                    var header = string.Format("--{0}", _boundary);
                    var footer = string.Format("--{0}--", _boundary);

                    foreach (var parameter in _parameters.OfType<FieldParameter>())
                    {
                        writer.WriteLine(header);

                        writer.WriteLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", parameter.Name));
                        writer.WriteLine();

                        writer.WriteLine(parameter.Value);
                    }

                    foreach (var parameter in _parameters.OfType<FileParameter>())
                    {
                        writer.WriteLine(header);

                        writer.WriteLine(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"", parameter.Name, parameter.FileName));
                        writer.WriteLine("Content-Type: application/octet-stream");
                        writer.WriteLine();

                        parameter.Stream.CopyTo(stream);
                        writer.WriteLine();
                    }

                    writer.WriteLine(footer);
                }
            }
        }

        private TResult ReadResponse(WebResponse response)
        {
            using (response)
            {
                using (var stream = response.GetResponseStream())
                {
                    if (response.Headers[HttpRequestHeader.ContentEncoding] == "gzip")
                    {
                        using (var gzip = new GZipInputStream(stream))
                        {
                            return ReadResult(gzip);
                        }
                    }

                    return ReadResult(stream);
                }
            }
        }

        private IObservable<WebResponse> Catch(WebException exception)
        {
            if (exception.Response == null)
            {
                return Observable.Throw<WebResponse>(exception);
            }

            return Observable.Return(exception.Response);
        }

        protected class RequestParameter
        {
            public string Name { get; set; }
        }

        protected class FieldParameter : RequestParameter
        {
            public string Value { get; set; }
        }

        protected class FileParameter : RequestParameter
        {
            public string FileName { get; set; }
            public Stream Stream { get; set; }
        }
    }
}