﻿#region Licensing
// ---------------------------------------------------------------------
// <copyright file="JungleClear.cs" company="EloBuddy">
// 
// Marksman Master
// Copyright (C) 2016 by gero
// All rights reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
// <summary>
// 
// Email: geroelobuddy@gmail.com
// PayPal: geroelobuddy@gmail.com
// </summary>
// ---------------------------------------------------------------------
#endregion
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace Marksman_Master.Plugins.Jhin.Modes
{
    internal class JungleClear : Jhin
    {
        public static void Execute()
        {
            var jungleMinions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Player.Instance.GetAutoAttackRange()).ToList();

            if (!jungleMinions.Any())
                return;

            if (Q.IsReady() && jungleMinions.Count > 1 && Settings.LaneClear.UseQInJungleClear &&
                Player.Instance.ManaPercent >= Settings.LaneClear.MinManaQ)
            {
                var minion = jungleMinions.OrderBy(unit => unit.Health).FirstOrDefault();
                if (minion != null)
                {
                    Q.Cast(minion);
                }
            }

            if (!W.IsReady() || jungleMinions.Count <= 1 || !Settings.LaneClear.UseWInJungleClear ||
                !(Player.Instance.ManaPercent >= Settings.LaneClear.MinManaW))
                return;

            var farmLocation = EntityManager.MinionsAndMonsters.GetLineFarmLocation(jungleMinions, 40, 2500, Player.Instance.Position.To2D());
            if (farmLocation.HitNumber > 1)
            {
                W.Cast(farmLocation.CastPosition);
            }
        }
    }
}
