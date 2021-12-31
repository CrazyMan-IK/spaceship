using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astetrio.Spaceship.Building;

namespace Astetrio.Spaceship.Models
{
    [CreateAssetMenu(fileName = "New ConnectorTypes", menuName = "Spaceship/Connector Types", order = 50)]
    public class ConnectorTypes : ScriptableObject
    {
        //[Serializable]
        //private sealed class ConnectorTypesDictionary : SerializedDictionary<Vector3, Connector> { }
        [Serializable]
        private sealed class ConnectorTypesDictionary : SerializableDictionary<int, Mesh> { }

        [SerializeField] private Mesh _default = null;
        [SerializeField] private ConnectorTypesDictionary _types = new ConnectorTypesDictionary();

        public Mesh Default => _default;
        public IReadOnlyDictionary<int, Mesh> Types => _types;
    }
}
