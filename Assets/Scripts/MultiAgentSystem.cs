using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class MultiAgentSystem : MonoBehaviour
{
    [Header("Materiales")]
    [SerializeField] private Material groundMaterial;
    [SerializeField] private Material agentMaterial;

    private int gridSize = 10;
    private Dictionary<int, GameObject> agents = new Dictionary<int, GameObject>();
    private TcpClient socketClient;
    private NetworkStream networkStream;
    private Thread receiveThread;
    private bool isConnected = false;
    private Queue<string> messageQueue = new Queue<string>();

    void Start()
    {
        if (groundMaterial == null)
            groundMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = new Color(0.7f, 0.7f, 0.7f)
            };

        if (agentMaterial == null)
            agentMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = new Color(1, 0.2f, 0.2f)
            };

        CreateGrid();
        CreateAgentCubes();
        ConnectToServer();
    }

    void CreateGrid()
    {
        Color[] tileColors = new Color[]
        {
            new Color(0.85f, 0.85f, 0.85f), // Gris claro
            new Color(0.6f, 0.6f, 0.6f)     // Gris oscuro
        };

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.name = $"Tile_{x}_{y}";
                tile.transform.position = new Vector3(x, 0, y);
                tile.transform.localScale = new Vector3(1, 0.1f, 1);
                tile.transform.parent = transform;

                MeshRenderer renderer = tile.GetComponent<MeshRenderer>();

                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                int colorIndex = (x + y) % 2;
                mat.color = tileColors[colorIndex];

                renderer.material = mat;
            }
        }
    }

    void CreateAgentCubes()
    {
        Color[] agentColors = new Color[]
        {
            Color.red,
            Color.green,
            Color.blue
        };

        for (int i = 0; i < 3; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = $"Agent_{i}";
            cube.transform.position = new Vector3(i, 0.5f, 0);
            cube.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            cube.transform.parent = transform;

            MeshRenderer renderer = cube.GetComponent<MeshRenderer>();

            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = agentColors[i];
            renderer.material = mat;

            Rigidbody rb = cube.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            agents[i] = cube;
        }
    }

    void ConnectToServer()
    {
        Thread connectThread = new Thread(() =>
        {
            try
            {
                socketClient = new TcpClient("127.0.0.1", 1101);
                networkStream = socketClient.GetStream();
                isConnected = true;
                Debug.Log("Conectado al servidor");

                receiveThread = new Thread(ReceiveData);
                receiveThread.Start();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error conectando: {e.Message}");
            }
        });
        connectThread.Start();
    }

    void ReceiveData()
    {
        byte[] buffer = new byte[4096];
        StringBuilder messageBuffer = new StringBuilder();

        while (isConnected)
        {
            try
            {
                int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) { isConnected = false; break; }

                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                messageBuffer.Append(data);

                string[] messages = messageBuffer.ToString().Split('$');

                for (int i = 0; i < messages.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(messages[i]))
                    {
                        lock (messageQueue)
                        {
                            messageQueue.Enqueue(messages[i]);
                        }
                    }
                }

                messageBuffer.Clear();
                messageBuffer.Append(messages[messages.Length - 1]);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error recibiendo: {e.Message}");
                isConnected = false;
            }
        }
    }

    void Update()
    {
        lock (messageQueue)
        {
            while (messageQueue.Count > 0)
            {
                string message = messageQueue.Dequeue();
                ProcessMessage(message);
            }
        }
    }

    void ProcessMessage(string message)
    {
        try
        {
            if (message.Contains("\"id\"") && message.Contains("\"x\""))
            {
                AgentData[] agentsData =
                    JsonUtility.FromJson<AgentDataList>("{\"agents\":" + message + "}").agents;

                foreach (var agentData in agentsData)
                {
                    if (agents.ContainsKey(agentData.id))
                    {
                        Vector3 newPos = new Vector3(agentData.x, 0.5f, agentData.y);
                        agents[agentData.id].transform.position = newPos;
                    }
                }
            }
        }
        catch { }
    }

    void OnDestroy()
    {
        isConnected = false;
        networkStream?.Close();
        socketClient?.Close();
    }

    [System.Serializable]
    public class AgentData
    {
        public int id;
        public int x;
        public int y;
    }

    [System.Serializable]
    public class AgentDataList
    {
        public AgentData[] agents;
    }
}
