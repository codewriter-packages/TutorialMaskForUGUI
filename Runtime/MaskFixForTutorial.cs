using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace CodeWriter.UIExtensions
{
    [RequireComponent(typeof(Mask))]
    public class MaskFixForTutorial : UIBehaviour, IMaterialModifier
    {
        [NonSerialized]
        private Graphic _graphic;

        [NonSerialized]
        private Mask _mask;

        [NonSerialized]
        private Material _maskMaterial;

        [NonSerialized]
        private Material _unmaskMaterial;

        public Graphic Graphic => _graphic ?? (_graphic = GetComponent<Graphic>());
        public Mask Mask => _mask ?? (_mask = GetComponent<Mask>());

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!Mask.MaskEnabled())
            {
                return baseMaterial;
            }

            var rootSortCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
            var stencilDepth = MaskUtilities.GetStencilDepth(transform, rootSortCanvas);
            if (stencilDepth >= 7)
            {
                Debug.LogWarning("Attempting to use a stencil mask with depth > 7", gameObject);
                return baseMaterial;
            }

            int desiredStencilBit = 1 << stencilDepth;

            // if we are at the first level...
            // we want to destroy what is there
            if (desiredStencilBit == 1)
            {
                var maskMaterial = StencilMaterial.Add(baseMaterial, 1, StencilOp.Replace, CompareFunction.Always,
                    Mask.showMaskGraphic ? ColorWriteMask.All : 0, 255, desiredStencilBit);
                StencilMaterial.Remove(_maskMaterial);
                _maskMaterial = maskMaterial;

                var unmaskMaterial = StencilMaterial.Add(baseMaterial, 1, StencilOp.Zero, CompareFunction.Always, 0,
                    255, desiredStencilBit);
                StencilMaterial.Remove(_unmaskMaterial);
                _unmaskMaterial = unmaskMaterial;
                Graphic.canvasRenderer.popMaterialCount = 1;
                Graphic.canvasRenderer.SetPopMaterial(_unmaskMaterial, 0);

                return _maskMaterial;
            }

            //otherwise we need to be a bit smarter and set some read / write masks
            var maskMaterial2 = StencilMaterial.Add(baseMaterial, desiredStencilBit | (desiredStencilBit - 1),
                StencilOp.Replace, CompareFunction.Equal, Mask.showMaskGraphic ? ColorWriteMask.All : 0,
                desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
            StencilMaterial.Remove(_maskMaterial);
            _maskMaterial = maskMaterial2;

            Graphic.canvasRenderer.hasPopInstruction = true;
            var unmaskMaterial2 = StencilMaterial.Add(baseMaterial, desiredStencilBit - 1, StencilOp.Replace,
                CompareFunction.Equal, 0, desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
            StencilMaterial.Remove(_unmaskMaterial);
            _unmaskMaterial = unmaskMaterial2;
            Graphic.canvasRenderer.popMaterialCount = 1;
            Graphic.canvasRenderer.SetPopMaterial(_unmaskMaterial, 0);

            return _maskMaterial;
        }
    }
}