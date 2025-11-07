using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PokemonGo.Runtime.BattleFieldSystems
{
    public class PlayerInBattleFieldController : MonoBehaviour
    {
        [SerializeField] PlayerInBattleFieldView _view;
        [SerializeField] GameObject _ballPrefab;
        [SerializeField] float _shootVelocity = 5f;
        [SerializeField] float _ballOffsetZ = 0.2f;
        Transform _standbyBall;
        bool _isInteracting;

        [SerializeField] InputActionReference _clickInputAction;
        [SerializeField] InputActionReference _pointInputAction;
        Vector2 _cachedTapPosition;
        [SerializeField] float _minSwipeSpeed = 500f;
        Camera _camera;


        private void Start()
        {
            _camera = Camera.main;
            _view.onLeaveButtonClicked += OnLeaveButtonClicked;
        }

        private void OnEnable()
        {
            _clickInputAction.action.performed += OnClickPerformed;
            _pointInputAction.action.performed += OnPointPerformed;
        }

        private void OnDisable()
        {
            _clickInputAction.action.performed -= OnClickPerformed;
            _pointInputAction.action.performed -= OnPointPerformed;
        }

        private void OnLeaveButtonClicked()
        {
            SceneManager.LoadScene("Map");
        }

        void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                if (_isInteracting)
                    return;

                _isInteracting = true;
                _standbyBall = Instantiate(_ballPrefab).transform;
                Rigidbody rigidbody = _standbyBall.GetComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                PlaceBall();
            }
            else
            {
                if (_isInteracting == false)
                    return;

                _isInteracting = false;
                Rigidbody rigidbody = _standbyBall.GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                Vector3 direction = (_camera.transform.forward + _camera.transform.up).normalized;
                rigidbody.AddForce(direction * _shootVelocity, ForceMode.VelocityChange);
                _standbyBall = null;
            }
        }

        void OnPointPerformed(InputAction.CallbackContext context)
        {
            _cachedTapPosition = context.ReadValue<Vector2>();
            PlaceBall();
        }

        void PlaceBall()
        {
            if (_standbyBall == null)
                return;

            Ray ray = _camera.ScreenPointToRay(_cachedTapPosition);
            _standbyBall.position = ray.GetPoint(_ballOffsetZ);
            _standbyBall.rotation = Quaternion.LookRotation(ray.direction);
        }
    }
}