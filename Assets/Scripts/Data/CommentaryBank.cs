using UnityEngine;

[CreateAssetMenu(fileName = "CommentaryBank", menuName = "TitanSoccer/Commentary Bank")]
public class CommentaryBank : ScriptableObject
{
    [TextArea] public string[] neutralComments;
    [TextArea] public string[] homeDominatingComments;   // topla oynama bizdeyken
    [TextArea] public string[] awayDominatingComments;   // rakip baskýlý
    [TextArea] public string[] chanceStartComments;      // pozisyon baþlarken
    [TextArea] public string[] goodOutcomeComments;      // asist/gol
    [TextArea] public string[] badOutcomeComments;       // kötü þut, top kaybý
}
