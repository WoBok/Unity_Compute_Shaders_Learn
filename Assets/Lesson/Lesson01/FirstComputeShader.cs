using UnityEngine;

public class FirstComputeShader : MonoBehaviour
{
    public ComputeShader shader;
    public int texResolution = 256;
    public Vector2 threadGroop = new Vector2(1, 1);

    Renderer m_Renderer;
    RenderTexture m_OutputTexture;
    int m_KernelHandle;

    void Start()
    {
        m_Renderer = GetComponent<Renderer>();
        m_Renderer.enabled = true;

        CreateTexture();
        UpdateShader();
    }
    void CreateTexture()
    {
        m_OutputTexture = new RenderTexture(texResolution, texResolution, 0);
        m_OutputTexture.enableRandomWrite = true;
        m_OutputTexture.Create();
    }
    void UpdateShader()
    {
        m_KernelHandle = shader.FindKernel("CSMain");
        shader.SetTexture(m_KernelHandle, "Result", m_OutputTexture);
        m_Renderer.material.SetTexture("_MainTex", m_OutputTexture);

        DispatchShader((int)threadGroop.x, (int)threadGroop.y);
    }
    void DispatchShader(int x, int y)
    {
        shader.Dispatch(m_KernelHandle, x, y, 1);
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            if (m_OutputTexture != null)
                m_OutputTexture.Release();
            CreateTexture();
            UpdateShader();
        }
    }
}