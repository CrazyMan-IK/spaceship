using UnityEngine;
using UnityEngine.SceneManagement;

namespace Astetrio.Spaceship
{
    /*public class FloatingOrigin : MonoBehaviour
    {
        [Tooltip("Point of reference from which to check the distance to origin.")]
        public Transform ReferenceObject = null;

        [Tooltip("Distance from the origin the reference object must be in order to trigger an origin shift.")]
        public float Threshold = 5000f;

        [Header("Options")]
        [Tooltip("When true, origin shifts are considered only from the horizontal distance to orign.")]
        public bool Use2DDistance = false;

        [Tooltip("When true, updates ALL open scenes. When false, updates only the active scene.")]
        public bool UpdateAllScenes = true;

        [Tooltip("Should ParticleSystems be moved with an origin shift.")]
        public bool UpdateParticles = true;

        [Tooltip("Should TrailRenderers be moved with an origin shift.")]
        public bool UpdateTrailRenderers = true;

        [Tooltip("Should LineRenderers be moved with an origin shift.")]
        public bool UpdateLineRenderers = true;

        private ParticleSystem.Particle[] parts = null;

        void LateUpdate()
        {
            if (ReferenceObject == null)
                return;

            Vector3 referencePosition = ReferenceObject.position;

            if (Use2DDistance)
                referencePosition.y = 0f;

            if (referencePosition.magnitude > Threshold)
            {
                MoveRootTransforms(referencePosition);

                if (UpdateParticles)
                    MoveParticles(referencePosition);

                if (UpdateTrailRenderers)
                    MoveTrailRenderers(referencePosition);

                if (UpdateLineRenderers)
                    MoveLineRenderers(referencePosition);
            }
        }

        private void MoveRootTransforms(Vector3 offset)
        {
            if (UpdateAllScenes)
            {
                for (int z = 0; z < SceneManager.sceneCount; z++)
                {
                    foreach (GameObject g in SceneManager.GetSceneAt(z).GetRootGameObjects())
                        g.transform.position -= offset;
                }
            }
            else
            {
                foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
                    g.transform.position -= offset;
            }
        }

        private void MoveTrailRenderers(Vector3 offset)
        {
            var trails = FindObjectsOfType<TrailRenderer>() as TrailRenderer[];
            foreach (var trail in trails)
            {
                Vector3[] positions = new Vector3[trail.positionCount];

                int positionCount = trail.GetPositions(positions);
                for (int i = 0; i < positionCount; ++i)
                    positions[i] -= offset;

                trail.SetPositions(positions);
            }
        }

        private void MoveLineRenderers(Vector3 offset)
        {
            var lines = FindObjectsOfType<LineRenderer>() as LineRenderer[];
            foreach (var line in lines)
            {
                Vector3[] positions = new Vector3[line.positionCount];

                int positionCount = line.GetPositions(positions);
                for (int i = 0; i < positionCount; ++i)
                    positions[i] -= offset;

                line.SetPositions(positions);
            }
        }

        private void MoveParticles(Vector3 offset)
        {
            var particles = FindObjectsOfType<ParticleSystem>() as ParticleSystem[];
            foreach (ParticleSystem system in particles)
            {
                if (system.main.simulationSpace != ParticleSystemSimulationSpace.World)
                    continue;

                int particlesNeeded = system.main.maxParticles;

                if (particlesNeeded <= 0)
                    continue;

                // ensure a sufficiently large array in which to store the particles
                if (parts == null || parts.Length < particlesNeeded)
                {
                    parts = new ParticleSystem.Particle[particlesNeeded];
                }

                // now get the particles
                int num = system.GetParticles(parts);

                for (int i = 0; i < num; i++)
                {
                    parts[i].position -= offset;
                }

                system.SetParticles(parts, num);
            }
        }
    }*/

    public class FloatingOrigin : MonoBehaviour
    {
        public static FloatingOrigin Instance;

        // Largest value allowed for the main camera's X or Z coordinate before that
        // coordinate is moved by the same amount towards 0 (which updates offset).
        // Pick a power of two for this, as floating point precision (the thing
        // we are trying to regulate) decreases with every successive power of two.
        public const float ThresholdValue = (float)Threshold._4;

        private ParticleSystem.Particle[] _parts = null;
        private Transform _anchor;

        // The origin is offset by offset * threshold
        public (byte x, byte y, byte z) Offset { get; private set; } = (0, 0, 0);

        public enum Threshold
        {
            _2 = 2,
            _4 = 4,
            _8 = 8,
            _16 = 16,
            _32 = 32,
            _64 = 64,
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024
        }

        public void OnEnable()
        {
            // Ensure singleton
            if (Instance != null)
            {
                Destroy(gameObject);
                throw new System.Exception(
                    "More than one instance of singleton detected."
                );
            }
            else
            {
                Instance = this;
            }
        }

        public void LateUpdate()
        {
            if (_anchor == null)
            {
                var camera = Camera.main;
                if (camera != null)
                {
                    _anchor = camera.transform;
                }
                else
                {
                    return;
                }
            }

            // Calculate offset

            Vector3 offsetToApply;
            float value;

            if (Mathf.Abs(_anchor.position.x) > ThresholdValue)
            {
                value = _anchor.position.x;
                offsetToApply = new Vector3(1f, 0f, 0f);
            }
            else if (Mathf.Abs(_anchor.position.y) > ThresholdValue)
            {
                value = _anchor.position.y;
                offsetToApply = new Vector3(0f, 1f, 0f);
            }
            else if (Mathf.Abs(_anchor.position.z) > ThresholdValue)
            {
                value = _anchor.position.z;
                offsetToApply = new Vector3(0f, 0f, 1f);
            }
            else
            {
                return;
            }

            float times = Mathf.Floor(Mathf.Abs(value) / ThresholdValue);
            float offsetSign = Mathf.Sign(value) * -1f;

            Offset = (
                (byte)(Offset.x + (offsetToApply.x * times * offsetSign)),
                (byte)(Offset.y + (offsetToApply.y * times * offsetSign)),
                (byte)(Offset.z + (offsetToApply.z * times * offsetSign))
            );

            float delta = ThresholdValue * times * offsetSign;
            offsetToApply *= delta;

            // Offset scene root objects

            GameObject[] objects = UnityEngine.SceneManagement.SceneManager
                .GetActiveScene().GetRootGameObjects();

            foreach (var o in objects)
            {
                Transform t = o.GetComponent<Transform>();
                t.position += offsetToApply;
            }

            // Offset world-space particles

            ParticleSystem[] particleSystems = FindObjectsOfType<ParticleSystem>();
            foreach (var sys in particleSystems)
            {
                if (sys.main.simulationSpace != ParticleSystemSimulationSpace.World)
                    continue;

                int particlesNeeded = sys.main.maxParticles;

                if (particlesNeeded <= 0)
                    continue;

                bool wasPaused = sys.isPaused;
                bool wasPlaying = sys.isPlaying;

                if (!wasPaused)
                    sys.Pause();

                // Ensure a sufficiently large array in which to store the particles
                if (_parts == null || _parts.Length < particlesNeeded)
                {
                    _parts = new ParticleSystem.Particle[particlesNeeded];
                }

                // Now get the particles
                int num = sys.GetParticles(_parts);

                for (int i = 0; i < num; i++)
                {
                    _parts[i].position += offsetToApply;
                }

                sys.SetParticles(_parts, num);

                if (wasPlaying)
                    sys.Play();
            }

            Star[] stars = FindObjectsOfType<Star>();
            foreach (var star in stars)
            {
                star.AddBaseOffset(offsetToApply);
            }
        }
    }
}
