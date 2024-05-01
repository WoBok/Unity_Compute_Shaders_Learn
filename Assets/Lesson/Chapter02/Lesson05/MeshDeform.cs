using UnityEngine;

public class MeshDeform : MonoBehaviour
{
    struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;

        public Vertex(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
        }
    }

    public ComputeShader shader;
    [Range(0.5f, 2.0f)]
    public float radius;

    int kernelHandle;
    Mesh mesh;
    Vertex[] originalVertex;
    Vertex[] changedVertex;
    ComputeBuffer originalVBuffer;
    ComputeBuffer changedVBuffer;

    void Start()
    {

        if (InitData())
        {
            InitShader();
        }
    }

    private bool InitData()
    {
        kernelHandle = shader.FindKernel("CSMain");

        MeshFilter mf = GetComponent<MeshFilter>();

        if (mf == null)
        {
            Debug.Log("No MeshFilter found");
            return false;
        }

        InitVertexArrays(mf.mesh);
        InitGPUBuffers();

        mesh = mf.mesh;

        return true;
    }

    private void InitShader()
    {
        shader.SetFloat("radius", radius);

    }

    private void InitVertexArrays(Mesh mesh)
    {
        originalVertex = new Vertex[mesh.vertices.Length];
        changedVertex = new Vertex[mesh.vertices.Length];

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            var vertex = new Vertex(mesh.vertices[i], mesh.normals[i]);
            originalVertex[i] = vertex;
            changedVertex[i] = vertex;
        }
    }

    private void InitGPUBuffers()
    {
        originalVBuffer = new ComputeBuffer(originalVertex.Length, sizeof(float) * 6);
        originalVBuffer.SetData(originalVertex);
        changedVBuffer = new ComputeBuffer(changedVertex.Length, sizeof(float) * 6);
        changedVBuffer.SetData(changedVertex);

        shader.SetBuffer(kernelHandle, "originalVBuffer", originalVBuffer);
        shader.SetBuffer(kernelHandle, "changedVBuffer", changedVBuffer);
    }

    void GetVerticesFromGPU()
    {
        changedVBuffer.GetData(changedVertex);
        var vertices = new Vector3[changedVertex.Length];
        var normal = new Vector3[changedVertex.Length];
        for (int i = 0; i < changedVertex.Length; i++)
        {
            vertices[i] = changedVertex[i].position;
            normal[i] = changedVertex[i].normal;
        }
        mesh.vertices = vertices;
        mesh.normals = normal;
    }

    void Update()
    {
        if (shader)
        {
            shader.SetFloat("radius", radius);
            float delta = (Mathf.Sin(Time.time) + 1) / 2;
            shader.SetFloat("delta", delta);
            shader.Dispatch(kernelHandle, changedVertex.Length, 1, 1);

            GetVerticesFromGPU();
        }
    }

    void OnDestroy()
    {
        originalVBuffer.Dispose();
        changedVBuffer.Dispose();
    }
}

