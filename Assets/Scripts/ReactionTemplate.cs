using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public enum ReactionType
{
    sleeping,
    nerd,
}

public class ReactionTemplate : NetworkBehaviour
{
    public ReactionType reactionType;

    [SerializeField] public GameObject sleepingEmojiPrefab;
    [SerializeField] public GameObject nerdEmojiPrefab;

    [SerializeField] private Button button;

    private Vector3 offset = new Vector3(0, 2.5f, 0);

    private void Awake()
    {
        button.onClick.AddListener(OnReactionButtonClicked);
    }

    public void OnReactionButtonClicked()
    {
        GameObject playerObject = Instantiate(EmojiToSpawn());
        playerObject.transform.position = transform.root.position + offset;

        // Get the NetworkObject component
        NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();

        // Spawn the object over the network
        networkObject.Spawn();
    }

    GameObject EmojiToSpawn()
    {
        switch(reactionType)
        {
            case ReactionType.sleeping:
                return sleepingEmojiPrefab;

            case ReactionType.nerd:
                    return nerdEmojiPrefab;

                default:
                return null;
        }
    }

}
