using System;
using Core.Data;
using Core.Particles;
using System.Collections.Generic;

namespace Core.Units
{
    public class HumanoidUnit : BaseUnit
    {
        public event Action OnUnitClicked;

        private List<ThreeDObjectExploder> _armor;
        private UnitRaycastTrigger _raycastTrigger;
        private Spirit _spirit;
        private WeaponStyle _weaponStyle;

        public override void Init(Enums.UnitType unitType)
        {
            base.Init(unitType);

            _raycastTrigger = GetComponentInChildren<UnitRaycastTrigger>();
            _raycastTrigger.OnClicked += () => OnUnitClicked?.Invoke();

            _weaponStyle = _stylesContainer.GetStyleOfWeaponType(_unitSettings.WeaponID);

            _spirit = GetComponentInChildren<Spirit>();
            _spirit.Init(_weaponStyle.AnimatorID, unitType);
            _spirit.CreateWeapon(_weaponStyle.WeaponPrefab);

            _armor = _spirit.CreateArmor();
        }
    }
}