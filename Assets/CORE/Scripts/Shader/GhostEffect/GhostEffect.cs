// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class GhostEffect : MonoBehaviour
    {
        [SerializeField, Space]
        bool enableOutline = true;

        [SerializeField, Space]
        Color color = Color.white;

        SpriteRenderer spriteRenderer;

        [HideInInspector]
        public SpriteRenderer SpriteRenderer
        {
            get
            {
                if (spriteRenderer == null)
                {
                    spriteRenderer = GetComponent<SpriteRenderer>();
                }
                return spriteRenderer;
            }
        }

        Color disabledColor = Color.black;
        float disabledSize = 0;

        Material preMat;

        static Material defaultMaterial = null;

        public static Material DefaultMaterial
        {
            get
            {
                if (defaultMaterial == null)
                {
                    defaultMaterial = Resources.Load<Material>("Outline");
                }
                return defaultMaterial;
            }
        }

        public void DisableOutline()
        {
            MaterialPropertyBlock _mpb = new MaterialPropertyBlock();
            SpriteRenderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat("_OutlineSize", disabledSize);
            _mpb.SetColor("_OutlineColor", disabledColor);
            SpriteRenderer.SetPropertyBlock(_mpb);
        }

        public void EnableOutline()
        {
            float _outlineSize = Mathf.PingPong(2f,9f);
            MaterialPropertyBlock _mpb = new MaterialPropertyBlock();
            SpriteRenderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat("_OutlineSize", _outlineSize);
            _mpb.SetColor("_OutlineColor", color);
            SpriteRenderer.SetPropertyBlock(_mpb);
        }            

        void OnEnable()
        {
            preMat = SpriteRenderer.sharedMaterial;
            SpriteRenderer.sharedMaterial = DefaultMaterial;
            UpdateOutline();
        }

        void OnDisable()
        {
            SpriteRenderer.sharedMaterial = preMat;
        }

        void OnValidate()
        {
            if (enabled)
            {
                UpdateOutline();
            }
        }

        void UpdateOutline()
        {
            if (!enableOutline) DisableOutline();
            if (enableOutline) EnableOutline();
        }
    }
}
