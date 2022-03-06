using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Astetrio.Spaceship
{
    [RequireComponent(typeof(Camera))]
    public class CameraFogDisabler : MonoBehaviour
    {
        private Camera _camera = null;
        private bool _isFogEnabled = false;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _isFogEnabled = RenderSettings.fog;
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnPreCameraRender;
            RenderPipelineManager.endCameraRendering += OnPostCameraRender;
        }

        private void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= OnPostCameraRender;
            RenderPipelineManager.beginCameraRendering -= OnPreCameraRender;
        }

        private void OnPreCameraRender(ScriptableRenderContext context, Camera camera)
        {
            if (camera == _camera)
            {
                RenderSettings.fog = false;
            }
        }

        private void OnPostCameraRender(ScriptableRenderContext context, Camera camera)
        {
            if (camera == _camera)
            {
                RenderSettings.fog = _isFogEnabled;
            }
        }
    }
}
