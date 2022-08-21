using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClickHandler : MonoBehaviour
{
    private GameObject grass;
    private GameObject startbtn;
    private GameManager gameManager;
    private Design design;
    public int id;
    // Start is called before the first frame update
    void Start()
    {
        design = FindObjectOfType<Design>();
        if (GameManager.btnflag)
        {
            Firstcolor();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.btnflag && GameManager.loop == 0)
        {
            design.Sphere.transform.position = new Vector3(0, 1f, -23f);
            Fullcolor();
            Rowcolor(GameManager.loop);
            if (design.pitnames.Length > 0)
            {
                for (int i = 0; i < design.pitnames.Length; i++)
                {
                    Destroy(GameObject.Find(design.pitnames[i]));
                }
            }
        }
    }
    public void setId(int _id)
    {
        id = _id;
    }
    void OnMouseDown()
    {
        if (GameManager.btnflag)
        {
            GameManager.loop = 0;
        }
        else
        {
            GameManager.startbtn.GetComponent<Button>().interactable = true;
            if (Design.clickAble)
            {
                GameManager.loop = GameManager.loop + 1;
                if (Check(gameObject.name, GameManager.loop))
                {
                    Clickcolor(gameObject.name);
                    design.handleClickNumber(id, gameObject.transform.position.x, 1f, gameObject.transform.position.z);
                }
                else
                {
                    GameManager.loop = GameManager.loop - 1;
                }
                if (GameManager.loop < 12)
                {
                    Rowcolor(GameManager.loop);
                }
            }
        }
    }
    private void Fullcolor()
    {
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                string name = "Cube" + (i + 1).ToString() + "_" + (j + 1).ToString();
                for (int k = 0; k < 8; k++)
                {
                    grass = GameObject.Find(name);
                    grass.GetComponent<MeshRenderer>().material.color =Color.black;
                }
            }
        }
    }
    private void Clickcolor(string objname)
    {
        for (int k = 0; k < 8; k++)
        {
            grass = GameObject.Find(name);
            grass.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
    }
    private void Rowcolor(int n)
    {
        for (int i = 0; i < 5; i++)
        {
            string name;
            if (Design.clickAble)
            {
                name = "Cube" + (n + 1).ToString() + "_" + (i + 1).ToString();
            }
            else
            {
                name = "Cube" + n.ToString() + "_" + (i + 1).ToString();
            }
            grass = GameObject.Find(name);
            grass.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
    public void Firstcolor()
    {
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                string name = "Cube" + (i + 1).ToString() + "_" + (j + 1).ToString();
                for (int k = 0; k < 8; k++)
                {
                    grass = GameObject.Find(name);
                    grass.GetComponent<MeshRenderer>().material.color = Color.white;
                }
            }
        }
    }
    private bool Check(string objectname, int n)
    {
        int k = 0;
        for (int i = 0; i < 5; i++)
        {
            string name = "Cube" + n.ToString() + "_" + (i + 1).ToString();
            if (objectname == name)
            {
                k++;
            }
        }
        if (k > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
