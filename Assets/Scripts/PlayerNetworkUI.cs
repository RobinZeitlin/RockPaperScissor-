using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNetworkUI : NetworkBehaviour
{
    [SerializeField] private List<EmojiSO> emojiSO;
    [SerializeField] private GameObject templateReaction;
    [SerializeField] private Transform UIReference;

    [SerializeField] private KeyCode menuButton;

    private List<GameObject> spawnedReactions = new List<GameObject>();

    private void Update()
    {
        if(!IsOwner) return;

        UI();
        
    }
    public void UI()
    {
        if(emojiSO.Count > 0 && Input.GetMouseButtonDown(1))
        {
            Debug.Log($"UI opened with {emojiSO.Count} reactions.");

            Cursor.lockState = CursorLockMode.None;

            SpawnReactionUI();
        }
        else if(emojiSO.Count == 0)
        {
            Debug.LogWarning($"No reactions in List");
        }
        else if(Input.GetMouseButtonUp(1))
        {
            Debug.LogWarning($"UI closed by releasing {menuButton}");

            Cursor.lockState = CursorLockMode.Locked;

            DestroyAllCurrentReactions();
        }
    }
    void SpawnReactionUI()
    {
        float degreesPer = 360 / emojiSO.Count;

        float minRadius = 0.0f;
        float maxRadius = 50.0f;

        for (int i = 0; i < emojiSO.Count; i++)
        {
            GameObject tempReaction = SpawnTempReaction(i);
            StartCoroutine(SetTransform(i, degreesPer, minRadius, maxRadius, tempReaction));
            spawnedReactions.Add(tempReaction);
        }
    }
    public GameObject SpawnTempReaction(int index)
    {
        GameObject tempReaction = Instantiate(templateReaction, transform.GetChild(0).GetChild(0));

        tempReaction.name = emojiSO[index].name;
        tempReaction.GetComponent<Image>().sprite = emojiSO[index].emojiSprite;

        tempReaction.GetComponent<ReactionTemplate>().reactionType = emojiSO[index].reactionType;

        return tempReaction;
    }
    public IEnumerator SetTransform(int index, float degreesPer, float minRadius, float maxRadius, GameObject targetObj)
    {
        float angle = index * degreesPer;
        float radians = angle * Mathf.Deg2Rad;

        RectTransform rectTransform = targetObj.GetComponent<RectTransform>();

        float lerpedRadius = minRadius;
        float duration = 0.1f;
        float elapsedTime = 0f;

        while (lerpedRadius < maxRadius)
        {
            if(rectTransform == null) yield break;

            elapsedTime += Time.deltaTime;
            lerpedRadius = Mathf.Lerp(minRadius, maxRadius, elapsedTime / duration);

            float x = Mathf.Cos(radians) * lerpedRadius;
            float y = Mathf.Sin(radians) * lerpedRadius;

            rectTransform.localPosition = new Vector2(x, y);

            yield return null;
        }

        float finalX = Mathf.Cos(radians) * maxRadius;
        float finalY = Mathf.Sin(radians) * maxRadius;

        if(rectTransform == null)
        {
            Debug.LogError("RectTransform is null");
            yield break;
        }
        
        rectTransform.localPosition = new Vector2(finalX, finalY);
    }

    void DestroyAllCurrentReactions()
    {
        foreach (GameObject reaction in spawnedReactions)
        {
            if (reaction != null)
            {
                Destroy(reaction);
            }
        }
        spawnedReactions.Clear();
    }
}
