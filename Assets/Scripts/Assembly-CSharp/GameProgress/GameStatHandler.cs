using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
	internal class GameStatHandler : BaseGameProgressHandler
	{
		private const int ExpPerKill = 10;

		private const int ExpPerLevelBase = 500;

		private const float ExpPerLevelMultiplier = 1.2f;

		private const int MaxLevel = 20;

		private List<int> _expPerLevel = new List<int>();

		private GameStatContainer _gameStat;

		public GameStatHandler(GameStatContainer gameStat)
		{
			_gameStat = gameStat;
			_expPerLevel.Add(500);
			for (int i = 1; i < 20; i++)
			{
				_expPerLevel.Add((int)((float)_expPerLevel[i - 1] * 1.2f));
			}
		}

		public int GetExpToNext()
		{
			if (_gameStat.Level.Value >= 20)
			{
				return 0;
			}
			return _expPerLevel[_gameStat.Level.Value];
		}

		public void AddExp(int exp)
		{
			_gameStat.Exp.Value += exp;
			CheckLevelUp();
		}

		private void CheckLevelUp()
		{
			if (_gameStat.Level.Value >= 20)
			{
				_gameStat.Exp.Value = 0;
				_gameStat.Level.Value = 20;
			}
			else if (_gameStat.Exp.Value > 0 && _gameStat.Exp.Value >= _expPerLevel[_gameStat.Level.Value])
			{
				_gameStat.Level.Value++;
				_gameStat.Exp.Value -= _expPerLevel[_gameStat.Level.Value];
				_gameStat.Exp.Value = Math.Max(_gameStat.Exp.Value, 0);
				CheckLevelUp();
			}
		}

		public override void RegisterTitanKill(GameObject character, TITAN victim, KillWeapon weapon)
		{
			switch (weapon)
			{
			case KillWeapon.Blade:
				_gameStat.TitansKilledBlade.Value++;
				break;
			case KillWeapon.Gun:
				_gameStat.TitansKilledGun.Value++;
				break;
			case KillWeapon.ThunderSpear:
				_gameStat.TitansKilledThunderSpear.Value++;
				break;
			default:
				_gameStat.TitansKilledOther.Value++;
				break;
			}
			_gameStat.TitansKilledTotal.Value++;
			AddExp(10);
		}

		public override void RegisterHumanKill(GameObject character, HERO victim, KillWeapon weapon)
		{
			switch (weapon)
			{
			case KillWeapon.Blade:
				_gameStat.HumansKilledBlade.Value++;
				break;
			case KillWeapon.Gun:
				_gameStat.HumansKilledGun.Value++;
				break;
			case KillWeapon.ThunderSpear:
				_gameStat.HumansKilledThunderSpear.Value++;
				break;
			case KillWeapon.Titan:
				_gameStat.HumansKilledTitan.Value++;
				break;
			default:
				_gameStat.HumansKilledOther.Value++;
				break;
			}
			_gameStat.HumansKilledTotal.Value++;
			AddExp(10);
		}

		public override void RegisterDamage(GameObject character, GameObject victim, KillWeapon weapon, int damage)
		{
			if (weapon == KillWeapon.Blade || weapon == KillWeapon.Gun)
			{
				_gameStat.DamageHighestOverall.Value = Math.Max(_gameStat.DamageHighestOverall.Value, damage);
				_gameStat.DamageTotalOverall.Value += damage;
				switch (weapon)
				{
				case KillWeapon.Blade:
					_gameStat.DamageHighestBlade.Value = Math.Max(_gameStat.DamageHighestBlade.Value, damage);
					_gameStat.DamageTotalBlade.Value += damage;
					break;
				case KillWeapon.Gun:
					_gameStat.DamageHighestGun.Value = Math.Max(_gameStat.DamageHighestGun.Value, damage);
					_gameStat.DamageTotalGun.Value += damage;
					break;
				}
			}
		}

		public override void RegisterSpeed(GameObject character, float speed)
		{
			_gameStat.HighestSpeed.Value = Mathf.Max(_gameStat.HighestSpeed.Value, speed);
		}

		public override void RegisterInteraction(GameObject character, GameObject interact, InteractionType type)
		{
		}
	}
}
