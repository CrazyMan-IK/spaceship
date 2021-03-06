using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AYellowpaper;
using Astetrio.Spaceship.Interfaces;

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

    [DefaultExecutionOrder(-999)]
    public class FloatingOrigin : MonoBehaviour
    {
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
            _1024 = 1024,
            _2048 = 2048,
            _4096 = 4096
        }

        public static FloatingOrigin Instance { get; private set; }

        // Largest value allowed for the main camera's X or Z coordinate before that
        // coordinate is moved by the same amount towards 0 (which updates offset).
        // Pick a power of two for this, as floating point precision (the thing
        // we are trying to regulate) decreases with every successive power of two.
        [SerializeField] private List<Star> _baseStars = new List<Star>();
        [SerializeField] private InterfaceReference<IStarsGenerator> _starsGenerator = null;
        public const float ThresholdValue = (float)Threshold._4096;

        public event Action Shifted = null;

        private ParticleSystem.Particle[] _parts = null;
        private Transform _anchor;

        // The origin is offset by offset * threshold
        public (byte x, byte y, byte z) Offset { get; private set; } = (0, 0, 0);

        public void OnEnable()
        {
            // Ensure singleton
            if (Instance != null)
            {
                Destroy(gameObject);
                throw new Exception("More than one instance of singleton detected.");
            }

            Instance = this;
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

            Shifted?.Invoke();

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

            GameObject[] objects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var o in objects)
            {
                if (o.TryGetComponent(out FloatingOriginTarget _))
                {
                    o.transform.position += offsetToApply;

                    continue;
                }
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

            foreach (var star in _baseStars)
            {
                star.AddBaseOffset(offsetToApply);
            }

            foreach (var star in _starsGenerator.Value.Stars)
            {
                star.AddBaseOffset(offsetToApply);
            }
        }
    }
}
