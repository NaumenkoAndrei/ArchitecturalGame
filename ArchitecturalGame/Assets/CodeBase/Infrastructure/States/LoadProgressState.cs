﻿using CodeBase.Data;
using CodeBase.Services.PersistentProgress;
using CodeBase.Services.SaveLoad;

namespace CodeBase.Infrastructure.States
{
	public class LoadProgressState : IState
	{
		private readonly GameStateMachine _gameStateMachine;
		private readonly IPersistentProgressService _progressService;
		private readonly ISaveLoadService _saveLoadService;

		public LoadProgressState(GameStateMachine gameStateMachine, IPersistentProgressService progressService, ISaveLoadService saveLoadService)
		{
			_gameStateMachine = gameStateMachine;
			_progressService = progressService;
			_saveLoadService = saveLoadService;
		}

		public void Enter()
		{
			LoadProgressOrInitNew();

			_gameStateMachine.Enter<LoadLevelState, string>(_progressService.Progress.WorldData.PositionOnLevel.Level);
		}

		public void Exit()
		{
		}

		private void LoadProgressOrInitNew() =>
			_progressService.Progress = _saveLoadService.LoadProgress() ?? NewProgress();

		private PlayerProgress NewProgress()
		{
			PlayerProgress progress = new PlayerProgress(initialLevel: "Main");

			progress.HeroState.MaxHp = 50;
			progress.HeroStats.Damage = 1;
			progress.HeroStats.DamageRadius = 0.5f;
			progress.HeroState.ResetHP();

			return progress;
		}
	}
}
