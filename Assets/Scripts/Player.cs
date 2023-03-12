using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // 在Unity中顯示
    // public float moveSpeed = 0.3f;

    // 在Unity中顯示，但是依然保持private的特性
    [SerializeField] float moveSpeed = 0.1f;
    GameObject floorUnder;
    int hp;
    [SerializeField] GameObject hpBar;
    int score = 0;
    [SerializeField] TMP_Text scoreText;
    float scoreTime = 0f;
    Animator anim;
    AudioSource deathSound;
    [SerializeField] Button restartButton;
    // Start is called before the first frame update
    void Start()
    {
        this.hp = 9;
        this.anim = this.GetComponent<Animator>();
        this.deathSound = this.GetComponent<AudioSource>();
        this.restartButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
        //     transform.Translate(0, moveSpeed * 1.3f * Time.deltaTime, 0);
        // }else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
        //     transform.Translate(0, - moveSpeed * Time.deltaTime, 0);
        // }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            transform.Translate(- moveSpeed * Time.deltaTime, 0, 0);
            this.GetComponent<SpriteRenderer>().flipX = true;
            this.anim.SetBool("isRunning", true);
        }else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
            this.GetComponent<SpriteRenderer>().flipX = false;
            this.anim.SetBool("isRunning", true);
        }else{
            this.anim.SetBool("isRunning", false);
        }
        UpdateScore();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "floor" || other.gameObject.tag == "nail"){
            /* 接觸到的邊的線段的兩個點
            Debug.Log(other.contacts[0].point);
            Debug.Log(other.contacts[1].point); */

            Debug.Log(other.gameObject.tag);
            if(other.gameObject.tag == "floor"){
                other.gameObject.GetComponent<AudioSource>().Play();
            }

            // 接觸線段的法向量，判斷是否為頭撞到或腳撞到，而非側面撞擊
            if(other.contacts[0].normal == new Vector2(0, 1)){
                // 如果是碰到水平的才更新腳下的地板
                if (other.contacts[0].point[1] < this.transform.position.y)
                {
                    switch(other.gameObject.tag){
                        case "floor":
                            if (other.gameObject != this.floorUnder){
                                this.ModifyHp(1);
                            }
                            break;
                        case "nail":
                            if (other.gameObject != this.floorUnder){
                                this.ModifyHp(-3);
                                anim.SetTrigger("hurt");
                                other.gameObject.GetComponent<AudioSource>().Play();
                            }
                            break;
                    }
                }
                floorUnder = other.gameObject;
            }
            
        }else if(other.gameObject.tag == "ceil"){
            floorUnder.GetComponent<BoxCollider2D>().enabled = false;
            this.ModifyHp(-3);
            anim.SetTrigger("hurt");
            other.gameObject.GetComponent<AudioSource>().Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.name == "deathLine"){
            Debug.Log("死了");
            this.ModifyHp(-100);
        }
    }

    void ModifyHp(int amount)
    {
        int target;
        if(amount >= 0){
            target = Mathf.Min(this.hp + amount, 10);
            for(int hpIdx = this.hp ; hpIdx < target ; ++hpIdx){
                hpBar.transform.GetChild(hpIdx).gameObject.SetActive(true);
            }
        }else{
            target = Mathf.Max(this.hp + amount, 0);
            for(int hpIdx = target ; hpIdx < this.hp ; ++hpIdx){
                hpBar.transform.GetChild(hpIdx).gameObject.SetActive(false);
            }
            
            if(target == 0){
                this.deathSound.Play();
                // 當死了，把遊戲的時間尺度調成0，讓遊戲凍結
                Time.timeScale = 0f;
                this.restartButton.gameObject.SetActive(true);
            }
        }
        this.hp = target;
    }

    void UpdateScore()
    {
        this.scoreTime += Time.deltaTime;
        if(this.scoreTime > 1.5f){
            this.scoreTime = 0;
            this.score += 1;
            scoreText.SetText("地下" + this.score.ToString() + "層");
        }
    }

    // function to be executed by restart button
    public void Replay()
    {
        // 恢復正常時間
        Time.timeScale = 1f;
        // 重新載入場景
        SceneManager.LoadScene("SampleScene");
    }
}
