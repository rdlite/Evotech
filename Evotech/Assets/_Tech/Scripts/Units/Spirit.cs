using Qnject;
using Core.Data;
using Extensions;
using UnityEngine;
using System.Collections.Generic;
using Core.Particles;

namespace Core.Units
{
    public class Spirit : MonoBehaviour
    {
        [SerializeField] private List<ArmorPoint> _points;
        [SerializeField] private Renderer _mainSkinRenderer;

        private OneWeaponLeftHandPoint _oneWeaponLeftHandPoint;
        private SpiritAnimator _animator;
        private GameObject _weapon;
        private StylesContainer _stylesContainer;
        private AssetsContainer _assetsContainer;

        [Inject]
        private void Construct(
            StylesContainer stylesContainer, AssetsContainer assetsContainer)
        {
            _stylesContainer = stylesContainer;
            _assetsContainer = assetsContainer;
        }

        public void Init(int animatorWeaponID, Enums.UnitType unitType)
        {
            _animator = GetComponentInChildren<SpiritAnimator>();
            _animator.Init(animatorWeaponID);

            _oneWeaponLeftHandPoint = GetComponentInChildren<OneWeaponLeftHandPoint>();

            UnitSpiritStyle spiritStyle = _stylesContainer.GetStyleOfUnitSpirit(unitType);
            _mainSkinRenderer.material.SetColor("_MainColor", spiritStyle.UnitColor);
            _mainSkinRenderer.material.SetColor("_WavesColor", spiritStyle.WavesColor);
        }

        public void CreateWeapon(GameObject weapon)
        {
            if (_weapon != null)
            {
                Destroy(_weapon);
            }

            _weapon = QnjectPrefabsFactory.CreatePrefab(weapon);
            _weapon.transform.SetParent(_oneWeaponLeftHandPoint.transform);
            _weapon.transform.ResetLocals();
        }

        public List<ThreeDObjectExploder> CreateArmor()
        {
            List<ThreeDObjectExploder> objects = new List<ThreeDObjectExploder>();

            for (int i = 0; i < _points.Count; i++)
            {
                ThreeDObjectExploder obj = QnjectPrefabsFactory.CreatePrefab(_assetsContainer.ArmorPrebafs.GetRandomObjectByType(_points[i].PointType));
                obj.transform.SetParent(_points[i].Point);
                obj.transform.ResetLocals();
                objects.Add(obj);
            }

            return objects;
        }
    }

    [System.Serializable]
    public class ArmorPoint
    {
        public Enums.SpiritArmorPointType PointType;
        public Transform Point;
    }
}