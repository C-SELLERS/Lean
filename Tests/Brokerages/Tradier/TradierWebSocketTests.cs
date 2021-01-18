/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System.Threading;
using NUnit.Framework;
using QuantConnect.Brokerages.Tradier;
using QuantConnect.Configuration;
using QuantConnect.Tests.Configuration;
using QuantConnect.Util.RateLimit;

namespace QuantConnect.Tests.Brokerages.Tradier
{
    [TestFixture]
    public class TradierWebSocketTests
    {
        [Test]
        public void TestMarketEvents()
        {
            var apiKey = Config.Get("tradier-access-token");
            var websocketClient = new TradierWebSocketWrapper(apiKey, "/markets/events");

            while (true)
            {

            }
        }

        [Test]
        public void TestAccountEvents()
        {
            var apiKey = Config.Get("tradier-access-token");
            var accountid = Config.Get("tradier-account-id");

            var websocketClient = new TradierWebSocketWrapper(apiKey, $"accounts/{accountid}/orders");

            while (true)
            {

            }
        }
    }
}
