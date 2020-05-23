using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextBoxManager : MonoBehaviour
{
    public TextAsset textFile;
    public string[] textLines;
    public List<string> textShowed;

    public GameObject dialogBoxCanvas;
    public GameObject btnCanvas;

    public GameObject dialogBox;
    public EventTrigger dialogBoxEventTrigger;
    public Text charaNameText;
    public Text dialogText;

    public Button autoBtn;
    public Text autoBtnText;
    public Button skipBtn;
    public Text skipBtnText;

    public int currentTextWord;
    public int currentTextLine;

    public bool isShowingTexts;
    public bool isShowingTextLine;
    public bool isAutoMode;

    public float changeTextWordDeltaTime;
    public float changeTextLineDeltaTime;

    public Coroutine showTextLineCoroutine;
    public Coroutine autoPlayCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        dialogBoxCanvas = GameObject.Find("DialogBoxCanvas");
        btnCanvas = GameObject.Find("BtnCanvas");

        dialogBox = GameObject.Find("DialogBoxCanvas/DialogBox");
        dialogBoxEventTrigger = dialogBox.GetComponent<EventTrigger>();
        charaNameText = dialogBox.transform.Find("CharaNameText").GetComponent<Text>();
        dialogText = dialogBox.transform.Find("DialogText").GetComponent<Text>();

        autoBtn = btnCanvas.transform.Find("AutoBtn").GetComponent<Button>();
        autoBtnText = autoBtn.transform.Find("Text").GetComponent<Text>();
        skipBtn = btnCanvas.transform.Find("SkipBtn").GetComponent<Button>();
        skipBtnText = skipBtn.transform.Find("Text").GetComponent<Text>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener(OnDialogBoxClick);
        dialogBoxEventTrigger.triggers.Add(entry);

        autoBtn.onClick.AddListener(OnAutoPlayBtnClick);
        skipBtn.onClick.AddListener(OnSkipBtnClick);

        changeTextWordDeltaTime = 0.05f;
        changeTextLineDeltaTime = 1.0f;

        CloseDialogBox();
    }

    void Update()
    {
        isShowingTexts = textLines != null && currentTextLine < textLines.Length;
        isShowingTextLine = isShowingTexts && currentTextWord <= textLines[currentTextLine].Length;

        autoBtnText.text = isAutoMode ? "Playing" : "Auto";
        autoBtn.interactable = isAutoMode ? false : true;

        if (isShowingTexts)
        {
            if (Input.GetMouseButtonDown(1))
            {
                SetDialogBoxActive(!dialogBox.activeSelf);
            }
        }
        else
        {
            if (dialogBox.activeSelf)
            {
                CloseDialogBox();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SetDialogBox();
            }
        }
    }

    private IEnumerator ShowTextLine()
    {
        while (true)
        {
            if (dialogBox.activeSelf && isShowingTextLine)
            {
                dialogText.text = textLines[currentTextLine].Substring(0, currentTextWord++);
            }

            yield return new WaitForSeconds(changeTextWordDeltaTime);
        }
    }

    private IEnumerator AutoPlay()
    {
        while (true)
        {
            isAutoMode = true;

            if (dialogBox.activeSelf && !isShowingTextLine)
            {
                yield return new WaitForSeconds(changeTextLineDeltaTime);

                NextTextLine();
            }

            yield return new WaitForSeconds(changeTextWordDeltaTime);
        }
    }

    public void SetDialogBox()
    {
        if (textFile != null)
        {
            textLines = (textFile.text.Split('\n'));
        }
        else
        {
            Debug.LogError("Text File not found");
            return;
        }

        charaNameText.text = "DD";

        SetDialogBoxActive(true);

        showTextLineCoroutine = StartCoroutine(ShowTextLine());
    }

    private void SetDialogBoxActive(bool value)
    {
        dialogBox.SetActive(value);
        autoBtn.gameObject.SetActive(value);
        skipBtn.gameObject.SetActive(value);
    }

    private void NextTextLine()
    {
        textShowed.Add(textLines[currentTextLine]);

        ++currentTextLine;
        currentTextWord = 0;
    }

    private void CloseDialogBox()
    {
        charaNameText.text = "";
        dialogText.text = "";

        textLines = null;
        textShowed.Clear();

        currentTextWord = 0;
        currentTextLine = 0;
        isAutoMode = false;

        StopAllCoroutines();
        SetDialogBoxActive(false);
    }

    private void OnDialogBoxClick(BaseEventData arg0)
    {
        if (isAutoMode)
        {
            StopCoroutine(autoPlayCoroutine);
            isAutoMode = false;
        }
        else
        {
            if (isShowingTextLine)
            {
                currentTextWord = textLines[currentTextLine].Length;
            }
            else
            {
                NextTextLine();
            }
        }
    }

    private void OnAutoPlayBtnClick()
    {
        autoPlayCoroutine = StartCoroutine(AutoPlay());
    }

    private void OnSkipBtnClick()
    {
        CloseDialogBox();
    }
}
