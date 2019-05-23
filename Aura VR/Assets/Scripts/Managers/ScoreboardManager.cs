﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardManager
{
    #region Singleton
    private static ScoreboardManager _instance;

    public static ScoreboardManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ScoreboardManager();
            return _instance;
        }
    }
    #endregion

    private string _filepath = "scores.txt";
    public string Filepath { get; set; }

    private int _scoreboardSize = 100;
    private List<ScoreData> _scores;

    private ScoreboardManager()
    {
        GameManager.Instance.OnGameOver += SaveScores;
        _scores = new List<ScoreData>();
    }

    public void SaveScores()
    {
        Debug.Log("Saving scores");

        try
        {
            using (StreamWriter sw = new StreamWriter(_filepath))
            {
                for (int i = 0; i < _scores.Count; i += 1)
                {
                    if (_scores[i].valid)
                    {
                        string line = _scores[i].ToString();

                        sw.WriteLine(line);
                    }
                    else
                    {
                        Debug.LogWarning("Attempted to save invalid score record \n" + 
                            "name: " + _scores[i].name + "\n" +
                            "score: " + _scores[i].score);
                    }
                }

                sw.Close();
            }

            Debug.Log("Scores saved");
        }
        catch
        {
            Debug.LogWarning("Failed to save scores at: " + _filepath);
        }
    }

    public void AddNewRecord(float score)
    {
        if (_scores.Count == 0)
        {
            DateTime time = DateTime.Now;
            ScoreData newScoreData = new ScoreData("Not Set", score, time);

            // place in list
            _scores.Add(newScoreData);

            return;
        }

        for (int i = 0; i < _scores.Count; i += 1)
        {
            if (score < _scores[i].score)
            {
                // Create record with current score
                DateTime time = DateTime.Now;
                ScoreData newScoreData = new ScoreData("Not Set", score, time);

                // place in list
                _scores.Insert(i, newScoreData);

                break;
            }
        }

        if (_scores.Count > _scoreboardSize)
        {
            // remove the excess records
            _scores.RemoveRange(_scoreboardSize, _scores.Count - _scoreboardSize);
        }
    }

    public List<ScoreData> LoadScores()
    {
        Debug.Log("Loading scoreboard");

        List<ScoreData> scoresFromFile = new List<ScoreData>();

        try
        {
            using (StreamReader sr = new StreamReader(_filepath))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    ScoreData score = new ScoreData(line);

                    if (score.valid)
                        scoresFromFile.Add(score);

                    Debug.Log("Score Loaded: " + score.score);
                }
            }

            Debug.Log("Scoreboard loaded");
        }
        catch
        {
            Debug.LogWarning("Failed to load scores from: " + _filepath);
        }

        return scoresFromFile;
    }
}

public struct ScoreData
{
    public bool valid;
    public string name;
    public float score;
    public DateTime time;
    
    public ScoreData(string name, float score, DateTime time)
    {
        valid = true;
        this.name = name;
        this.score = score;
        this.time = time;
    }

    public ScoreData(string dataLine)
    {
        string[] parts = dataLine.Split(' ');

        if (parts.Length == 3)
        {
            valid = true;
            name = parts[0];
            float.TryParse(parts[1], out score);
            DateTime.TryParse(parts[2], out time);
        }
        else
        {
            valid = false;
            name = dataLine;
            score = 0;
            time = DateTime.Now;
        }
    }

    public override string ToString()
    {
        return "";
    }
}
