using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace CodeWriter.UIExtensions
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("CodeWriter/UIExtensions/Tutorial Object")]
    public class TutorialObject : UIBehaviour, IMaterialModifier
    {
        [SerializeField]
        private bool showGraphics = true;

        [NonSerialized]
        private Graphic _graphic;

        [NonSerialized]
        private Material _highlightMaterial;

        public Graphic Graphic => _graphic ?? (_graphic = GetComponent<Graphic>());

        protected override void OnEnable()
        {
            base.OnEnable();

            if (Graphic != null)
            {
                Graphic.SetMaterialDirty();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (Graphic != null)
            {
                Graphic.SetMaterialDirty();
            }

            StencilMaterial.Remove(_highlightMaterial);
            _highlightMaterial = null;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!IsActive())
            {
                return;
            }

            if (Graphic != null)
            {
                Graphic.SetMaterialDirty();
            }
        }
#endif

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!IsActive())
            {
                return baseMaterial;
            }

            const int tutorialBit = 1 << 7;

            if (Graphic is MaskableGraphic maskableGraphic && maskableGraphic.maskable)
            {
                var rootCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
                var stencilValue = MaskUtilities.GetStencilDepth(transform, rootCanvas);

                if (stencilValue > 0)
                {
                    var highlightMat = StencilMaterial.Add(baseMaterial,
                        stencilID: tutorialBit | ((1 << stencilValue) - 1),
                        operation: StencilOp.Replace,
                        compareFunction: CompareFunction.Equal,
                        colorWriteMask: showGraphics ? ColorWriteMask.All : 0,
                        readMask: (1 << stencilValue) - 1,
                        writeMask: tutorialBit
                    );
                    StencilMaterial.Remove(_highlightMaterial);
                    _highlightMaterial = highlightMat;

                    return _highlightMaterial;
                }
            }

            {
                var highlightMat = StencilMaterial.Add(baseMaterial,
                    stencilID: tutorialBit,
                    operation: StencilOp.Replace,
                    compareFunction: CompareFunction.Always,
                    colorWriteMask: showGraphics ? ColorWriteMask.All : 0,
                    readMask: 0,
                    writeMask: tutorialBit
                );
                StencilMaterial.Remove(_highlightMaterial);
                _highlightMaterial = highlightMat;
            }

            return _highlightMaterial;
        }
    }
}