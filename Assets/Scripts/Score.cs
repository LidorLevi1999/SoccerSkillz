using UnityEngine;

[CreateAssetMenu(fileName = "Score", menuName = "Data/Score")]
public class Score : ScriptableObject
{
	public int homePoints;

	public int awayPoints;

	public override string ToString()
	{
		return $"{homePoints} : {awayPoints}";
	}
}
