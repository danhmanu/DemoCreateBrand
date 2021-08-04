using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace QLK_API_Client.Model
{
    public class ProviderAPI
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlApi"></param>
        /// <param name="method"></param>
        /// <param name="i_ParaExtendURL"></param>
        /// <param name="i_HeadersPlus"></param>
        /// <param name="i_Parameter"></param>
        /// <param name="i_BodyJson"></param>
        /// <returns></returns>
        public string CallService(string urlApi, string method, Dictionary<string, string> i_ParaExtendURL, Dictionary<string, string> i_HeadersPlus, Dictionary<string, string> i_Parameter, string i_BodyJson)
        {
            try
            {
                string extendURL = "";
                if (i_ParaExtendURL != null && i_ParaExtendURL.Count > 0)
                {
                    foreach (var key in i_ParaExtendURL.Keys)
                    {
                        extendURL = string.Format("{0}/{1}", extendURL, i_ParaExtendURL[key]);
                    }
                }

                Task<string> task = null;

                task = Task.Run(() => CallApiService(urlApi, method, extendURL, i_HeadersPlus, i_Parameter, i_BodyJson));

                task.Wait();
                return task.Result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlApi"></param>
        /// <param name="method"></param>
        /// <param name="i_ExtendURL"></param>
        /// <param name="i_Headers"></param>
        /// <param name="i_Parameter"></param>
        /// <param name="i_BodyJson"></param>
        /// <returns></returns>
        private async Task<string> CallApiService(string urlApi, string method, string i_ExtendURL, Dictionary<string, string> i_Headers, Dictionary<string, string> i_Parameter, string i_BodyJson)
        {
            try
            {
                if (i_Headers == null)
                {
                    i_Headers = new Dictionary<string, string>();
                }
                i_Headers.Add("IdRequest", Guid.NewGuid().ToString());


                string fullUri = urlApi + "/" + i_ExtendURL;

                HttpRequestMessage request = null;
                switch (method)
                {
                    case "HttpGet":
                        request = CreateRequest(HttpMethod.Get, fullUri, i_Headers, i_Parameter, null);
                        break;
                    case "HttpPost":
                        request = CreateRequest(HttpMethod.Post, fullUri, i_Headers, i_Parameter, i_BodyJson);
                        break;
                    case "HttpDelete":
                        request = CreateRequest(HttpMethod.Delete, fullUri, i_Headers, i_Parameter, i_BodyJson);
                        break;
                    case "HttpPut":
                        request = CreateRequest(HttpMethod.Put, fullUri, i_Headers, i_Parameter, i_BodyJson);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                TimeSpan t = new TimeSpan(0,0,120);
                return await SendRequest(request, t);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i_HttpMethod"></param>
        /// <param name="i_Uri"></param>
        /// <param name="i_Headers"></param>
        /// <param name="i_Parameter"></param>
        /// <param name="i_BodyJson"></param>
        /// <returns></returns>
        private HttpRequestMessage CreateRequest(HttpMethod i_HttpMethod, string i_Uri, Dictionary<string, string> i_Headers, Dictionary<string, string> i_Parameter, string i_BodyJson)
        {
            try
            {

                UriBuilderExt builder = new UriBuilderExt(i_Uri);
                // add parameter
                if (i_Parameter != null && i_Parameter.Count > 0)
                {
                    foreach (var key in i_Parameter.Keys)
                    {
                        builder.AddParameter(key, i_Parameter[key]);
                    }
                }

                HttpRequestMessage request = new HttpRequestMessage(i_HttpMethod, builder.Uri);

                if (request != null)
                {

                    // add header
                    if (i_Headers != null && i_Headers.Count > 0)
                    {
                        foreach (var key in i_Headers.Keys)
                        {
                            request.Headers.Add(key, i_Headers[key]);
                        }
                    }

                    // add body
                    if (i_BodyJson != null)
                    {
                        request.Content = new StringContent(i_BodyJson, Encoding.UTF8, "application/json");
                    }

                    //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);                           
                }

                return request;
            }
            catch (HttpRequestException ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i_Request"></param>
        /// <param name="i_TimeOut">Second Timeout</param>
        /// <returns></returns>
        private async Task<string> SendRequest(HttpRequestMessage i_Request, TimeSpan i_TimeOut)
        {
            try
            {

                CancellationTokenSource source = new CancellationTokenSource();
                source.CancelAfter(i_TimeOut);
                CancellationToken cancellationToken = source.Token;

                HttpClient httpClientService = new HttpClient();
                httpClientService.Timeout = Timeout.InfiniteTimeSpan;

                using (var response = await httpClientService.SendAsync(i_Request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    string servermessage = String.Empty;
                    if (response.IsSuccessStatusCode)
                        return await StreamToStringAsync(stream);
                    else
                        servermessage = await StreamToStringAsync(stream);
                    //LoggerForSerilog2.LogError(response); //log sendRequest khong thanh cong

                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.Unauthorized:
                            throw new System.UnauthorizedAccessException(response.ToString());
                        case System.Net.HttpStatusCode.Forbidden:
                            throw new System.UnauthorizedAccessException(response.ToString());
                        case System.Net.HttpStatusCode.RequestTimeout:
                            throw new System.TimeoutException();
                        default:
                            throw new HttpRequestException(String.Format("Client:{0} Server: {1}", response.ToString(), servermessage));
                    }

                }
            }
            catch (TaskCanceledException ex)
            {
                throw new TimeoutException("Time out exception :", ex);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //
        private async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
                using (var sr = new StreamReader(stream))
                    content = await sr.ReadToEndAsync();

            return content;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class UriBuilderExt
    {
        private NameValueCollection collection;
        private UriBuilder builder;
        //
        public UriBuilderExt(string uri)
        {
            builder = new UriBuilder(uri);
            collection = HttpUtility.ParseQueryString(string.Empty);
        }

        public void AddParameter(string key, string value)
        {
            collection.Add(key, value);
        }

        public Uri Uri
        {
            get
            {
                builder.Query = collection.ToString();
                return builder.Uri;
            }
        }
    }
}
