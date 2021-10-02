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
    bool victory = false;

    void FixedUpdate()
    {
        if (victory)
        {
            player.position = head.position;
            player.GetComponent<Rigidbody2D>().Sleep();
        }
        if(victory && Input.anyKeyDown)
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (SceneManager.sceneCount > nextSceneIndex)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
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
