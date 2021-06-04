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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

// Collection of response objects for Quantconnect Data/ endpoints
namespace QuantConnect.Api
{
    /// <summary>
    /// Data/Read response wrapper, contains link to requested data
    /// </summary>
    public class DataLink : RestResponse
    {
        /// <summary>
        /// Url to the data requested
        /// </summary>
        [JsonProperty(PropertyName = "link")]
        public string Url { get; set; }

        /// <summary>
        /// Remaining USD balance on account after this transaction
        /// </summary>
        [JsonProperty(PropertyName = "balance")]
        public double BalanceUSD { get; set; }

        /// <summary>
        /// Remaining QCC balance on account after this transaction
        /// </summary>
        public double BalanceQCC => BalanceUSD * 100;

        /// <summary>
        /// USD Cost or this data link
        /// </summary>
        [JsonProperty(PropertyName = "cost")]
        public double CostUSD { get; set; }

        /// <summary>
        /// QCC Cost for this data link
        /// </summary>
        public double CostQCC => CostUSD * 100;
    }

    /// <summary>
    /// Data/List response wrapper for available data
    /// </summary>
    public class DataList : RestResponse
    {
        /// <summary>
        /// List of all available data from this request
        /// </summary>
        [JsonProperty(PropertyName = "objects")]
        public List<string> AvailableData { get; set; }
    }

    /// <summary>
    /// Data/Prices response wrapper for prices by vendor
    /// </summary>
    public class DataPricesList : RestResponse
    {
        /// <summary>
        /// Collection of prices objects
        /// </summary>
        [JsonProperty(PropertyName = "prices")]
        public List<PriceEntry> Prices { get; set; }

        /// <summary>
        /// The Agreement URL for this Organization
        /// </summary>
        [JsonProperty(PropertyName = "agreement")]
        public string AgreementUrl { get; set; }

        /// <summary>
        /// Get the price for a given data file
        /// </summary>
        /// <param name="path">Lean data path of the file</param>
        /// <returns>Price for data, -1 if no entry found</returns>
        public int GetPrice(string path)
        {
            if (path == null)
            {
                return -1;
            }

            // Convert windows paths into linux form
            path = path.Replace("\\", "/", StringComparison.InvariantCulture);
            var entry = Prices.FirstOrDefault(x => Regex.IsMatch(path, x.RegEx));
            return entry == null ? -1 : entry.Price;
        }
    }

    /// <summary>
    /// Prices entry for Data/Prices response
    /// </summary>
    public class PriceEntry
    {
        /// <summary>
        /// Vendor for this price
        /// </summary>
        [JsonProperty(PropertyName = "vendorName")]
        public string Vendor { get; set; }

        /// <summary>
        /// Regex for this data price entry
        /// Trims regex open, close, and multiline flag
        /// because it won't match otherwise
        /// </summary>
        public string RegEx
        {
            get => RawRegEx.TrimStart('/').TrimEnd('m').TrimEnd('/');
            set => RawRegEx = value;
        }

        /// <summary>
        /// RegEx directly form response
        /// </summary>
        [JsonProperty(PropertyName = "regex")]
        public string RawRegEx { get; set; }

        /// <summary>
        /// The requested price
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public int Price { get; set; }
    }
}