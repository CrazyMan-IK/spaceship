using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;
using Astetrio.Spaceship.Interfaces;

namespace Astetrio.Spaceship
{
    public class SimpleObjectsGenerator : MonoBehaviour
    {
        [SerializeField] private Transform _target = null;
        [SerializeField] private List<Transform> _objectsPrefabs = null;
        [SerializeField] private Transform _objectsRoot = null;
        [SerializeField] private LayerMask _objectsLayerMask = default;

        private readonly List<Transform> _objects = new List<Transform>();

        private void Awake()
        {
            StartGeneration(Vector3Int.zero, 15);
        }

        private void OnEnable()
        {
            FloatingOrigin.Instance.Shifted += OnFloatingOriginShifted;
        }

        private void OnDisable()
        {
            FloatingOrigin.Instance.Shifted -= OnFloatingOriginShifted;
        }

        public void StartGeneration(Vector3Int position, int additionalSeed)
        {
            StartCoroutine(GenerateMultiple(position, additionalSeed));
        }

        private IEnumerator GenerateMultiple(Vector3Int position, int additionalSeed)
        {
            var oldObjects = _objects.ToList();
            foreach (var obj in oldObjects)
            {
                if (Vector3.Distance(obj.position, _target.position) > 1024)
                {
                    Destroy(obj.gameObject);
                    _objects.Remove(obj);
                }
            }

            yield return null;

            yield return StartCoroutine(Generate(Vector3.zero, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.forward * 1024, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.back * 1024, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.left * 1024, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.right * 1024, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.up * 1024, position, additionalSeed));
            yield return StartCoroutine(Generate(Vector3.down * 1024, position, additionalSeed));

            /*UnityEditor.EditorApplication.isPaused = true;

            yield return null;
            yield return null;*/
        }

        private IEnumerator Generate(Vector3 offset, Vector3Int position, int additionalSeed)
        {
            int hashCode = -1119599109;
            hashCode = hashCode * -1521134295 + position.GetHashCode();
            hashCode = hashCode * -1521134295 + additionalSeed.GetHashCode();

            URandom.InitState(hashCode);

            var tries = 0;
            for (int i = 0; i < 256; i++)
            {
                var direction = URandom.insideUnitSphere;
                var targetPosition = offset + direction * 1024;
                var prefab = _objectsPrefabs[URandom.Range(0, _objectsPrefabs.Count)];

                if (Physics.CheckSphere(targetPosition, 1, _objectsLayerMask, QueryTriggerInteraction.Collide))
                {
                    tries++;
                    i--;

                    if (tries > 10000)
                    {
                        Debug.Log("Error");

                        break;
                    }
                    
                    continue;
                }

                var obj = Instantiate(prefab, targetPosition, URandom.rotation, _objectsRoot);

                _objects.Add(obj);

                tries = 0;

                //yield return null;

                Physics.SyncTransforms();
            }
            
            yield return null;
        }

        private void OnFloatingOriginShifted()
        {
            StartGeneration(Vector3Int.zero, 15);
        }
    }
}
