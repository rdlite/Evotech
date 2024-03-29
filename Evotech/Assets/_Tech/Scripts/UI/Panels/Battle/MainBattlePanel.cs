using Core.Battle;
using Core.UI.Elements;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class MainBattlePanel : Panel
    {
        [SerializeField] private Button _unitStatsAllSwitcher;
        [SerializeField] private Image _switchStatsIcon;
        [SerializeField] private UnitsSequencePanel _unitsSequencePanel;
        [SerializeField] private float _deactivatedStatsIconAlpha = .5f;

        private BattleObserver _battleObserver;

        protected override void Awake()
        {
            base.Awake();
            _switchStatsIcon.SetAlpha(_deactivatedStatsIconAlpha);
        }

        public override void Show()
        {
            base.Show();
        }

        public void InitBattleObserver(BattleObserver battleObserver)
        {
            _battleObserver = battleObserver;
        }

        private void OnEnable()
        {
            _unitStatsAllSwitcher.onClick.AddListener(SwitchBannersButtonPressed);
        }

        private void OnDisable()
        {
            _unitStatsAllSwitcher.onClick.RemoveListener(SwitchBannersButtonPressed);
        }

        private void SwitchBannersButtonPressed()
        {
            bool isActive = _battleObserver.SwitchStatsState();
            _switchStatsIcon.SetAlpha(isActive ? 1f : _deactivatedStatsIconAlpha);
        }

        public UnitsSequencePanel GetUnitsSequencePanel()
        {
            return _unitsSequencePanel;
        }

        public override void Freeze()
        {
            base.Freeze();
            _unitsSequencePanel.Freeze();
        }

        public override void Unfreeze()
        {
            base.Unfreeze();
            _unitsSequencePanel.Unfreeze();
        }
    }
}