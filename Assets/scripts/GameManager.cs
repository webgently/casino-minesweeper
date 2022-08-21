using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Linq;
using SimpleJSON;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static APIForm apiform;
    public static Globalinitial _global;
    public Sprite collectimg;
    public Sprite startimg;
    public Button StartOrCollectbtn;

    public GameObject increasebtn;
    public GameObject decreasebtn;
    public GameObject input;
    public TMP_InputField inputPriceText;

    private float betValue;
    private float totalValue;
    public TMP_Text totalPriceText;
    public static float[] Score;
    public static int loop;
    public static bool btnflag = true;
    public static int[] pit;
    public static bool server = false;
    private Design design;
    public static GameObject startbtn;
    private GameObject grassch;
    private GameObject grass;
    public TMP_Text alertText;

    [DllImport("__Internal")]
    private static extern void GameReady(string msg);
    BetPlayer _player;
    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        _player.token = usersInfo["token"];
        _player.username = usersInfo["userName"];
        float i_balance = float.Parse(usersInfo["amount"]);
        totalValue = i_balance;
        totalPriceText.text = totalValue.ToString("F2");
    }
    void Start()
    {
        _player = new BetPlayer();
#if UNITY_WEBGL == true && UNITY_EDITOR == false
                    GameReady("Ready");
#endif
        Score = new float[12] { 1.23f, 1.53f, 1.9f, 2.4f, 2.99f, 3.74f, 4.68f, 5.85f, 7.31f, 9.14f, 11.42f, 14.28f };
        design = FindObjectOfType<Design>();
        pit = new int[12];
        betValue = 10f;
        inputPriceText.text = betValue.ToString("F2");
    }
    // Update is called once per frame
    void Update()
    {
        if (btnflag)
        {
            StartOrCollectbtn.GetComponent<Image>().sprite = startimg;
            increasebtn.SetActive(true);
            decreasebtn.SetActive(true);
            input.SetActive(true);
            if (betValue <= 10)
            {
                decreasebtn.SetActive(false);
            }
            else if (betValue >= 1000000)
            {
                betValue = 1000000;
                increasebtn.SetActive(false);
            }
        }
        else
        {
            StartOrCollectbtn.GetComponent<Image>().sprite = collectimg;
        }
        if (server)
        {
            StartCoroutine(Server());
            server = false;
        }
        if (Design.lose)
        {
            StartCoroutine(alert("Better luck next time!", "other"));
            Design.lose = false;
        }
    }
    public void halfControll()
    {
        betValue = betValue / 2;
        inputPriceText.text = betValue.ToString("F2");
    }
    public void doubleControll()
    {
        betValue = betValue * 2;
        inputPriceText.text = betValue.ToString("F2");
    }
    public void inputChanged()
    {
        betValue = float.Parse(string.IsNullOrEmpty(inputPriceText.text) ? "0" : inputPriceText.text);
        if (betValue <= 10)
        {
            betValue = 10;
            inputPriceText.text = betValue.ToString("F2");
            decreasebtn.SetActive(false);
        }
        else if (betValue >= 1000000)
        {
            betValue = 1000000;
            inputPriceText.text = betValue.ToString("F2");
            increasebtn.SetActive(false);
        }
        else
        {
            increasebtn.SetActive(true);
            decreasebtn.SetActive(true);
        }
    }
    public void play()
    {
        if (totalValue >= betValue)
        {
            if (totalValue >= 10)
            {
                if (btnflag)
                {
                    StartCoroutine(beginServer());
                }
                else
                {
                    btnflag = true;
                    server = true;
                    reColor();
                }
            }
            else
            {
                StartCoroutine(alert("Insufficient balance!", "other"));
            }
        }
        else
        {
            StartCoroutine(alert("Insufficient balance!", "other"));
        }
    }
    IEnumerator beginServer()
    {
        WWWForm form = new WWWForm();
        form.AddField("userName", _player.username);
        form.AddField("token", _player.token);
        form.AddField("betAmount", betValue.ToString("F2"));
        form.AddField("amount", totalValue.ToString("F2"));
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/start-Minesweeper", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strdata);
            if (apiform.serverMsg == "Success")
            {
                pit = apiform.pitArray;
                DesignClear();
                StartCoroutine(UpdateCoinsAmount(totalValue, apiform.total));
            }
            else
            {
                StartCoroutine(alert(apiform.serverMsg, "other"));
            }
        }
        else
        {
            StartCoroutine(alert("Can't find server!", "other"));
        }
        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator Server()
    {
        WWWForm form = new WWWForm();
        form.AddField("userName", _player.username);
        form.AddField("betAmount", betValue.ToString("F2"));
        form.AddField("token", _player.token);
        form.AddField("amount", totalValue.ToString("F2"));
        form.AddField("cases", Score[loop - 1].ToString("F2"));
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/game-result", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strdata);
            if (apiform.serverMsg == "Success")
            {
                StartCoroutine(alert(apiform.msg, "win"));
                StartCoroutine(UpdateCoinsAmount(totalValue, apiform.total));
            }
            else
            {
                StartCoroutine(alert(apiform.serverMsg, "other"));
                StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + betValue));
            }
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + betValue));
            StartCoroutine(alert("Can't find server!", "other"));
        }
    }
    public IEnumerator alert(string msg, string state)
    {
        if (state == "win")
        {
            AlertController.isWin = true;
        }
        else
        {
            AlertController.isLose = true;
        }
        alertText.text = msg;
        yield return new WaitForSeconds(3.5f);
        AlertController.isWin = false;
        AlertController.isLose = false;
    }
    private void DesignClear()
    {
        AlertController.isWin = false;
        AlertController.isLose = false;
        startbtn = GameObject.Find("startbtn");
        startbtn.GetComponent<Button>().interactable = false;
        loop = 0;
        btnflag = false;
        Design.clickAble = true;
        Design.lose = false;
        server = false;
        design.moveFlag = false;
        decreasebtn.SetActive(false);
        increasebtn.SetActive(false);
        input.SetActive(false);
    }
    public void reColor()
    {
        if (loop < 12)
        {
            for (int i = 0; i < 5; i++)
            {

                string name = "Cube" + (loop + 1).ToString() + "_" + (i + 1).ToString();
                grass = GameObject.Find(name);
                grass.GetComponent<MeshRenderer>().material.color = Color.gray;
            }
        }
    }
    private IEnumerator UpdateCoinsAmount(float preValue, float changeValue)
    {
        // Animation for increasing and decreasing of coins amount
        const float seconds = 0.2f;
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            totalPriceText.text = Mathf.Floor(Mathf.Lerp(preValue, changeValue, (elapsedTime / seconds))).ToString();
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        totalValue = changeValue;
        totalPriceText.text = totalValue.ToString();
    }
}
public class BetPlayer
{
    public string username;
    public string token;
}
