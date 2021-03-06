using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public SlingShooter slingShooter;
    public TrailController trailController;
    public BoxCollider2D tapCollider;

    public GameObject birdContainer;
    public GameObject enemyContainer;
    public UIPauseMenuController pauseMenuController;
    
    public List<Bird> birds;
    public List<Enemy> enemies;

    private Bird _shotBird;
    private bool _isGameEnded = false;

    private void Awake()
    {
        birds.AddRange(birdContainer.GetComponentsInChildren<Bird>());
        enemies.AddRange(enemyContainer.GetComponentsInChildren<Enemy>());
    }

    private void Start()
    {
        for (int i = 0; i < birds.Count; i++)
        {
            birds[i].OnBirdDestroyed += ChangeBird;
            birds[i].OnBirdShot += AssignTrail;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].OnEnemyDestroyed += ReduceEnemy;
        }
        
        tapCollider.enabled = false;
        slingShooter.InstantiateBird(birds[0]);
        _shotBird = birds[0];

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            pauseMenuController.RestartGame();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (_isGameEnded == false)
            {
                if (Time.timeScale != 0f)
                {
                    pauseMenuController.PauseGame();
                }
                else
                {
                    pauseMenuController.ResumeGame();
                }
            }
        }
    }

    public void ChangeBird()
    {
        tapCollider.enabled = false;
        if (_isGameEnded) return;
        
        birds.RemoveAt(0);

        if (birds.Count > 0)
        {
            slingShooter.InstantiateBird(birds[0]);
            _shotBird = birds[0];
        }
        CheckGameEnd();
    }

    public void ReduceEnemy(GameObject destroyedEnemy)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].gameObject == destroyedEnemy)
            {
                enemies.RemoveAt(i);
                break;
            }
        }
    }
    
    public void CheckGameEnd()
    {
        if (enemies.Count == 0)
        {
            SetGameOver(true);
        }
        else if (birds.Count == 0)
        {
            SetGameOver(false);
        }
    }

    public void AssignTrail(Bird bird)
    {
        trailController.targetBird = bird;
        StartCoroutine(trailController.SpawnTrail());
        tapCollider.enabled = true;
    }

    void OnMouseUp()
    {
        if (_shotBird != null)
        {
            _shotBird.OnTap();
        }
    }
    
    public void SetGameOver(bool isWin)
    {
        _isGameEnded = true;

        pauseMenuController.EndGame(isWin);
    }
}
