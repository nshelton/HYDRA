using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physarum : MonoBehaviour
{
    public struct Agent
    {
            public float posX;
            public float posY;
            public float direction;
            public float age;
            public float r;
            public float g;
            public float b;
            public float random;
    }

    public ComputeShader m_fieldShader;

    private Dictionary<string, int> m_kernels = new Dictionary<string, int>();
    string[] m_fieldKernelNames = new string[]
    {
        "Step",
        "RenderField",
        "RenderAgents",
        "XGaussian",
        "YGaussian",
        "Deposit",
        "RenderLuma",
        "Post"
    };

    [SerializeField] private RenderTexture _resultTexture;
    private RenderTexture m_fieldA;
     private RenderTexture m_fieldB;
    private GPUBuffer<Agent> m_particleBuffer;

    public Texture SourceTexture {
        get { return (m_inputType == InputType.Video) ? (Texture)m_inputVideo : (Texture)m_inputImage; }
    }

    public int NumThreadsX { get { return ((int)m_resolution.x + 7 ) / 8 ; } }
    public int NumThreadsY { get { return ((int)m_resolution.y + 7 ) / 8 ; } }

    public enum InputType 
    {
        Video,
        Image
    }


    public enum BlendMode 
    {
        Over = 0,
        Additive = 1,
        Screen = 2
    }

    [SerializeField] public Vector2 m_resolution = new Vector2(1920, 1080);
    [SerializeField] public Material m_material;

    [SerializeField] public InputType m_inputType = InputType.Video;
    [SerializeField] public RenderTexture m_inputVideo;
    [SerializeField] public Texture2D m_inputImage;
    [SerializeField][Range(0,1)] float _inputAmount = 0.1f;
    [Range(0,0.5f)] public float _margin = 0.1f;


    private RenderTexture _screenTex;
    private RenderTexture _screenTexSwap;
    private RenderTexture _lumaTex;
    private RenderTexture _postTex;

    
  //  [SerializeField] int m_textureDim = 1024;
    [SerializeField] int m_particleCount = 100000;
    [SerializeField][Range(0.95f,1)] float m_fieldDecay = 0.99f;
    public float _fieldDecay 
    {
        get {return m_fieldDecay;}
        set {m_fieldDecay = value;}
    }
    [SerializeField][Range(0,1)] float m_fieldRenderScale = 1f;
    public float _fieldRenderScale 
    {
        get {return m_fieldRenderScale;}
        set {m_fieldRenderScale = value;}
    }
    [SerializeField][Range(0.01f,1)] float m_fieldBlurAmount = 1f;
    public float _fieldBlurAmount 
    {
        get {return m_fieldBlurAmount;}
        set {m_fieldBlurAmount = value;}
    }

    [SerializeField][Range(0,100f)] public float _sensorDegrees = 0.1f;
    public float sensorDegrees 
    {
        get {return _sensorDegrees;}
        set {_sensorDegrees = value;}
    }
    [SerializeField][Range(0,25)] public float _sensorDist = 1f;
    public float sensorDist 
    {
        get {return _sensorDist;}
        set {_sensorDist = value;}
    }
    [SerializeField][Range(0,360f)] public float _rotationDegrees = 0.1f;
    public float rotationDegrees 
    {
        get {return _rotationDegrees;}
        set {_rotationDegrees = value;}
    }
    [SerializeField][Range(0,200f)] public float _speed = 0.1f;
    public float speed 
    {
        get {return _speed;}
        set {_speed = value;}
    }
    [SerializeField][Range(0,1)] public float _speedRandom = 0.1f;
    public float speedRandom 
    {
        get {return _speedRandom;}
        set {_speedRandom = value;}
    }

    [SerializeField][Range(0,1f)] public float _pheremoneAmount = 0.1f;
    public float pheremoneAmount 
    {
        get {return _pheremoneAmount;}
        set {_pheremoneAmount = value;}
    }
    [SerializeField][Range(1,30f)] public int _iterations = 1;
    public float iterations 
    {
        get {return _iterations;}
        set {_iterations = Mathf.FloorToInt(value);}
    }
    [Header("Rendering")]
    [SerializeField][ColorUsage(false, true)] Color _pointColor;
    [SerializeField] bool _colorFromTexture;
    [SerializeField] bool _renderAgents = true;
    [SerializeField] bool _renderField = false;
    [SerializeField][Range(0.8f,1)] public float m_textureDecay = 0.99f;
    
    public float _textureDecay 
    {
        get {return m_textureDecay;}
        set {m_textureDecay = value;}
    }
    [SerializeField][Range(0.01f,1)] public float m_textureBlurAmount = 1f;
    public float _textureBlurAmount 
    {
        get {return m_textureBlurAmount;}
        set {m_textureBlurAmount = value;}
    }

    [Range(0,5)] public float _lumaRenderScale = 1;
    public float LumaRenderScale 
    {
        get {return _lumaRenderScale;}
        set {_lumaRenderScale = value;}
    }
    [Range(0,1)] public float _lumaAmount = 1;
    public float LumaAmount 
    {
        get {return _lumaAmount;}
        set {_lumaAmount = value;}
    }
    [Range(0,5)] public float _pointRadius = 0.5f;
    public float PointRadius 
    {
        get {return _pointRadius;}
        set {_pointRadius = value;}
    }

    [Range(0,1)] public float _pointAlpha = 0.5f;
    public float PointAlpha 
    {
        get {return _pointAlpha;}
        set {_pointAlpha = value;}
    }
    public BlendMode _blendMode = BlendMode.Over;

    [Header("Post")]

    [Range(0,2)] public float _brightness = 1f;
    public float Brightness 
    {
        get {return _brightness;}
        set {_brightness = value;}
    }

    [Range(0,2)] public float _contrast = 1f;
    public float Contrast 
    {
        get {return _contrast;}
        set {_contrast = value;}
    }

    [Header("forces")]

    [SerializeField][Range(0,0.1f)] public float _noiseAmount = 1;
    public float noiseAmount 
    {
        get {return _noiseAmount;}
        set {_noiseAmount = value;}
    }

    [SerializeField][Range(0,1)] public float _noiseRandom = 0f;
    public float noiseRandom 
    {
        get {return _noiseRandom;}
        set {_noiseRandom = value;}
    }
    [SerializeField][Range(0,1f)] public float _noiseScroll = 1;
    public float noiseScroll 
    {
        get {return _noiseScroll;}
        set {_noiseScroll = value;}
    }
    [SerializeField][Range(0,100f)] public float _noiseFreq = 1;
    public float noiseFreq 
    {
        get {return _noiseFreq;}
        set {_noiseFreq = value;}
    }
    [SerializeField] public float _noiseSeed = 1;
    public float noiseSeed 
    {
        get {return _noiseSeed;}
        set {_noiseSeed = value;}
    }

    [SerializeField][Range (1,5)] int _fractalLevels = 1;

    [SerializeField] Vector2 _linearForce = Vector2.zero;
    [SerializeField][Range(0,1)] public float _linearRandom = 0f;
    public float linearRandom 
    {
        get {return _linearRandom;}
        set {_linearRandom = value;}
    }

    public Vector2 linearForce
    {
        get { return _linearForce; }
        set { _linearForce = value; }
    }

    public float RadialForce 
        {
        get {return _radialForce;}
        set {_radialForce = value;}
    }

    [SerializeField][Range (-0.1f,0.1f)]  float _radialForce = 0;

    [SerializeField][Range(0,1)] public float _radialRandom = 0f;
    public float radialRandom 
    {
        get {return _radialRandom;}
        set {_radialRandom = value;}
    }
    [SerializeField] Vector2 _radialCenter = Vector2.zero;

    [Header("particle system")]
    public float _maxLifetime = 100;
    public float maxLifetime 
    {
        get {return _maxLifetime;}
        set {_maxLifetime = value;}
    }
    private Texture2D m_lumaGradientTexture;
    public Gradient m_lumaGradient;
    public Gradient m_gradient;
    
    

    private Texture2D m_gradientTexture;
    private Vector3 m_fieldBlurKernel;
    private Vector3 m_textureBlurKernel;
    void Init()
    {
        if ( _screenTex != null)
            _screenTex?.Release();

        _screenTex = null;

        if ( _screenTexSwap != null)
            _screenTexSwap?.Release();
        _screenTexSwap = null;
        
        if ( _lumaTex != null)
            _lumaTex?.Release();
        _lumaTex = null;
        
        if ( _postTex != null)
            _postTex?.Release();
        _postTex = null;
        
        m_particleBuffer?.Dispose();
        m_particleBuffer = new GPUBuffer<Agent>(m_particleCount);

        for (int i = 0; i < m_particleCount; i++)
        {
            m_particleBuffer.CPUData[i].posX = Random.value;
            m_particleBuffer.CPUData[i].posY = Random.value;
            m_particleBuffer.CPUData[i].direction = Random.value * Mathf.PI * 2;
            m_particleBuffer.CPUData[i].age = Random.value * _maxLifetime;
            m_particleBuffer.CPUData[i].random = Random.value * 2 - 1;
        }
        m_particleBuffer.Upload();

        // textures
        m_fieldA = new RenderTexture(_resultTexture.width, _resultTexture.height, 0, RenderTextureFormat.ARGBFloat);
        m_fieldA.filterMode = FilterMode.Bilinear;
        m_fieldA.wrapMode = TextureWrapMode.Repeat;
        m_fieldA.enableRandomWrite = true;
        m_fieldA.Create();

        m_fieldB = new RenderTexture(_resultTexture.width, _resultTexture.height, 0, RenderTextureFormat.ARGBFloat);
        m_fieldB.filterMode = FilterMode.Bilinear;
        m_fieldB.wrapMode = TextureWrapMode.Repeat;
        m_fieldB.enableRandomWrite = true;
        m_fieldB.Create();

            
        m_material.mainTexture = _screenTex;

        foreach (var k in m_fieldKernelNames)
        {
            m_kernels[k] = m_fieldShader.FindKernel(k);
        }

        m_gradientTexture= new Texture2D(128,1);
        m_gradientTexture.wrapMode = TextureWrapMode.Clamp;

        m_lumaGradientTexture= new Texture2D(128,1);
        m_lumaGradientTexture.wrapMode = TextureWrapMode.Clamp;


        UpdateGradientTextures();
    }
    void UpdateGradientTextures() 
    {
        if ( m_gradientTexture != null)
        {
             for(int x = 0; x < m_gradientTexture.width; x ++)
            {
                m_gradientTexture.SetPixel(x, 0, m_gradient.Evaluate((float)x / m_gradientTexture.width));
            }
            
            m_gradientTexture.Apply();
        }

        if ( m_lumaGradientTexture != null)
        {
             for(int x = 0; x < m_lumaGradientTexture.width; x ++)
            {
                m_lumaGradientTexture.SetPixel(x, 0, m_lumaGradient.Evaluate((float)x / m_lumaGradientTexture.width));
            }
            
            m_lumaGradientTexture.Apply();
        }


    }

    void Start()
    {
        Init();
    }
    float N(float i, float sigma) {
         return Mathf.Exp( -0.5f * (i * i) / (sigma * sigma));
    }

    Vector3 Gaussian(float sigma)
    {
        Vector3 subKernel = new Vector3(
            N(2, sigma),
            N(1, sigma),
            N(0, sigma));

        return  subKernel / (2 * subKernel.x + 2 * subKernel.y + subKernel.z);
    }

    void OnValidate()
    {
        if (m_fieldBlurAmount > 0)
            m_fieldBlurKernel = Gaussian(m_fieldBlurAmount); 
        
        if (m_textureBlurAmount > 0)
            m_textureBlurKernel = Gaussian(m_textureBlurAmount); 

        UpdateGradientTextures();
    }

    void Swap(RenderTexture a, RenderTexture b)
    {
        var tmp = a;
        a = b;
        b = tmp;
    }

    void Update()
    {
        
        if (!m_kernels.ContainsKey("Step") || Input.GetKeyDown(KeyCode.Space))
        {
            Init();
            return;
        }

        m_fieldShader.SetFloat("_colorFromTexture", _colorFromTexture ? 1 : 0);
        m_fieldShader.SetFloat("_maxLifetime", _maxLifetime);
        m_fieldShader.SetVector("_fieldDim", new Vector2(m_fieldA.width, m_fieldA.height));
        if ( _screenTex != null)
        {
            m_fieldShader.SetVector("_screenDim", new Vector2(m_fieldA.width, m_fieldA.height));
        }
            m_fieldShader.SetFloat("_blurKernelA", m_fieldBlurKernel.x);
        m_fieldShader.SetFloat("_blurKernelB", m_fieldBlurKernel.y);
        m_fieldShader.SetFloat("_blurKernelC", m_fieldBlurKernel.z);
        m_fieldShader.SetFloat("_dt", Time.deltaTime);
        m_fieldShader.SetFloat("_decayWeights", m_fieldDecay);
        m_fieldShader.SetFloat("_speed", _speed);
        m_fieldShader.SetVector("_speedRandom", new Vector4(_speedRandom, _noiseRandom, _linearRandom, _radialRandom));
        m_fieldShader.SetFloat("_margin", _margin);
        m_fieldShader.SetFloat("_sensorRadians", _sensorDegrees * Mathf.Deg2Rad);
        m_fieldShader.SetFloat("_sensorDist", _sensorDist);
        m_fieldShader.SetFloat("_inputAmount", _inputAmount);
        m_fieldShader.SetFloat("_pheremoneAmount", _pheremoneAmount);
        m_fieldShader.SetFloat("_rotationRadians", _rotationDegrees * Mathf.Deg2Rad);
        m_fieldShader.SetVector("_pointColor", _pointColor);
        m_fieldShader.SetFloat("_fractalLevels", _fractalLevels);
        
        m_fieldShader.SetFloat("_time", Time.time);

        m_fieldShader.SetVector("_noiseParam", new Vector4(_noiseAmount, _noiseFreq, _noiseScroll, _noiseSeed));
        m_fieldShader.SetVector("_linearForce", _linearForce);
        m_fieldShader.SetVector("_radialCenter", _radialCenter);
        m_fieldShader.SetFloat("_radialForce", _radialForce);

        m_fieldShader.SetTexture(m_kernels["Deposit"], "_Source", SourceTexture);
        m_fieldShader.SetTexture(m_kernels["Step"], "_Input", SourceTexture);

        for(int i = 0; i < _iterations; i++)
        {
            m_fieldShader.SetTexture(m_kernels["Step"], "_Source", m_fieldB);
            m_fieldShader.SetTexture(m_kernels["Step"], "_Destination", m_fieldA);
            m_fieldShader.SetBuffer(m_kernels["Step"], "_particleBuffer", m_particleBuffer.Buffer);
            m_fieldShader.Dispatch(m_kernels["Step"], m_particleBuffer.Buffer.count / 16, 1, 1);
            
            m_fieldShader.SetTexture(m_kernels["XGaussian"], "_Source", m_fieldA);
            m_fieldShader.SetTexture(m_kernels["XGaussian"], "_Destination", m_fieldB);
            m_fieldShader.Dispatch(m_kernels["XGaussian"], NumThreadsX , NumThreadsY , 1); 

            m_fieldShader.SetTexture(m_kernels["YGaussian"], "_Source", m_fieldB);
            m_fieldShader.SetTexture(m_kernels["YGaussian"], "_Destination", m_fieldA);
            m_fieldShader.Dispatch(m_kernels["YGaussian"], NumThreadsX , NumThreadsY , 1); 
                        
            m_fieldShader.SetTexture(m_kernels["Deposit"], "_Destination", m_fieldA);
            m_fieldShader.Dispatch(m_kernels["Deposit"], NumThreadsX, NumThreadsY , 1); 

            Swap(m_fieldA, m_fieldB);
        }
        Render();
    }

    void Render()
    {

        if ( _screenTex == null || _screenTex.width != _resultTexture.width)
        {
            _screenTex = new RenderTexture(_resultTexture);
            _screenTex.enableRandomWrite = true;
            _screenTex.filterMode = FilterMode.Bilinear;
            _screenTex.antiAliasing = 1;
            _screenTex.Create();

            _screenTexSwap = new RenderTexture(_resultTexture);
            _screenTexSwap.enableRandomWrite = true;
            _screenTexSwap.filterMode = FilterMode.Bilinear;
            _screenTexSwap.antiAliasing = 1;
            _screenTexSwap.Create();

            _lumaTex = new RenderTexture(_resultTexture);
            _lumaTex.enableRandomWrite = true;
            _lumaTex.antiAliasing = 1;
            _lumaTex.Create();

            _postTex = new RenderTexture(_resultTexture);
            _postTex.enableRandomWrite = true;
            _postTex.antiAliasing = 1;
            _postTex.Create();

        }

        int NumThreadsScreenX =  (m_fieldA.width + 7 ) / 8 ;
        int NumThreadsScreenY =  (m_fieldA.height + 7 ) / 8 ;

        if ( _renderField)
        {
            m_fieldShader.SetFloat("_renderScale", m_fieldRenderScale);
            m_fieldShader.SetTexture(m_kernels["RenderField"], "_Source", m_fieldA);
            m_fieldShader.SetTexture(m_kernels["RenderField"], "_Destination", _screenTex);
            m_fieldShader.SetTexture(m_kernels["RenderField"], "_gradient", m_gradientTexture);

            m_fieldShader.Dispatch(
                m_kernels["RenderField"],
                NumThreadsScreenX,
                NumThreadsScreenY, 1);
        }

        if ( _renderAgents)
        {
            m_fieldShader.SetFloat("_pointRadius", _pointRadius);
            m_fieldShader.SetFloat("_pointAlpha", _pointAlpha);
            m_fieldShader.SetFloat("_blendMode", (int)_blendMode);
            m_fieldShader.SetBuffer(m_kernels["RenderAgents"], "_particleBuffer", m_particleBuffer.Buffer);
            m_fieldShader.SetTexture(m_kernels["RenderAgents"], "_Destination", _screenTex);
            m_fieldShader.Dispatch(m_kernels["RenderAgents"], m_particleBuffer.Buffer.count / 16, 1, 1);

            m_fieldShader.SetFloat("_blurKernelA", m_textureBlurKernel.x);
            m_fieldShader.SetFloat("_blurKernelB", m_textureBlurKernel.y);
            m_fieldShader.SetFloat("_blurKernelC", m_textureBlurKernel.z);
             m_fieldShader.SetFloat("_decayWeights", m_textureDecay);

            m_fieldShader.SetTexture(m_kernels["XGaussian"], "_Source", _screenTex);
            m_fieldShader.SetTexture(m_kernels["XGaussian"], "_Destination", _screenTexSwap);
            m_fieldShader.Dispatch(m_kernels["XGaussian"], NumThreadsScreenX, NumThreadsScreenY, 1); 

            m_fieldShader.SetTexture(m_kernels["YGaussian"], "_Source", _screenTexSwap);
            m_fieldShader.SetTexture(m_kernels["YGaussian"], "_Destination", _screenTex);
            m_fieldShader.Dispatch(m_kernels["YGaussian"], NumThreadsScreenX, NumThreadsScreenY, 1); 

            if ( _lumaAmount > 0)
            {
              
                m_fieldShader.SetTexture(m_kernels["Post"], "_Source", _screenTex);
                m_fieldShader.SetTexture(m_kernels["Post"], "_Destination", _postTex);
                m_fieldShader.SetFloat("_brightness", _brightness);
                m_fieldShader.SetFloat("_contrast", _contrast);
                
                m_fieldShader.Dispatch(
                    m_kernels["Post"],
                    NumThreadsScreenX,
                    NumThreadsScreenY, 1);
                
                m_fieldShader.SetFloat("_renderScale", _lumaRenderScale);
                m_fieldShader.SetFloat("_lumaAmount", _lumaAmount);
                
                m_fieldShader.SetTexture(m_kernels["RenderLuma"], "_Source", _postTex);
                m_fieldShader.SetTexture(m_kernels["RenderLuma"], "_Destination", _lumaTex);
                m_fieldShader.SetTexture(m_kernels["RenderLuma"], "_gradient", m_lumaGradientTexture);
                
                m_fieldShader.Dispatch(
                    m_kernels["RenderLuma"],
                    NumThreadsScreenX,
                    NumThreadsScreenY, 1);


                Graphics.Blit(_lumaTex, _resultTexture);
            }
        }
        
        if ( _lumaAmount == 0)
        {
           m_fieldShader.SetTexture(m_kernels["Post"], "_Source", _screenTex);
                m_fieldShader.SetTexture(m_kernels["Post"], "_Destination", _postTex);
                m_fieldShader.SetFloat("_brightness", _brightness);
                m_fieldShader.SetFloat("_contrast", _contrast);
                
                m_fieldShader.Dispatch(
                    m_kernels["Post"],
                    NumThreadsScreenX,
                    NumThreadsScreenY, 1);
                
                Graphics.Blit(_postTex, _resultTexture);
        }
    }
}
