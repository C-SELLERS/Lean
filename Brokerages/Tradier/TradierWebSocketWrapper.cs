using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using QuantConnect.Logging;
using RestSharp;

namespace QuantConnect.Brokerages.Tradier
{
    /// <summary>
    /// 
    /// </summary>
    public class TradierWebSocketWrapper : WebSocketClientWrapper
    {
        private const string BaseUrl = "wss://ws.tradier.com/v1";
        private string _endpoint;

        private readonly string _apiKey;
        private string _url;
        private readonly ISymbolMapper _symbolMapper;
        private string _sessionId; // Storing tradier session ID
        private DateTime _sessionIdExpiration = DateTime.MinValue;

        /// <summary>
        /// Creates a new instance of the <see cref="TradierWebSocketWrapper"/> class
        /// </summary>
        /// <param name="apiKey">Tradier apiKey</param>
        /// <param name="endpoint">Endpoint </param>
        public TradierWebSocketWrapper(string apiKey, string endpoint)
        {
            _apiKey = apiKey;
            _endpoint = endpoint;


            _url = BaseUrl + _endpoint;
            Initialize(_url);
            SetSession();

            Open += OnOpen;
            Closed += OnClosed;
            Message += OnMessage;
            Error += OnError;
            Connect();
        }

        /// <summary>
        /// Create session for streaming with tradier using rest api
        /// </summary>
        private void SetSession()
        {
            if (_sessionId == null || _sessionIdExpiration < DateTime.UtcNow)
            {
                // Get session IDs
                var url = "https://api.tradier.com/v1" + _endpoint;
                var client = new RestClient(url);
                var request = new RestRequest("/session", Method.POST);
                request.AddHeader("Authorization", "Bearer " + _apiKey);

                var rawResponse = client.Execute(request);
                var result = JsonConvert.DeserializeObject<TradierSessionResponse>(rawResponse.Content);

                string sessionId;
                if (!result.Stream.TryGetValue("sessionid", out sessionId))
                {
                    Log.Error("TradierWebSocketWrapper.CreateSession(): Failed to create session");
                    // Log the error!!!
                }

                // This session ID expires in 5 minutes
                _sessionIdExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(5);
                _sessionId = sessionId;
            }
        }

        internal class TradierSessionResponse : RestResponse 
        {
            /// <summary>
            /// Collection of backtest nodes
            /// </summary>
            [JsonProperty(PropertyName = "stream")]
            public Dictionary<string, string> Stream;
        }

        /// <summary>
        /// Test method
        /// </summary>
        public void test()
        {
            var testarray = new string[] { "SPY" };

            var request = JsonConvert.SerializeObject(
                new
                {
                    symbols = testarray, 
                    sessionid = _sessionId,
                });

            Send(request);
        }

        private void OnError(object sender, WebSocketError e)
        {
            Log.Error(e.Message);
        }

        private void OnMessage(object sender, WebSocketMessage e)
        {
            Log.Trace(e.Message);
        }

        private void OnClosed(object sender, WebSocketCloseData e)
        {
            Log.Trace($"TradierWebSocket.OnClosed(): {e.Reason}");
        }

        private void OnOpen(object sender, EventArgs e)
        {
            Log.Trace($"TradierWebSocket.OnOpen(): connection open");
            test();
        }
    }
}
