﻿#region Licensing
// ---------------------------------------------------------------------
// <copyright file="Combo.cs" company="EloBuddy">
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

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Marksman_Master.Utils;

namespace Marksman_Master.Plugins.Sivir.Modes
{
    internal class Combo : Sivir
    {
        public static void Execute()
        {
            if (Q.IsReady() && Settings.Combo.UseQ)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                if (target != null && !target.HasUndyingBuffA() && !target.HasSpellShield())
                {
                    var qPrediction = Q.GetPrediction(target);

                    if(qPrediction.HitChance >= HitChance.Medium && target.TotalHealthWithShields() < Player.Instance.GetAutoAttackDamage(target, true) * 2 + Player.Instance.GetSpellDamage(target, SpellSlot.Q))
                    {
                        Q.Cast(qPrediction.CastPosition);
                    }
                    else if (qPrediction.HitChancePercent >= 65 && (Player.Instance.Mana - 60 > (R.IsReady() ? 100 : 0)))
                    {
                        Q.Cast(qPrediction.CastPosition);
                    }
                }
            }

            if (!W.IsReady() || !Settings.Combo.UseW || !IsPostAttack)
                return;
            {
                var target = TargetSelector.GetTarget(Player.Instance.GetAutoAttackRange(), DamageType.Physical);

                if (target != null && target.HealthPercent < 25 && target.Health - IncomingDamage.GetIncomingDamage(target) > 30 &&
                    target.Health - IncomingDamage.GetIncomingDamage(target) <
                    Player.Instance.GetAutoAttackDamage(target, true))
                {
                    W.Cast();
                } else if (target != null && target.Distance(Player.Instance) < Player.Instance.GetAutoAttackRange() - 50)
                {
                    W.Cast();
                }
            }
        }
    }
}