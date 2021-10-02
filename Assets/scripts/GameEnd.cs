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
    public string nextScene;

    bool victory = false;
    bool gameOver = false;

    void FixedUpdate()
    {
        if (victory)
        {
            player.position = head.position;
            player.GetComponent<Rigidbody2D>().Sleep();
        }

        if(victory && Input.anyKey)
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if(nextScene != null)
            {
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        if (gameOver && Input.anyKey)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);            
        }

        if (player.gameObject.GetComponent<PlayerBehaviour>().scale < 0.5f)
        {
            gameOver = true;
            player.GetComponent<Rigidbody2D>().Sleep();
            Vector3 vector = new Vector3(mainCamera.position.x, mainCamera.position.y, this.transform.position.z - 3);
            GameObject.Instantiate(gameOverScreen, vector, Quaternion.identity);


        }

    } 

    void OnTriggerEnter2D(Collider2D col)
    {

        if (GameObject.ReferenceEquals(col.gameObject, player.gameObject))
        {
            this.victory = true;
            Vector3 vector = new Vector3(mainCamera.position.x, mainCamera.position.y, this.transform.position.z - 3);
            GameObject.Instantiate(victoryScreen, vector, Quaternion.identity);
        }
    }
}
