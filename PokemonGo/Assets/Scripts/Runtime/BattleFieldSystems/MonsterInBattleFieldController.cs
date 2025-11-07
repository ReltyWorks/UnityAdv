using PokemonGo.Persistence.Entities;
using PokemonGo.Persistence.Repositories;
using UnityEngine;
using VContainer;

namespace PokemonGo.Runtime.BattleFieldSystems
{
    public class MonsterInBattleFieldController : MonoBehaviour
    {
        public int hp { get; private set; }

        [SerializeField] MonsterInBattleFieldView _view;
        [SerializeField] MonsterSpecLibrary _specLibrary;

        [Header("Spawning settings")]
        [SerializeField] float _spawnInnerRadius = 1f;
        [SerializeField] float _spawnOuterRadius = 3f;
        MonsterSpec _spec;
        float _height;

        int _monsterBallLayer;
        Transform _camera;

        [Inject] IItemRepository _itemRepository;

        private void Awake()
        {
            _spec = _specLibrary.GetRandom();
            hp = _spec.hpMax;
            GameObject monster = Instantiate(_spec.prefab, transform);
            CalcHeight(monster.GetComponent<Collider>());
            _camera = Camera.main.transform;
            _monsterBallLayer = LayerMask.NameToLayer("MonsterBall");
        }

        private void Start()
        {
            _view.InitializeHpBar(_spec.hpMax);
            _view.UpdateHpBar(hp);
            transform.position = RandomPointInAnnulus(_spawnInnerRadius, _spawnOuterRadius);
        }

        private void Update()
        {
            _view.ShowStatus(transform.position + Vector3.up * _height,
                             Quaternion.LookRotation(_camera.forward, _camera.up));
        }

        public void GetDamage(int amount)
        {
            hp -= amount;
            hp = Mathf.Clamp(hp, 0, hp);
            _view.UpdateHpBar(hp);

            if (hp == 0)
            {
                Destroy(transform.GetChild(0).gameObject);
                GetComponent<Rigidbody>().isKinematic = true;
                _view.HideStatus();
                _itemRepository.InsertItem(new Item(_spec.id, 1));
                _itemRepository.Save();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == _monsterBallLayer)
            {
                GetDamage(20);
            }
        }

        void CalcHeight(Collider collider)
        {
            if (collider is SphereCollider sphere)
            {
                _height = sphere.radius * 2f;
            }
            else if (collider is CapsuleCollider capsule)
            {
                _height = capsule.radius + capsule.height;
            }
            else
            {
                _height = 1f;
            }
        }

        /// <summary>
        /// 도넛형태의 바깥 링 내 균일 분포 랜덤 좌표 계산 함수
        /// </summary>
        /// <param name="r1"> inner </param>
        /// <param name="r2"> outer </param>
        Vector3 RandomPointInAnnulus(float r1, float r2)
        {
            float u = Random.value;
            float theta = Random.Range(0f, Mathf.PI * 2f);
            float r = Mathf.Sqrt(u * (r2 * r2 - r1 * r1) + r1 * r1); // 면적 기준 균일분포
            //r = Mathf.Lerp(r1, r2, u); // 반지름 기준 균일분포
            return _camera.position + new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta)) * r;
        }
    }
}
