using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Design : MonoBehaviour
{
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject prefab4;
    
    public GameObject Sphere;
    public GameObject shpere1;
    private GameObject Cube;
    private GameObject Quad;
    private GameObject pitSphere;
    private GameObject Grass;
    public string[] pitnames;
    public static bool clickAble = true;
    public float ball_position_x;
    public float ball_position_y;
    public float ball_position_z;
    public float ball_rotate_x = 0;
    public float ball_rotate_z = 0;
    public float move_ball_position_x;
    public float move_ball_position_y;
    public float move_ball_position_z;
    private GameManager gameManager;
    public static bool lose = false;
    float distance;
    public bool moveFlag = false;
    string direction = "center";
    float moveX;
    float moveZ;
    float commonX;
    float commonZ;
    private float xAngle, yAngle, zAngle;
    // Start is called before the first frame update
    void Start()
    {
        pitnames = new string[12];
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Cube = Instantiate(prefab1, new Vector3(-6f + 3f * j,0f, -18.7f + 3.4f * i), Quaternion.identity);
                Cube.transform.SetParent(GameObject.FindGameObjectWithTag("Plane").transform);
                Cube.name = "Cube" + (i + 1) + "_" + (j + 1);
                Cube.transform.localScale = new Vector3(1.8f, 0.1f, 0.75f);
                Cube.GetComponent<ClickHandler>().setId(i * 5 + j + 1);

                Grass = Instantiate(prefab4, new Vector3(-6f + 3f * j, -0.5f, -18.7f + 3.4f * i), Quaternion.identity);
                Grass.transform.SetParent(GameObject.FindGameObjectWithTag("Plane").transform);
                Grass.name = "Grass" + (i + 1) + "_" + (j + 1);
                Grass.transform.localScale = new Vector3(0.035f, 0.1f, 0.015f);
            }
            Quad = Instantiate(prefab2, new Vector3(-9f, 0.27f, -17f + 3.4f * i), Quaternion.identity);

            Quad.transform.SetParent(GameObject.FindGameObjectWithTag("Plane").transform);
            Quad.name = "Quad1_" + (i + 1);
            Quad.transform.eulerAngles = new Vector3(Quad.transform.eulerAngles.x + 20,
                Quad.transform.eulerAngles.y,
                Quad.transform.eulerAngles.z
            );
            Quad.transform.localScale = new Vector3(3.2f, 1f, 1f);


            Quad = Instantiate(prefab2, new Vector3(9f, 0.27f, -17f + 3.4f * i), Quaternion.identity);
            Quad.transform.SetParent(GameObject.FindGameObjectWithTag("Plane").transform);
            Quad.name = "Quad2_" + (i + 1);
            Quad.transform.eulerAngles = new Vector3(Quad.transform.eulerAngles.x + 20,
                Quad.transform.eulerAngles.y,
                Quad.transform.eulerAngles.z
            );
            Quad.transform.localScale = new Vector3(3.2f, 1f, 1f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (moveFlag)
        {
            if (direction == "right")
            {
                if (Sphere.transform.position.x < move_ball_position_x && Sphere.transform.position.z < move_ball_position_z)
                {
                    commonX = Sphere.transform.position.x + distance * 0.3f / moveZ;
                    commonZ = Sphere.transform.position.z + distance * 0.3f / moveX;
                    if (commonX > move_ball_position_x)
                    {
                        commonX = move_ball_position_x;
                    }
                    Sphere.transform.position = new Vector3(commonX, Sphere.transform.position.y, commonZ);
                    Sphere.transform.Rotate(xAngle, 0, zAngle);
                }
            }
            else if (direction == "left")
            {
                if (Sphere.transform.position.x > move_ball_position_x && Sphere.transform.position.z < move_ball_position_z)
                {
                    commonX = Sphere.transform.position.x - distance * 0.3f / moveZ;
                    commonZ = Sphere.transform.position.z + distance * 0.3f / moveX;
                    if (commonX < move_ball_position_x)
                    {
                        commonX = move_ball_position_x;
                    }
                    Sphere.transform.position = new Vector3(commonX, Sphere.transform.position.y, commonZ);
                    Sphere.transform.Rotate(-xAngle, 0, zAngle);
                }
            }
            else
            {
                if (Sphere.transform.position.z < move_ball_position_z)
                {
                    Sphere.transform.position = new Vector3(Sphere.transform.position.x, Sphere.transform.position.y, Sphere.transform.position.z + 0.3f);
                    Sphere.transform.Rotate(0, 0, 30);
                }
            }
        }
    }
    // public IEnumerator ballMove(Vector3 from, Vector3 to, Vector3 angleFrom,Vector3 angleTo)
    // {
    //     float time = 0;
    //     const float seconds = 0.5f;
    //     while (time < seconds)
    //     {
    //         Sphere.transform.position = Vector3.Lerp(from, to, time / seconds);
    //         Sphere.transform.rotation = Quaternion.Lerp(Quaternion.Euler(angleFrom), Quaternion.Euler(angleTo), time / seconds);
    //         time += Time.deltaTime;
    //         yield return new WaitForEndOfFrame();
    //     }
    //     Sphere.transform.position = to;
    //     yield return new WaitForSeconds(0.1f);
    // }
    public void handleClickNumber(int number, float x, float y, float z)
    {
        int n = 0;
        for (int i = 0; i < 12; i++)
        {
            if (GameManager.pit[i] == number)
            {
                n++;
            }
        }
        ball_position_x = Sphere.transform.position.x;
        ball_position_y = Sphere.transform.position.y;
        ball_position_z = Sphere.transform.position.z;
        move_ball_position_x = x;
        move_ball_position_y = y;
        move_ball_position_z = z;
        moveFlag = true;
        distance = Vector3.Distance(Sphere.transform.position, new Vector3(x, y, z));
        moveX = Mathf.Abs(move_ball_position_x - ball_position_x);
        moveZ = Mathf.Abs(move_ball_position_z - ball_position_z);
        
        xAngle = distance * 1000 / moveZ;
        zAngle = distance * 1000 / moveX;
        if (ball_position_x < move_ball_position_x){
            direction = "right";
        }
        if (ball_position_x > move_ball_position_x){
            direction = "left";
        }
        if (ball_position_x == move_ball_position_x){
            direction = "center";
        }
        if (n > 0){
            for (int i = 0; i < 12; i++)
            {
                int num = (int)GameManager.pit[i] / 5 + 1;
                int order = (int)GameManager.pit[i] % 5;
                if (order == 0)
                {
                    num = num - 1;
                    order = 5;
                }
                string objectname = "Cube" + num + "_" + order;
                float positionX = GameObject.Find(objectname).transform.position.x;
                float positionZ = GameObject.Find(objectname).transform.position.z;
                pitSphere = Instantiate(prefab3, new Vector3(positionX, 0.546f, positionZ), Quaternion.identity);
                pitSphere.transform.SetParent(GameObject.FindGameObjectWithTag("Plane").transform);
                pitSphere.name = "pitSphere" + GameManager.pit[i];
                pitnames[i] = pitSphere.name;
                pitSphere.transform.localScale = new Vector3(1.8f, 1.8f, 0.8f);
            }
            GameManager.btnflag = true;
            GameManager.server = false;
            clickAble = false;
            lose = true;
        }
        else{
            if(GameManager.loop == 12){
                GameManager.server = true;
                GameManager.btnflag = true;
            }else{
                int num = (int)GameManager.pit[GameManager.loop - 1] / 5 + 1;
                int order = (int)GameManager.pit[GameManager.loop - 1] % 5;
                if (order == 0)
                {
                    num = num - 1;
                    order = 5;
                }
                string objectname = "Cube" + num + "_" + order;
                float positionX = GameObject.Find(objectname).transform.position.x;
                float positionZ = GameObject.Find(objectname).transform.position.z;
                pitSphere = Instantiate(prefab3, new Vector3(positionX, 0.546f, positionZ), Quaternion.identity);
                pitSphere.transform.SetParent(GameObject.FindGameObjectWithTag("Plane").transform);
                pitSphere.name = "pitSphere" + GameManager.pit[GameManager.loop - 1];
                pitnames[GameManager.loop - 1] = pitSphere.name;
                pitSphere.transform.localScale = new Vector3(1.8f, 1.8f, 0.8f);
            }
        }
    }
}
