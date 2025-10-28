using System;
using System.Collections;
using UnityEngine;

namespace BeatSaber.Runtime.InGame
{
    public class NoteDissolveEffector : MonoBehaviour
    {
        readonly int DISSOLVE_ID = Shader.PropertyToID("_Dissolve");

        [SerializeField] float _duration = 2f;
        MaterialPropertyBlock _propertyBlock; // 머티리얼 인스턴스 새로 만들지않고 특정 프로퍼티 값만 덮어쓰기위함.


        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void Play(Renderer renderer, bool destoryOnFinish = true)
        {
            StartCoroutine(C_Dissolve(renderer, destoryOnFinish));
        }

        IEnumerator C_Dissolve(Renderer renderer, bool destroyOnFinish)
        {
            float animationSpeed = 1f / _duration;
            float dissolve = 0f;
            float elapsedTime = 0f;

            while (dissolve < 1f)
            {
                dissolve = elapsedTime * animationSpeed;
                elapsedTime += Time.deltaTime;
                renderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat(DISSOLVE_ID, dissolve);
                renderer.SetPropertyBlock(_propertyBlock);

                // renderer.sharedMaterial.SetFloat(DISSOLVE_ID, dissolve);  // 전체가 공유하기때문에 하나바꾸면 다바뀐거 적용
                // renderer.material.SetFloat(DISSOLVE_ID, dissolve); // 머티리얼 인스턴스 한개 더 만들어서 프로퍼티값 바꿈.. (메모리 낭비)
                yield return null;
            }

            if (destroyOnFinish)
                Destroy(renderer.gameObject);
        }
    }
}