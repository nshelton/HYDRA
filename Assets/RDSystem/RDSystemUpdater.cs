using UnityEngine;

namespace RDSystem
{
    public class RDSystemUpdater : BaseGUIModule
    {
        [SerializeField] CustomRenderTexture _texture;
        [SerializeField, Range(1, 16)] int _stepsPerFrame = 4;

        [SerializeField, Range(0, 1)] float m_RotationPower = 0.5f;
        [SerializeField, Range(0, 1)] float m_Amplification = 0.5f;
        [SerializeField, Range(0, 1)] float m_Diagonal = 0.5f;
        [SerializeField, Range(0, 1)] float m_curl = 0.25f;
        [SerializeField, Range(0, 1)] float m_laplacian = 0.24f;
        [SerializeField, Range(0, 2)] float m_laplDiv = 0.24f;
        [SerializeField, Range(0, 2)] float m_div = 0.24f;

        [SerializeField, Range(0, 2)] float m_centerWeight = 1f;
        [SerializeField, Range(0, 2)] float m_edgeWeight = 1f;
        [SerializeField, Range(0, 2)] float m_cornerWeight = 1f;
        [SerializeField, Range(0, 2)] float m_displace = 0.1f;

        [SerializeField, Range(0, 1)] float m_dy = 0f;
        [SerializeField, Range(0, 1)] float m_dx = 0f;


        private Material m_material;

        void Start()
        {
            _texture.Initialize();
            m_material = GetComponent<MeshRenderer>().material;
        }
        public override string Name() { return "surface"; }


        public override void Init()
        {
            Parameters.Add(new GUIFloat("displace", 0, 1, 0.1f,
                delegate (float v) { m_displace = v; }));
            Parameters.Add(new GUIFloat("Steps", 0, 16, 1,
                 delegate (float v) { _stepsPerFrame = (int)v; }));
            Parameters.Add(new GUIFloat("m_dy", 0, 1, 0,
                 delegate (float v) { m_dy = v; }));
            Parameters.Add(new GUIFloat("m_dx", 0, 1, 0,
                delegate (float v) { m_dx = v; }));
            Parameters.Add(new GUIFloat("rotation", 0, 1, 0.3f,
                delegate (float v) { m_RotationPower = v; }));
            Parameters.Add(new GUIFloat("amplification", 0, 1, 0.5f,
                delegate (float v) { m_Amplification = v; }));
            Parameters.Add(new GUIFloat("diagonal", 0, 1, 0.5f,
                delegate (float v) { m_Diagonal = v; }));

            foreach (var p in Parameters)
            {
                var r = new GUIRow();
                r.Items.Add(p);
                GUIRows.Add(r);
            }
            base.Init();

        }

        void Update()
        {
            _texture.material.SetFloat("_Laplacian", m_laplacian);
            _texture.material.SetFloat("_Curl", m_curl);
            _texture.material.SetFloat("_RotationPower", m_RotationPower);
            _texture.material.SetFloat("_Amplification", m_Amplification);
            _texture.material.SetFloat("_Diagonal", m_Diagonal);
            _texture.material.SetFloat("_LaplDivScale", m_laplDiv);
            _texture.material.SetFloat("_DivScale", m_div);
            _texture.material.SetFloat("_CenterWeight", m_centerWeight);
            _texture.material.SetFloat("_EdgeWeight", m_edgeWeight);
            _texture.material.SetFloat("_CornerWeight", m_cornerWeight);

            _texture.material.SetFloat("_dx", m_dx / 1000);
            _texture.material.SetFloat("_dy", m_dy / 1000);

            m_material.SetFloat("_Displacement", m_displace);

            _texture.Update(_stepsPerFrame);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _texture.Initialize();
            }
        }
    }
}
