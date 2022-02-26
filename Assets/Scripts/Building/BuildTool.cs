using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astetrio.Spaceship.Extensions;

namespace Astetrio.Spaceship.Building
{
    public class BuildTool : MonoBehaviour
    {
        [SerializeField] private SpaceShip _spaceShip = null;
        [SerializeField] private Ghost _ghost1 = null;
        [SerializeField] private Ghost _ghost2 = null;
        [SerializeField] private LayerMask _ignoreMask = default;
        [SerializeField] private LayerMask _ghostMask = default;

        private Vector3 _ghostBaseEulerAngles = Vector3.zero;
        private Vector3 _screenHalfSize = Vector2.zero;
        private Ghost _ghost = null;

        private void Awake()
        {
            _ghost2.gameObject.SetActive(false);
            _ghost = _ghost1;
            _ghost.gameObject.SetActive(true);

            _ghostBaseEulerAngles = _ghost.transform.eulerAngles;
            _screenHalfSize = new Vector2(Screen.width / 2, Screen.height / 2);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _ghost.transform.eulerAngles = _ghostBaseEulerAngles;

                _ghost.gameObject.SetActive(false);
                _ghost = _ghost1;
                _ghost.gameObject.SetActive(true);

                _ghostBaseEulerAngles = _ghost.transform.eulerAngles;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _ghost.transform.eulerAngles = _ghostBaseEulerAngles;

                _ghost.gameObject.SetActive(false);
                _ghost = _ghost2;
                _ghost.gameObject.SetActive(true);

                _ghostBaseEulerAngles = _ghost.transform.eulerAngles;
            }

            var ray = Camera.main.ScreenPointToRay(_screenHalfSize + Vector3.forward);
            
            if (Physics.Raycast(ray, out var info, 5, ~_ignoreMask))
            {
                if (info.collider.TryGetComponent(out ConnectionPoint point))
                {
                    Vector3 normalPoint;
                    Vector3 offset;
                    Quaternion rotation = Quaternion.identity;

                    _ghost.transform.eulerAngles = new Vector3(_ghostBaseEulerAngles.x, 0, _ghostBaseEulerAngles.z);
                    _ghost.transform.rotation = _spaceShip.transform.rotation * _ghost.transform.rotation;

                    if (_ghost.Block.CanAutoRotate && IsDirectionInOnePlane(point.transform.up, _ghost.transform.forward, _spaceShip.transform.up, out rotation))
                    {
                        var reflectedDirection = Vector3.Reflect(ray.direction, info.normal);
                        var reflectionAngle = Vector3.Angle(ray.direction, reflectedDirection);

                        //if (reflectionAngle >= 90 && point.transform.forward != _ghost.transform.up && point.transform.forward != -_ghost.transform.up && (info.normal == point.transform.up || info.normal == -point.transform.up))
                        if (reflectionAngle >= 90 && 
                            !Compare(point.transform.forward, _ghost.transform.up) && 
                            !Compare(point.transform.forward, -_ghost.transform.up) && 
                            (Compare(info.normal, point.transform.up) || 
                            Compare(info.normal, -point.transform.up)))
                        {
                            normalPoint = Vector3.Scale(info.collider.GetLocalBounds().extents, info.normal);
                            rotation *= Quaternion.AngleAxis(90, Vector3.up);

                            offset = point.transform.forward / 2;
                            offset += info.normal / 2;
                        }
                        else
                        {
                            normalPoint = Vector3.Scale(info.collider.GetLocalBounds().extents, point.transform.forward);

                            offset = point.transform.forward;
                        }
                    }
                    else if (_ghost.Block.CanAutoRotate && IsDirectionInOnePlane(point.transform.forward, _ghost.transform.forward, _spaceShip.transform.up, out rotation))
                    {
                        var reflectedDirection = Vector3.Reflect(ray.direction, info.normal);
                        var reflectionAngle = Vector3.Angle(ray.direction, reflectedDirection);

                        if (reflectionAngle >= 90 && 
                            !Compare(point.transform.forward, _ghost.transform.up) && 
                            !Compare(point.transform.forward, -_ghost.transform.up) && 
                            (Compare(info.normal, point.transform.up) || 
                            Compare(info.normal, -point.transform.up)))
                        {
                            normalPoint = Vector3.Scale(info.collider.GetLocalBounds().extents, info.normal);

                            offset = info.normal / 2;
                        }
                        else
                        {
                            normalPoint = info.point - point.transform.position;
                            normalPoint = Quaternion.Inverse(point.transform.rotation) * normalPoint;
                            normalPoint.x = 0;
                            normalPoint.z = 0;

                            offset = 0.5f * Mathf.Sign(normalPoint.y) * point.transform.up;
                            
                            normalPoint = point.transform.rotation * normalPoint;
                        }

                        offset += point.transform.forward / 2;
                    }
                    else if (!_ghost.Block.CanAutoRotate && Compare(point.transform.parent.forward, _ghost.transform.forward))
                    {
                        normalPoint = Vector3.Scale(info.collider.GetLocalBounds().extents, point.transform.localRotation * Vector3.forward);
                        normalPoint = _ghost.transform.localRotation * normalPoint;

                        offset = point.transform.forward;
                    }
                    else if (!_ghost.Block.CanAutoRotate && Compare(point.transform.forward, _ghost.transform.forward))
                    {
                        normalPoint = info.point - point.transform.position;
                        normalPoint = Quaternion.Inverse(point.transform.rotation) * normalPoint;
                        normalPoint.x = 0;

                        offset = 0.5f * Mathf.Sign(normalPoint.y) * point.transform.up;

                        normalPoint = point.transform.rotation * normalPoint;

                        offset += point.transform.forward * 0.5f;
                    }
                    else
                    {
                        _ghost.Hide();
                        return;
                    }

                    _ghost.transform.rotation *= rotation;

                    var closestPoint = _ghost.Block.GetClosestPoint(-normalPoint);
                    _ghost.transform.position = point.transform.position - _ghost.transform.localRotation * closestPoint.transform.localPosition + offset;

                    var local = _ghost.transform.localPosition;
                    local.x = (float)Math.Round(local.x, 1, MidpointRounding.AwayFromZero);
                    local.y = (float)Math.Round(local.y, 1, MidpointRounding.AwayFromZero);
                    local.z = (float)Math.Round(local.z, 1, MidpointRounding.AwayFromZero);

                    if (_ghost.IsCollideWithAnything(~_ghostMask))
                    {
                        _ghost.Hide();
                        return;
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        _ghost.Paste(_spaceShip.transform, ~_ghostMask);
                        return;
                    }

                    _ghost.Show();
                    return;
                }
            }

            _ghost.Hide();
        }

        private bool IsDirectionInOnePlane(Vector3 pointUp, Vector3 ghostForward, Vector3 upAxis, out Quaternion rotation)
        {
            for (int i = 0; i < 4; i++)
            {
                var tempRotation = Quaternion.AngleAxis(90 * i, upAxis);

                if (Compare(pointUp, tempRotation * ghostForward))
                {
                    rotation = Quaternion.AngleAxis(90 * i, Vector3.up);
                    return true;
                }
            }

            rotation = Quaternion.identity;
            return false;
        }

        private bool Compare(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b) < 0.00002;
        }
    }
}
