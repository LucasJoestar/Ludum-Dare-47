using UnityEngine;

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Rewind effect")]
#pragma warning disable 0649
public class RewindEffect: MonoBehaviour
    {

        [SerializeField, Range(0, 1)]
        float scanLineJitter = 0;

        public float ScanLineJitter {
            get { return scanLineJitter; }
            set { scanLineJitter = value; }
        }


        [SerializeField, Range(0, 1)]
        float verticalJump = 0;

        public float VerticalJump {
            get { return verticalJump; }
            set { verticalJump = value; }
        }
        

        [SerializeField, Range(0, 1)]
        float colorDrift = 0;

        public float ColorDrift {
            get { return colorDrift; }
            set { colorDrift = value; }
        }

        [SerializeField] Shader _shader;

        Material material;

        float verticalJumpTime;

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (material == null)
            {
                material = new Material(_shader);
                material.hideFlags = HideFlags.DontSave;
            }

            verticalJumpTime += Time.deltaTime * verticalJump * 11.3f;

            float _threshold = Mathf.Clamp01(1.0f - scanLineJitter * 1.2f);
            float _displace = 0.002f + Mathf.Pow(scanLineJitter, 3) * 0.05f;
            material.SetVector("_ScanLineJitter", new Vector2(_displace, _threshold));

            Vector2 _vj = new Vector2(verticalJump, verticalJumpTime);
            material.SetVector("_VerticalJump", _vj);

            Vector2 _cd = new Vector2(colorDrift * 0.04f, Time.time * 606.11f);
            material.SetVector("_ColorDrift", _cd);

            Graphics.Blit(source, destination, material);
        }
    }