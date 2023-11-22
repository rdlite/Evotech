using Qnject;
using Core.Data;
using Extensions;
using Hexnav.Core;
using UnityEngine;
using Utils.Decal;
using System.Collections.Generic;

namespace Core.Battle
{
    public class WalkFieldVisualizer : IWalkFieldVisualizer
    {
        private List<DecalData> _walkFieldDecals = new List<DecalData>();
        private List<NodeBase> _currentPath = new List<NodeBase>();
        private AssetsContainer _assetsContainer;
        private GameObject _selectionHexFX;

        public WalkFieldVisualizer(AssetsContainer assetsContainer)
        {
            _assetsContainer = assetsContainer;
        }

        public void Show(NodeBase startNode, List<NodeBase> nodes)
        {
            CreateList(startNode, nodes);
        }

        public void Hide()
        {
            for (int i = 0; i < _walkFieldDecals.Count; i++)
            {
                int id = i;
                DecalWrapper decalWrapper = _walkFieldDecals[id].DecalWrapper;
                decalWrapper.Hide(.3f, _walkFieldDecals[id].Delay, DG.Tweening.Ease.OutSine, () => Object.Destroy(decalWrapper.gameObject));
            }

            SelectionFX().SetActive(false);
            _walkFieldDecals.Clear();
            _currentPath.Clear();
        }

        private void CreateList(NodeBase startNode, List<NodeBase> nodes)
        {
            Hide();

            _walkFieldDecals = new List<DecalData>();

            foreach (var node in nodes)
            {
                DecalData newDecal = new DecalData();
                newDecal.Delay = Vector3.Distance(node.WorldPos.FlatY(), startNode.WorldPos.FlatY()) / 40f;
                newDecal.Node = node;
                newDecal.DecalWrapper = QnjectPrefabsFactory.Instantiate(_assetsContainer.BattlePrefabs.HexagonalDefaultDecal);
                newDecal.DecalWrapper.transform.position = node.WorldPos + node.SurfaceOffset + Vector3.up * .05f;
                newDecal.DecalWrapper.SetMinScale();
                newDecal.DecalWrapper.Show(.3f, newDecal.Delay, DG.Tweening.Ease.OutBack);
                _walkFieldDecals.Add(newDecal);
            }
        }

        public void ProcessPathScale(List<NodeBase> path)
        {
            if (path == null)
            {
                if (SelectionFX().activeSelf)
                {
                    for (int i = 0; i < _walkFieldDecals.Count; i++)
                    {
                        _walkFieldDecals[i].DecalWrapper.SetMinScale();
                    }

                    SelectionFX().SetActive(false);
                }

                return;
            }

            for (int i = 0; i < _walkFieldDecals.Count; i++)
            {
                _walkFieldDecals[i].DecalWrapper.SetMinScale();
            }

            if (path.Count > 1)
            {
                for (int j = 0; j < path.Count - 1; j++)
                {
                    for (int i = 0; i < _walkFieldDecals.Count; i++)
                    {
                        if (_walkFieldDecals[i].Node == path[j] && path.Count > 1)
                        {
                            _walkFieldDecals[i].DecalWrapper.SetMediumScale();
                        }
                        else if (_walkFieldDecals[i].Node == path[path.Count - 1])
                        {
                            _walkFieldDecals[i].DecalWrapper.SetMaxScale();
                        }
                    }
                }
            }
            else if (path.Count == 1)
            {
                for (int i = 0; i < _walkFieldDecals.Count; i++)
                {
                    if (_walkFieldDecals[i].Node == path[0])
                    {
                        _walkFieldDecals[i].DecalWrapper.SetMaxScale();
                    }
                }
            }

            if (path.Count > 0)
            {
                if (!SelectionFX().activeSelf)
                {
                    SelectionFX().SetActive(true);
                }

                SelectionFX().transform.position = path[path.Count - 1].WorldPos;
            }
            else
            {
                if (SelectionFX().activeSelf)
                {
                    SelectionFX().SetActive(false);
                }
            }
        }

        private class DecalData
        {
            public DecalWrapper DecalWrapper;
            public NodeBase Node;
            public float Delay;
        }

        private GameObject SelectionFX()
        {
            if (_selectionHexFX == null)
            {
                _selectionHexFX = Object.Instantiate(_assetsContainer.BattlePrefabs.SelectionHexagon);
            }

            return _selectionHexFX;
        }
    }

    public interface IWalkFieldVisualizer
    {
        public void Show(NodeBase startNode, List<NodeBase> nodes);
        public void Hide();
        public void ProcessPathScale(List<NodeBase> path);
    }
}