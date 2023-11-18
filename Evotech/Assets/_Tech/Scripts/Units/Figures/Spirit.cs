using Qnject;
using Core.Data;
using Extensions;
using UnityEngine;
using Core.Particles;
using Core.Battle.Weapon;
using System.Collections.Generic;

namespace Core.Units
{
    public class Spirit : BaseFigure
    {
        [SerializeField] private List<ArmorPoint> _points;

        private OneWeaponLeftHandPoint _oneWeaponLeftHandPoint;
        private GameObject _weapon;
        private StylesContainer _stylesContainer;
        private AssetsContainer _assetsContainer;
        private WeaponEffectsSetter _weaponEffectsSetter;
        private BaseUnit _baseUnit;

        private int MAIN_COLOR_HASH = Shader.PropertyToID("_MainColor");
        private int MAIN_ALPHA_HASH = Shader.PropertyToID("_MainAlpha");

        [Inject]
        private void Construct(
            StylesContainer stylesContainer, AssetsContainer assetsContainer)
        {
            _stylesContainer = stylesContainer;
            _assetsContainer = assetsContainer;
        }

        public void Init(
            WeaponStyle weaponStyle, Enums.UnitType unitType, BaseUnit baseUnit)
        {
            _baseUnit = baseUnit;

            _animator = GetComponentInChildren<SpiritAnimator>();
            _animator.Init(weaponStyle);

            _oneWeaponLeftHandPoint = GetComponentInChildren<OneWeaponLeftHandPoint>();

            UnitSpiritStyle spiritStyle = _stylesContainer.GetStyleOfUnitSpirit(unitType);
            _mainSkinRenderer.material.SetColor("_MainColor", spiritStyle.UnitColor);
            _mainSkinRenderer.material.SetColor("_WavesColor", spiritStyle.WavesColor);

            if (_weaponEffectsSetter != null)
            {
                _baseUnit.GetEventsCatcher().OnActivateSlashEffect += _weaponEffectsSetter.ActivateSlashTrailEffect;
                _baseUnit.GetEventsCatcher().OnDeactivateSlashEffect += _weaponEffectsSetter.DeactivateSlashTrailEffect;
            }
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
            _weaponEffectsSetter = _weapon.GetComponent<WeaponEffectsSetter>();
        }

        public void SetDead()
        {
            _animator.PlayDead();
            _mainSkinRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        public void SetSpiritTransparency(float t)
        {
            _mainSkinRenderer.material.SetFloat(MAIN_ALPHA_HASH, Mathf.Clamp01(t));
        }

        public void PlayDeathParticle()
        {
            ParticleSystem deathParticle = Instantiate(_assetsContainer.UnitAssets.SpiritEvaporationParticle);
            var mainModule = deathParticle.main;
            var shapeModule = deathParticle.shape;

            mainModule.startColor = _mainSkinRenderer.material.GetColor(MAIN_COLOR_HASH).SetAlpha(mainModule.startColor.color.a);
            shapeModule.skinnedMeshRenderer = _mainSkinRenderer as SkinnedMeshRenderer;

            deathParticle.Play();
        }

        public void DestroyWeapon()
        {
            _weapon.transform.SetParent(null);
            _weapon.GetComponentInChildren<ThreeDObjectExploder>().SmoothGravityFalling(true, 5f);
        }

        private void OnDisable()
        {
            if (_weaponEffectsSetter != null)
            {
                _baseUnit.GetEventsCatcher().OnActivateSlashEffect += _weaponEffectsSetter.ActivateSlashTrailEffect;
                _baseUnit.GetEventsCatcher().OnDeactivateSlashEffect += _weaponEffectsSetter.DeactivateSlashTrailEffect;
            }
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