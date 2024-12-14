using UnityEngine;
using UnityEngine.UIElements;

public class PlayerData : MonoBehaviour
{
    public GameObject player;
    
    public Image playerIcon;
    public Vector3 playerPosition;
    public string playerName;
    public string playerId;

    public void SetPlayerPosition(float x, float y, float z)
    {
        player.transform.position = new Vector3(x, y, z);
    }
}