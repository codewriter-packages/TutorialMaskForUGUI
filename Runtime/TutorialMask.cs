using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace CodeWriter.UIExtensions
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("CodeWriter/UIExtensions/Tutorial Mask")]
    public class TutorialMask : UIBehaviour, IMaterialModifier
    {
        [NonSerialized]
        private Material _maskMaterial;

        protected override void OnDisable()
        {
            base.OnDisable();

            StencilMaterial.Remove(_maskMaterial);
            _maskMaterial = null;
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!isActiveAndEnabled)
            {
                return baseMaterial;
            }

            const int tutorialBit = 1 << 7;

            var maskMat = StencilMaterial.Add(baseMaterial,
                stencilID: tutorialBit,
                operation: StencilOp.Keep,
                compareFunction: CompareFunction.NotEqual,
                colorWriteMask: ColorWriteMask.All,
                readMask: tutorialBit,
                writeMask: 0
            );
            StencilMaterial.Remove(_maskMaterial);
            _maskMaterial = maskMat;

            return maskMat;
        }
    }
}