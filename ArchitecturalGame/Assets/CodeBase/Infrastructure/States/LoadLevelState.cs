﻿using CodeBase.CameraLogic;
using CodeBase.Enemy;
using CodeBase.Hero;
using CodeBase.Infrastructure.Factory;
using CodeBase.Logic;
using CodeBase.Services.PersistentProgress;
using CodeBase.Services.StaticData;
using CodeBase.StaticData;
using CodeBase.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure.States
{
	public class LoadLevelState : IPayloadedState<string>
	{
		private const string InitialPointTag = "InitialPoint";
		private const string EnemySpawnerTag = "EnemySpawner";

		private readonly GameStateMachine _stateMachine;
		private readonly SceneLoader _sceneLoader;
		private readonly LoadingCurtain _curtain;
		private readonly IGameFactory _gameFactory;
		private readonly IPersistentProgressService _progressService;
		private readonly IStaticDataService _staticData;

		public LoadLevelState(GameStateMachine stateMachine, SceneLoader sceneLoader, LoadingCurtain curtain, IGameFactory gameFactory, IPersistentProgressService progressService, IStaticDataService staticData)
		{
			_stateMachine = stateMachine;
			_sceneLoader = sceneLoader;
			_curtain = curtain;
			_gameFactory = gameFactory;
			_progressService = progressService;
			_staticData = staticData;
		}

		public void Enter(string sceneName)
		{
			_curtain.Show();
			_gameFactory.Cleanup();
			_sceneLoader.Load(sceneName, OnLoaded);
		}

		public void Exit() =>
			_curtain.Hide();

		private void OnLoaded()
		{
			InitGameWorld();
			InformProgressRiders();

			_stateMachine.Enter<GameLoopState>();
		}

		private void InformProgressRiders() =>
			_gameFactory.ProgressReaders.ForEach(x => x.LoadProgress(_progressService.Progress));

		private void InitGameWorld()
		{
			InitSpawners();
			InitLootPieces();
			GameObject hero = InitHero();
			InitHud(hero);
			CameraFollow(hero);
		}

		private void InitSpawners()
		{
			string sceneKey = SceneManager.GetActiveScene().name;
			LevelStaticData levelData = _staticData.ForLevel(sceneKey);

			foreach(EnemySpawnerData spawnerData in levelData.EnemySpawners)
				_gameFactory.CreateSpawner(spawnerData.Position, spawnerData.Id, spawnerData.MonsterTypeId);
		}

		private void InitLootPieces()
		{
			foreach(string key in _progressService.Progress.WorldData.LootData.LootPiecesOnScene.Dictionary.Keys)
			{
				LootPiece lootPiece = _gameFactory.CreateLoot();
				lootPiece.GetComponent<UniqueId>().Id = key;
			}
		}

		private GameObject InitHero() =>
			_gameFactory.CreateHero(at: GameObject.FindWithTag(InitialPointTag));

		private void InitHud(GameObject hero)
		{
			GameObject hud = _gameFactory.CreateHud();

			hud.GetComponentInChildren<ActorUI>().Construct(hero.GetComponent<HeroHealth>());
		}

		private void CameraFollow(GameObject hero) =>
			Camera.main.GetComponent<CameraFollow>().Follow(hero);
	}
}
