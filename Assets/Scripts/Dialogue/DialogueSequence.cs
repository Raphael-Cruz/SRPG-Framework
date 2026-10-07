using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Header("Identificação")]
    public string speakerName;
    public string speakerID;
    
    [Tooltip("Marque se esta fala for do Player. Se desmarcado, será considerado NPC.")]
    public bool isPlayer; 

    [Header("Visual")]
    public Sprite portrait;

    [Header("Conteúdo")]
    [TextArea(3, 5)]
    public string text;
    
    public float displayTime = 3f;     
}

[CreateAssetMenu(fileName = "New Dialogue Sequence", menuName = "SRPG/Dialogue/Sequence")]
public class DialogueSequence : ScriptableObject
{
    [Header("Primeira Conversa (Sequencial)")]
    [Tooltip("Estas falas tocarão em ordem, uma após a outra, na primeira vez que falar com o NPC.")]
    public List<DialogueLine> firstTimeLines;

    [Header("Conversas Seguintes (Sorteio)")]
    [Tooltip("Após a primeira vez, o NPC sorteará apenas UMA destas linhas para falar por interação.")]
    public List<DialogueLine> randomRepeatedLines;
}