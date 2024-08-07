using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerReactions : NetworkBehaviour
{
    [SerializeField] private List<EmojiSO> emojiList;
    private Dictionary<KeyCode, int> keyMapping;

    private void Start()
    {
        keyMapping = new Dictionary<KeyCode, int>
        {
            { KeyCode.Alpha1, 0 },
            { KeyCode.Alpha2, 1 },
            { KeyCode.Alpha3, 2 },
            { KeyCode.Alpha4, 3 },
            { KeyCode.Alpha5, 4 },
            { KeyCode.Alpha6, 5 },
            { KeyCode.Alpha7, 6 },
            { KeyCode.Alpha8, 7 },
            { KeyCode.Alpha9, 8 },
            { KeyCode.Alpha0, 9 }
        };
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        foreach (var key in keyMapping.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                int index = keyMapping[key];
                if (index < emojiList.Count)
                {
                    SpawnReactionServerRpc(index, transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity);
                }
            }
        }
    }

    [ServerRpc]
    private void SpawnReactionServerRpc(int index, Vector3 pos, Quaternion rot)
    {
        GameObject objToSpawn = Instantiate(emojiList[index].emojiPrefab, pos, rot);
        objToSpawn.GetComponent<NetworkObject>().Spawn();


        Debug.Log(objToSpawn + " Spawned");
    }
}
