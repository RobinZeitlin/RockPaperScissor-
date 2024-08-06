using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ReactionTemplate : MonoBehaviour
{
    [SerializeField] public GameObject reactionObj;
    [SerializeField] private Button button;

    private Vector3 offset = new Vector3(0, 2.5f, 0);

    private Sprite sprite;

    private void Start()
    {
        button.onClick.AddListener(Reaction);
        sprite = button.GetComponent<Image>().sprite;
    }

    public void Reaction()
    {
        GameObject tempObj = Instantiate(reactionObj, transform.position, Quaternion.identity);
        tempObj.GetComponentInChildren<Image>().sprite = sprite;

        tempObj.transform.position = transform.root.position + offset;

        NetworkObject networkObject = tempObj.GetComponent<NetworkObject>();
        networkObject.Spawn();
    }
}
