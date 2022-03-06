using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;
using Astetrio.Spaceship.Interfaces;

namespace Astetrio.Spaceship
{
    public class SimpleStarsGenerator : MonoBehaviour, IStarsGenerator
    {
        [SerializeField] private Transform _target = null;
        [SerializeField] private Star _starPrefab = null;
        [SerializeField] private Transform _starsRoot = null;
        [SerializeField] private LayerMask _starsLayerMask = default;

        private readonly List<Star> _stars = new List<Star>();

        private void Awake()
        {
            _starPrefab.enabled = false;

            StartGeneration(Vector3Int.zero, 0);
        }

        public void StartGeneration(Vector3Int position, int additionalSeed)
        {
            StartCoroutine(Generate(position, additionalSeed));
        }

        private IEnumerator Generate(Vector3Int position, int additionalSeed)
        {
            int hashCode = -1119599109;
            hashCode = hashCode * -1521134295 + position.GetHashCode();
            hashCode = hashCode * -1521134295 + additionalSeed.GetHashCode();

            URandom.InitState(hashCode);

            var tries = 0;
            for (int i = 0; i < 500; i++)
            {
                var direction = URandom.insideUnitSphere;
                //var targetPosition = direction.normalized * 49152 + direction * 73728; //GOOD
                var targetPosition = direction.normalized * 65536 + direction * 57344; //GOOD
                //var targetPosition = direction * 24576;
                //var targetPosition = direction.normalized * 9216 + direction * 9216;

                // 2048
                // 3072
                // 4096
                // 5120
                // 6144
                // 8192
                // 9216
                // 10240
                // 12288
                // 16384
                // 20480
                // 24576
                // 32768
                // 40960
                // 49152
                // 57344
                // 65536
                // 81920
                // 98304

                Physics.SyncTransforms();
                if (Physics.CheckSphere(targetPosition, 16384, _starsLayerMask, QueryTriggerInteraction.Collide))
                {
                    tries++;
                    i--;

                    if (tries > 1000)
                    {
                        Debug.Log("Error");

                        break;
                    }

                    //yield return null;
                    continue;
                }

                var star = Instantiate(_starPrefab, targetPosition, Quaternion.identity, _starsRoot);
                star.Initialize(_target);

                _stars.Add(star);

                tries = 0;

                yield return null;
            }

            /*UnityEditor.EditorApplication.isPaused = true;

            yield return null;
            yield return null;*/

            foreach (var star in _stars)
            {
                star.enabled = true;
            }
        }
    }
}
