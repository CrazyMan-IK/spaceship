using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship.Building
{
    public class BuildTool : MonoBehaviour
    {
        [SerializeField] private SpaceShip _spaceShip = null;
        [SerializeField] private Ghost _ghost = null;
        [SerializeField] private LayerMask _ghostMask = default;

        private Vector3 _ghostBaseEulerAngles = Vector3.zero;
        private Vector3 _screenHalfSize = Vector2.zero;

        private void Awake()
        {
            _ghostBaseEulerAngles = _ghost.transform.eulerAngles;
            _screenHalfSize = new Vector2(Screen.width / 2, Screen.height / 2);
        }

        private void Update()
        {
            var ray = Camera.main.ScreenPointToRay(_screenHalfSize + Vector3.forward);
            
            if (Physics.Raycast(ray, out var info, 5, ~_ghostMask))
            {
                if (info.collider.TryGetComponent(out ConnectionPoint point))
                {
                    Vector3 normalPoint;
                    Quaternion rotation = Quaternion.identity;

                    _ghost.transform.eulerAngles = new Vector3(_ghostBaseEulerAngles.x, 0, _ghostBaseEulerAngles.z);
                    _ghost.transform.rotation = _spaceShip.transform.rotation * _ghost.transform.rotation;

                    if (_ghost.Block.CanAutoRotate && IsDirectionInOnePlane(point.transform.up, _ghost.transform.forward, _spaceShip.transform.up, out rotation))
                    {
                        var reflectedDirection = Vector3.Reflect(ray.direction, info.normal);
                        var reflectionAngle = Vector3.Angle(ray.direction, reflectedDirection);

                        if (reflectionAngle >= 90 && point.transform.forward != _ghost.transform.up && point.transform.forward != -_ghost.transform.up && (info.normal == point.transform.up || info.normal == -point.transform.up))
                        {
                            normalPoint = Vector3.Scale(info.collider.bounds.extents, info.normal);
                            rotation *= Quaternion.AngleAxis(90, Vector3.up);
                        }
                        else
                        {
                            normalPoint = Vector3.Scale(info.collider.bounds.extents, point.transform.forward);
                        }
                    }
                    else if (_ghost.Block.CanAutoRotate && IsDirectionInOnePlane(point.transform.forward, _ghost.transform.forward, _spaceShip.transform.up, out rotation))
                    {
                        var reflectedDirection = Vector3.Reflect(ray.direction, info.normal);
                        var reflectionAngle = Vector3.Angle(ray.direction, reflectedDirection);

                        if (reflectionAngle >= 90 && point.transform.forward != _ghost.transform.up && point.transform.forward != -_ghost.transform.up && (info.normal == point.transform.up || info.normal == -point.transform.up))
                        {
                            normalPoint = Vector3.Scale(info.collider.bounds.extents, info.normal);
                        }
                        else
                        {
                            normalPoint = info.point - point.transform.position;
                            normalPoint = Quaternion.Inverse(point.transform.rotation) * normalPoint;
                            normalPoint.x = 0;
                            normalPoint.z = 0;
                            normalPoint = point.transform.rotation * normalPoint;
                        }
                    }
                    else if (!_ghost.Block.CanAutoRotate && point.transform.parent.forward == _ghost.transform.forward)
                    {
                        normalPoint = Vector3.Scale(info.collider.bounds.extents, point.transform.forward);
                    }
                    else if (!_ghost.Block.CanAutoRotate && point.transform.forward == _ghost.transform.forward)
                    {
                        normalPoint = info.point - point.transform.position;
                        normalPoint = Quaternion.Inverse(point.transform.rotation) * normalPoint;
                        normalPoint.x = 0;
                        normalPoint = point.transform.rotation * normalPoint;
                    }
                    else
                    {
                        return;
                    }

                    _ghost.transform.rotation *= rotation;

                    var closestPoint = _ghost.Block.GetClosestPoint(-normalPoint);
                    _ghost.transform.position = point.transform.position - _ghost.transform.localRotation * closestPoint.transform.localPosition;

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

                if (pointUp == tempRotation * ghostForward)
                {
                    rotation = Quaternion.AngleAxis(90 * i, Vector3.up);
                    return true;
                }
            }

            rotation = Quaternion.identity;
            return false;
        }
    }
}
