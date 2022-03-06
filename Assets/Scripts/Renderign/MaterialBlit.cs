using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace Astetrio.Spaceship.Rendering
{
    public class MaterialBlit : ScriptableRendererFeature
    {
        [SerializeField] private Material _material = null;

        private ScriptableRenderPass _pass = null;

        public override void Create()
        {
            _pass = new FinalBlitPass(RenderPassEvent.AfterRenderingOpaques, _material);

            //_pass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_pass);
        }
    }
}
