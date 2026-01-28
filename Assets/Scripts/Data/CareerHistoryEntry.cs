using System;
using UnityEngine;

[System.Serializable]
public class CareerHistoryEntry
{
    public string season;
    public string teamName;
    public int matches;
    public int goals;
    public int assists;
    public float averageRating;

    public CareerHistoryEntry(string season, string team, int matches, int goals, int assists, float rating)
    {
        this.season = season;
        this.teamName = team;
        this.matches = matches;
        this.goals = goals;
        this.assists = assists;
        this.averageRating = rating;
    }
}
