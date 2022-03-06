using System.Linq;
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

            StartGeneration(Vector3Int.zero, 5);
        }

        public void StartGeneration(Vector3Int position, int additionalSeed)
        {
            /*StartCoroutine(Generate(Vector3.zero, position, additionalSeed));
            StartCoroutine(Generate(Vector3.forward * 131072, position, additionalSeed));
            StartCoroutine(Generate(Vector3.back * 131072, position, additionalSeed));
            StartCoroutine(Generate(Vector3.left * 131072, position, additionalSeed));
            StartCoroutine(Generate(Vector3.right * 131072, position, additionalSeed));
            StartCoroutine(Generate(Vector3.up * 131072, position, additionalSeed));
            StartCoroutine(Generate(Vector3.down * 131072, position, additionalSeed));*/

            /*var enumerable = ToIEnumerable(Generate(Vector3.zero, position, additionalSeed)).Cast<YieldInstruction>()
                .Concat(ToIEnumerable(Generate(Vector3.forward * 131072, position, additionalSeed)).Cast<YieldInstruction>())
                .Concat(ToIEnumerable(Generate(Vector3.back * 131072, position, additionalSeed)).Cast<YieldInstruction>())
                .Concat(ToIEnumerable(Generate(Vector3.left * 131072, position, additionalSeed)).Cast<YieldInstruction>())
                .Concat(ToIEnumerable(Generate(Vector3.right * 131072, position, additionalSeed)).Cast<YieldInstruction>())
                .Concat(ToIEnumerable(Generate(Vector3.up * 131072, position, additionalSeed)).Cast<YieldInstruction>())
                .Concat(ToIEnumerable(Generate(Vector3.down * 131072, position, additionalSeed)).Cast<YieldInstruction>())
                .Concat(ToIEnumerable(ActivateStars()).Cast<YieldInstruction>());

            StartCoroutine(enumerable.GetEnumerator());*/

            StartCoroutine(GenerateMultiple(position, additionalSeed));
        }

        /*private static IEnumerable ToIEnumerable(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }*/

        /*private IEnumerator ActivateStars()
        {
            *//*UnityEditor.EditorApplication.isPaused = true;

            yield return null;
            yield return null;*//*

            foreach (var star in _stars)
            {
                star.enabled = true;
            }

            yield return null;
        }*/

        private IEnumerator GenerateMultiple(Vector3Int position, int additionalSeed)
        {
            yield return StartCoroutine(Generate(Vector3.zero, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.forward * 131072, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.back * 131072, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.left * 131072, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.right * 131072, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.up * 131072, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.down * 131072, position, additionalSeed));

            /*UnityEditor.EditorApplication.isPaused = true;

            yield return null;
            yield return null;*/

            foreach (var star in _stars)
            {
                star.enabled = true;
            }
        }

        private IEnumerator Generate(Vector3 offset, Vector3Int position, int additionalSeed)
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
                var targetPosition = offset + direction.normalized * 65536 + direction * 65536; //GOOD
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

                if (Physics.CheckSphere(targetPosition, 32768, _starsLayerMask, QueryTriggerInteraction.Collide))
                {
                    tries++;
                    i--;

                    if (tries > 10000)
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

                Physics.SyncTransforms();
            }
        }
    }
}
