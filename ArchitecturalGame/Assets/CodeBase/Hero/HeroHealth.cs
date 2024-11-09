using CodeBase.Data;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Logic;
using System;
using UnityEngine;

namespace CodeBase.Hero
{
	[RequireComponent(typeof(HeroAnimator))]
	public class HeroHealth : MonoBehaviour, ISavedProgress, IHealth
	{
		[SerializeField] private HeroAnimator _animator;

		public event Action HealthChanged;

		private State _state;

		public float Current
		{
			get => _state.CurrentHp;
			set
			{
				if(_state.CurrentHp != value)
				{
					_state.CurrentHp = value;
					HealthChanged?.Invoke();
				}
			}
		}

		public float Max
		{
			get => _state.MaxHp;
			set => _state.MaxHp = value;
		}

		public void LoadProgress(PlayerProgress progress)
		{
			_state = progress.HeroState;
			HealthChanged?.Invoke();
		}

		public void UpdateProgress(PlayerProgress progress)
		{
			progress.HeroState.CurrentHp = Current;
			progress.HeroState.MaxHp = Max;
		}

		public void TakeDamage(float damage)
		{
			if(Current <= 0)
				return;

			Current -= damage;
			_animator.PlayHit();
		}
	}
}