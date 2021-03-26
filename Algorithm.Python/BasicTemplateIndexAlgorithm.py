# QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
# Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License

from QuantConnect.Algorithm import *
from QuantConnect.Data import *
from QuantConnect.Indicators import *
from QuantConnect import *


class BasicTemplateIndexAlgorithm(QCAlgorithm):
    def Initialize(self) -> None:
        self.SetStartDate(2021, 1, 4)
        self.SetEndDate(2021, 1, 15)
        self.SetCash(1000000)

        # Use indicator for signal; but it cannot be traded
        self.spx = self.AddIndex("SPX", Resolution.Minute).Symbol
        self.emaSlow = self.EMA(self.spx, 80)
        self.emaFast = self.EMA(self.spx, 200)

        # Trade on SPY
        self.spy = self.AddEquity("SPY", Resolution.Minute).Symbol

    def OnData(self, data: Slice):
        if self.spx not in data.Bars or self.spy not in data.Bars:
            return

        if not self.emaSlow.IsReady:
            return

        if self.emaFast > self.emaSlow:
            self.SetHoldings(self.spy, 1)
        else:
            self.Liquidate()

    def OnEndOfAlgorithm(self) -> None:
        if self.Portfolio[self.spx].TotalSaleVolume > 0:
            raise Exception("Index is not tradable.")