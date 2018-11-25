using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrlenDriver.Services
{
    public  class WebService
    {
        protected readonly Uri _baseUri;
        protected readonly string _token;
        protected readonly string _userID;
        protected readonly string _clientID;

        public WebService()
        {
            _baseUri = new Uri("https://pksorlen.pl/api");

        }
        

        public async Task<List<Route>> GetBusStopsAsync(string urlString)
        {
            var url = new Uri(_baseUri, new Uri(urlString));
          
            var data = await SendRequestAsyncWithDataResultList<Route>(url, httpMethod: HttpMethod.Get);
            var list = data.ToList() ;
           
            return list;
        }

       



        protected async Task<T> SendRequestAsync<T>(
                    Uri url, HttpMethod httpMethod = null,
                    IDictionary<string, string> headers = null,
                    object requestData = null, string parentTag = null)
        {
            try
            {
                var result = default(T);
                var json = CreateJsonContent(requestData, parentTag);
                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                //var content = CreateInputContent(json);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                System.Diagnostics.Debug.WriteLine(url);
                System.Diagnostics.Debug.WriteLine(json);

                System.Diagnostics.Debug.WriteLine(response);
                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(responseContent);
                    result = JsonConvert.DeserializeObject<T>(responseContent);
                }

                return result;

            }
            catch (Exception ex)
            {
                //var result = (T)Activator.CreateInstance(typeof(WebStatusData));
                //(result as WebStatusData).Errcode = "500";
                //(result as WebStatusData).Errmsg = ex.Message.ToString();
                //(result as WebStatusData).Status = Status.ERROR;
                return default(T);

            }

        }
        protected async Task<T> SendRequestAsyncWithDataResult<T, ListType>(
                    Uri url, string JsonEntry, HttpMethod httpMethod = null,
                    IDictionary<string, string> headers = null,
                    object requestData = null)
        {
            try
            {
                var result = default(T);
                var data = CreateHttpClient(url, httpMethod, headers);
                var response = await data.Item1.SendAsync(data.Item2, HttpCompletionOption.ResponseContentRead);
                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    JObject dane = JObject.Parse(content);
                    System.Diagnostics.Debug.WriteLine(dane);
                    string res = "";
                    if (string.IsNullOrEmpty(JsonEntry))
                        res = dane.ToString();
                    else
                        res = dane["result"][JsonEntry].ToString();

                    ListType searchResult = JsonConvert.DeserializeObject<ListType>(res);

                    result = JsonConvert.DeserializeObject<T>(content);
                }
                return result;
            }
            catch (Exception ex)
            {
                //var result = (T)Activator.CreateInstance(typeof(WebStatusData));
                //(result as WebStatusData).Errcode = "500";
                //(result as WebStatusData).Errmsg = ex.Message.ToString();
                //(result as WebStatusData).Status = Status.ERROR;
                return default(T);
            }

        }

        protected async Task<List<ListType>> SendRequestAsyncWithDataResultList<ListType>(
                    Uri url, HttpMethod httpMethod = null,
                    IDictionary<string, string> headers = null
                   )
        {

            var result = default(ListType);
            List<ListType> DataList = new List<ListType>();
            var data = CreateHttpClient(url, httpMethod, headers);
            var response = await data.Item1.SendAsync(data.Item2, HttpCompletionOption.ResponseContentRead);
            System.Diagnostics.Debug.WriteLine(url);

            System.Diagnostics.Debug.WriteLine(response);
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(content);
                JObject dane = JObject.Parse(content);
                System.Diagnostics.Debug.WriteLine(dane);

                IList<JToken> res = dane["route"]["route"].Children().ToList();
                foreach (JToken item in res)
                {
                    System.Diagnostics.Debug.WriteLine(item);
                    ListType searchResult = item.ToObject<ListType>();
                    DataList.Add(searchResult);
                }
            }
            return DataList;
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.StackTrace);
            //    return default(T);
            //}

        }

        private (HttpClient, HttpRequestMessage) CreateHttpClient(Uri url, HttpMethod httpMethod = null,
                   IDictionary<string, string> headers = null
                   )
        {
            var method = httpMethod ?? HttpMethod.Get;
            var request = new HttpRequestMessage(method, url);

            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);
            System.Diagnostics.Debug.WriteLine("REQ");
            System.Diagnostics.Debug.WriteLine(request);
            return (client, request);
        }
        private HttpRequestMessage CreateHttpClientJsonContent(Uri url, HttpMethod httpMethod = null,
                   IDictionary<string, string> headers = null,
                   object requestData = null, string ParentTag = null, IDictionary<string, string> keyValuesJson = null)
        {
            var method = httpMethod ?? HttpMethod.Get;
            var request = new HttpRequestMessage(method, url);
            var data = requestData == null ? null : JObject.FromObject(requestData);

            if (data != null)
            {
                if (keyValuesJson != null)
                {
                    data.Add("user_id", Int32.Parse(keyValuesJson["user_id"]));
                    keyValuesJson.Remove("user_id");
                    foreach (var item in keyValuesJson)
                    {
                        data.Add(item.Key, item.Value);
                    }
                }
                if (!string.IsNullOrEmpty(ParentTag))
                {
                    var temp = new JObject();
                    temp[ParentTag] = data;
                    data = temp;
                }
                System.Diagnostics.Debug.WriteLine(data);
                var content = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("input", data.ToString())
                };
                System.Diagnostics.Debug.WriteLine(request.Content);
                request.Content = new FormUrlEncodedContent(content);
            }
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    request.Headers.Add(h.Key, h.Value);
                }
            }


            return request;
        }
        private string CreateJsonContent(object requestData = null, string ParentTag = null)
        {
            string json = "";
            var data = requestData == null ? null : JObject.FromObject(requestData);

            if (data != null)
            {
                if (!string.IsNullOrEmpty(ParentTag))
                {
                    json = data.ToString();
                    //if (ParentTag.Equals(Status.Client))
                    //    json = JsonConvert.SerializeObject(new { Customer = data });
                    //else if (ParentTag.Equals(Status.Visit))
                    //    json = JsonConvert.SerializeObject(new { Appointment = data });

                }
                else
                    json = data.ToString();
            }
            return json;
        }
        private FormUrlEncodedContent CreateInputContent(string json)
        {
            System.Diagnostics.Debug.WriteLine("POST INPUT:");
            System.Diagnostics.Debug.WriteLine(json);
            var ContentList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("input", json.ToString())
            };
            return new FormUrlEncodedContent(ContentList);
        }

    }


}
