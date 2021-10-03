using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    public Transform player;
    public Transform head;
    public Transform mainCamera;
    public GameObject victoryScreen;
    public GameObject gameOverScreen;
    public float sizeMaxGameOver = 3f;
    public float sizeMinGameOver = 0.5f;
    public string nextScene;

    bool victory = false;
    bool gameOver = false;

    void Update()
    {
        if (gameOver && Input.anyKeyDown)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (victory && Input.anyKeyDown)
        {

            if (!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                SceneManager.LoadScene("Level0");
            }
        }
        if (Input.GetButtonDown("Restart"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void FixedUpdate()
    {
        if (!gameOver &&
            (player.gameObject.GetComponent<PlayerBehaviour>().scale < sizeMinGameOver ||
            player.gameObject.GetComponent<PlayerBehaviour>().scale > sizeMaxGameOver))
        {
            this.GameOver();
        }
    } 

    void OnTriggerEnter2D(Collider2D col)
    {

        if (GameObject.ReferenceEquals(col.gameObject, player.gameObject))
        {
            this.Victory();
        }
    }

    public void Victory()
    {
        this.victory = true;
        Vector3 vector = new Vector3(mainCamera.position.x, mainCamera.position.y, this.transform.position.z - 5);
        GameObject.Instantiate(victoryScreen, vector, Quaternion.identity);
        player.position = head.position;
        player.BroadcastMessage("GameOver");
    }

    public void GameOver()
    {
        this.gameOver = true;
        player.BroadcastMessage("GameOver");
        Vector3 vector = new Vector3(mainCamera.position.x, mainCamera.position.y, this.transform.position.z - 5);
        GameObject.Instantiate(gameOverScreen, vector, Quaternion.identity);
    }
}
