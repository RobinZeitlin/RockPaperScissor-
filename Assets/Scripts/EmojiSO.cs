using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmojiSO", menuName = "EmojiSO")]
public class EmojiSO : ScriptableObject
{
    public Sprite emojiSprite;
    public ReactionType reactionType;
}
