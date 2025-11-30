using UnityEngine;

public class ManualGridSetup : MonoBehaviour
{
    [SerializeField] private MultiAgentSystem gridManager;

    public void CreateGridManually()
    {
        // Buscar el GridManager en la escena
        if (gridManager == null)
            gridManager = FindObjectOfType<MultiAgentSystem>();

        if (gridManager != null)
        {
            Debug.Log("GridManager encontrado. Los elementos se crearán automáticamente al presionar Play");
        }
        else
        {
            Debug.LogError("No se encontró GridManager en la escena");
        }
    }
}