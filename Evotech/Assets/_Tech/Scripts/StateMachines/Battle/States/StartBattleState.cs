using Core.Data;
using Hexnav.Core;
using UnityEngine;
using Core.Factories;
using Core.Cameras;
using Constants;
using Extensions;
using Core.Battle;
using Core.Units;

namespace Core.StateMachines.Battle
{
    public class StartBattleState : IState
    {
        private readonly BattleStateMachine _battleSM;
        private readonly IUnitsFactory _unitsFactory;
        private readonly IMapDataProvider _mapDataProvider;
        private readonly CameraController _camera;
        private readonly BattleObserver _battleObserver;
        private Transform _playerNodeToLookup;

        public StartBattleState(
            BattleStateMachine battleSM, IUnitsFactory unitsFactory, IMapDataProvider mapDataProvider,
            CameraController camera, BattleObserver battleObserver)
        {
            _battleSM = battleSM;
            _unitsFactory = unitsFactory;
            _mapDataProvider = mapDataProvider;
            _camera = camera;
            _battleObserver = battleObserver;
        }

        public void Enter()
        {
            CreateUnits();
            _camera.SetInstanLookupPoint(_playerNodeToLookup.position.FlatY());
            _battleSM.Enter<WaitingForTurnState>();
        }

        public void Exit() { }

        private void CreateUnits()
        {
            foreach (NodeBase node in _mapDataProvider.GetNodes())
            {
                if (node != null && node.Tags != null && node.Tags.Count != 0)
                {
                    foreach (string tag in node.Tags)
                    {
                        ResolveTag(tag, node);
                    }
                }
            }
        }

        private void ResolveTag(string tag, NodeBase node)
        {
            if (tag == StringConstants.PLAYER_MAP_TAG)
            {
                BaseUnit unit = _unitsFactory.Create(node, Enums.UnitType.Player);
                _battleObserver.AddUnit(unit);
                _playerNodeToLookup = node.WorldObject;
                node.NonwalkableFactors++;
            }
            else if (tag == StringConstants.DEFAULT_ENEMY_MAP_TAG)
            {
                BaseUnit unit = _unitsFactory.Create(node, Enums.UnitType.EnemyRed);
                _battleObserver.AddUnit(unit);
                node.NonwalkableFactors++;
            }
        }
    }
}