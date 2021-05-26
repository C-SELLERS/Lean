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
using QuantConnect.Securities;

namespace QuantConnect.Orders.Fees
{
    /// <summary>
    /// Provides an implementation of <see cref="FeeModel"/> that models Bitfinex order fees.
    /// Currently we do not model 30-day volume, so we use the first tier fee model by default.
    /// </summary>
    public class BitfinexFeeModel : FeeModel
    {
        /// <summary>
        /// Tier 1 maker fees: 0.1%
        /// Maker fees are paid when you add liquidity to our order book by placing a limit order under the ticker price for buy and above the ticker price for sell.
        /// https://www.bitfinex.com/fees
        /// </summary>
        public const decimal MakerFee = 0.001m;
        /// <summary>
        /// Tier 1 taker fees: 0.2%
        /// Taker fees are paid when you remove liquidity from our order book by placing any order that is executed against an order of the order book.
        /// Note: If you place a hidden order, you will always pay the taker fee. If you place a limit order that hits a hidden order, you will always pay the maker fee.
        /// https://www.bitfinex.com/fees
        /// </summary>
        public const decimal TakerFee = 0.002m;

        /// <summary>
        /// Get the fee for this order in quote currency
        /// </summary>
        /// <param name="parameters">A <see cref="OrderFeeParameters"/> object
        /// containing the security and order</param>
        /// <returns>The cost of the order in quote currency</returns>
        public override OrderFee GetOrderFee(OrderFeeParameters parameters)
        {
            var order = parameters.Order;
            var security = parameters.Security;
            decimal feePercentage = TakerFee;
            var props = order.Properties as BitfinexOrderProperties;

            if (order.Type == OrderType.Limit &&
                props?.Hidden != true &&
                (props?.PostOnly == true || !order.IsMarketable))
            {
                // limit order posted to the order book
                feePercentage = MakerFee;
            }

            // get order value in quote currency
            var unitPrice = order.Direction == OrderDirection.Buy ? security.AskPrice : security.BidPrice;
            if (order.Type == OrderType.Limit)
            {
                // limit order posted to the order book
                unitPrice = ((LimitOrder)order).LimitPrice;
            }

            unitPrice *= security.SymbolProperties.ContractMultiplier;

            // Determine fee and round decimal places to lotsize
            var fee = unitPrice * order.AbsoluteQuantity * feePercentage;
            fee -= fee % security.SymbolProperties.LotSize;
            return new OrderFee(new CashAmount(
                fee,
                security.QuoteCurrency.Symbol));
        }
    }
}
