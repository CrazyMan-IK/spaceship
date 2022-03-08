using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;
using Astetrio.Spaceship.Models;
using Astetrio.Spaceship.Interfaces;

namespace Astetrio.Spaceship.Items
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(BoxCollider))]
    public class Item : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IItemInformationPresenter> _itemInformation = null;
        [SerializeField] private MeshRenderer _lodRenderer = null;

        private MeshFilter _meshFilter = null;
        private MeshRenderer _meshRenderer = null;
        private MeshCollider _meshCollider = null;
        private BoxCollider _boxCollider = null;
        private IItemInformation _information = null;

        public IItemInformation Information => _information;
        public int Count { get; private set; }

        private void Awake()
        {
            Initialize(_itemInformation?.Value?.GetInformation(), 1);
        }

        public void Initialize(IItemInformation information, int count)
        {
            if (information == null)
            {
                return;
            }

            _information = information;

            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshCollider = GetComponent<MeshCollider>();
            _boxCollider = GetComponent<BoxCollider>();

            _meshFilter.sharedMesh = _information.Mesh;
            _meshRenderer.sharedMaterials = _information.Materials.ToArray();
            _lodRenderer.sharedMaterials = _meshRenderer.sharedMaterials;
            _meshCollider.sharedMesh = _information.Mesh;
            _boxCollider.center = _information.Mesh.bounds.center;
            _boxCollider.size = _information.Mesh.bounds.size * 1.5f;

            Count = count;
        }
    }
}
