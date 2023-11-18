using Utils;
using Qnject;
using UnityEngine;
using NaughtyAttributes;

namespace Core.Battle.Weapon
{
    public class WeaponEffectsSetter : MonoBehaviour
    {
        [SerializeField] private bool _useSlashTrail;
        [ShowIf(nameof(_useSlashTrail)), SerializeField] private ParticleSystem _slashTrail;

        private IUpdateProvider _updateProvider;
        private Transform _defaultSlashTrailParent;
        private Vector3 _localSlashPos;
        private Vector3 _localSlashScale;
        private bool _isSlashTrailActive;
        private bool _isSlashTrailExists;
        private float _slashTrailExistingTimer;

        [Inject]
        private void Construct(IUpdateProvider updateProvider)
        {
            _updateProvider = updateProvider;
            _updateProvider?.AddUpdate(Tick);

            if (_useSlashTrail)
            {
                _defaultSlashTrailParent = _slashTrail.transform.parent;
                _slashTrail.gameObject.SetActive(false);
                _localSlashPos = _slashTrail.transform.localPosition;
                _localSlashScale = _slashTrail.transform.localScale;
            }
        }

        private void OnEnable()
        {
            _updateProvider?.AddUpdate(Tick);
        }

        private void OnDisable()
        {
            _updateProvider?.RemoveUpdate(Tick);
        }

        private void Tick()
        {
            if (!_isSlashTrailActive && _isSlashTrailExists)
            {
                _slashTrailExistingTimer -= Time.deltaTime;

                if (_slashTrailExistingTimer <= 0f)
                {
                    _isSlashTrailExists = false;
                    _slashTrail.gameObject.SetActive(false);
                    _slashTrail.transform.SetParent(_defaultSlashTrailParent);
                    _slashTrail.transform.localPosition = _localSlashPos;
                    _slashTrail.transform.localRotation = Quaternion.identity;
                    _slashTrail.transform.localScale = _localSlashScale;
                }
            }
        }

        public void ActivateSlashTrailEffect()
        {
            if (!_useSlashTrail) return;

            _isSlashTrailExists = true;
            _isSlashTrailActive = true;
            _slashTrail.gameObject.SetActive(true);
        }

        public void DeactivateSlashTrailEffect()
        {
            if (!_useSlashTrail) return;

            _isSlashTrailActive = false;
            _slashTrailExistingTimer = 1f;
            _slashTrail.transform.SetParent(null);
            _slashTrail.transform.localScale = _localSlashScale;
        }
    }
}