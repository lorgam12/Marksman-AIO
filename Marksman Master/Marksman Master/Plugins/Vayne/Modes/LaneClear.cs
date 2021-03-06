﻿#region Licensing
// ---------------------------------------------------------------------
// <copyright file="LaneClear.cs" company="EloBuddy">
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
using Marksman_Master.Utils;

namespace Marksman_Master.Plugins.Vayne.Modes
{
    internal class LaneClear : Vayne
    {
        public static bool CanILaneClear()
        {
            return !Settings.LaneClear.EnableIfNoEnemies || Player.Instance.CountEnemiesInRange(Settings.LaneClear.ScanRange) <= Settings.LaneClear.AllowedEnemies;
        }

        public static void Execute()
        {
            var laneMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position,
    Player.Instance.GetAutoAttackRange() + 250).ToList();

            if (!laneMinions.Any() || !CanILaneClear())
                return;

            if (!Q.IsReady() || !IsPostAttack || !Settings.LaneClear.UseQToLaneClear ||
                !(Player.Instance.ManaPercent >= Settings.LaneClear.MinMana))
                return;

            var minion =
                EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                    x =>
                        x.IsValidTarget(Player.Instance.GetAutoAttackRange()) && x.Health > Player.Instance.GetAutoAttackDamage(x, true) &&
                        x.Health <
                        Player.Instance.GetAutoAttackDamage(x, true) +
                        Player.Instance.TotalAttackDamage*Damage.QBonusDamage[Q.Level] && Prediction.Health.GetPrediction(x, 500) > Player.Instance.GetAutoAttackDamage(x, true)).OrderBy(x=>x.Distance(Player.Instance));

            if (!minion.Any())
                return;

            if (Player.Instance.Position.Extend(Game.CursorPos, 299)
                .IsInRange(minion.First(), Player.Instance.GetAutoAttackRange()))
            {
                Q.Cast(Player.Instance.Position.Extend(Game.CursorPos, 285).To3D());
                return;
            }

            var pos = SafeSpotFinder.PointsInRange(Player.Instance.Position.To2D(), 900, 100).Where(x=> EntityManager.Heroes.Enemies.Any(e=>e.IsInRange(x, e.GetAutoAttackRange())) == false).ToList();

            if (!pos.Any())
                return;

            var poss = Misc.SortVectorsByDistance(pos, minion.First().Position.To2D())[0];

            Q.Cast(poss.Distance(Player.Instance) > Q.Range ? Player.Instance.Position.Extend(poss, Q.Range).To3D() : poss.To3D());
        }
    }
}