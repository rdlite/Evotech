using Core.Data;
using System;

namespace Core.Units
{
    public class HumanoidUnit : BaseUnit
    {
        public event Action OnUnitClicked;

        private UnitRaycastTrigger _raycastTrigger;
        private Spirit _spirit;
        private WeaponStyle _weaponStyle;

        public override void Init()
        {
            base.Init();

            _raycastTrigger = GetComponentInChildren<UnitRaycastTrigger>();
            _raycastTrigger.OnClicked += () => OnUnitClicked?.Invoke();

            _weaponStyle = _stylesContainer.GetStyleOfWeaponType(_unitSettings.WeaponID);

            _spirit = GetComponentInChildren<Spirit>();
            _spirit.Init(_weaponStyle.AnimatorID);
            _spirit.CreateWeapon(_weaponStyle.WeaponPrefab);
        }
    }
}