using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Astetrio.Spaceship
{
    [RequireComponent(typeof(MeshRenderer))]
    public class EffectTest : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.5f;

        private MeshRenderer _renderer = null;
        private bool _isReversed = false;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                _isReversed = !_isReversed;

                /*_renderer.material.DOFloat(0, "_Offset", 1 / _duration).From(1).SetSpeedBased();*///.SetEase(Ease.OutCubic);
            }
            //return;
            /*if (Input.GetKeyDown(KeyCode.G))
            {
                _renderer.material.DOFloat(1, "_Offset", 1 / _duration)*//*.From(0)*//*.SetSpeedBased().SetEase(Ease.OutCubic);
            }*/

            var isReversed = _isReversed ? -1 : 1;

            var value = _renderer.material.GetFloat("_Offset");
            value = Mathf.MoveTowards(value, value + isReversed, Time.deltaTime * 1 / _duration);
            value = Mathf.Clamp01(value);
            _renderer.material.SetFloat("_Offset", value);
            //_renderer.material.DOFloat(0, "", 0).SetSpeedBased();
            //.DOFloat(0, "_Offset", 1 / _duration)/*.From(1)*/.SetSpeedBased().SetEase(Ease.OutCubic);
        }
    }
}
