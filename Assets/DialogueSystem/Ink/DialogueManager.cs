using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.EventSystems;
using TMPro; //vi su dung Story
public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] GameObject dialoguePannel;
    [SerializeField] TextMeshProUGUI dialogueText;
    private Story currentStory;
    public bool dialogueIsPlaying { get; set; } //readonly//nen tao 1 statemanger trong game dung cho tat ca cac scene va kiem soat duoc tat ca cac trang thai
    [SerializeField] public GameObject[] choices;
    private TextMeshProUGUI[] choiceTexts;
    private static DialogueManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);//vi la singleton nen gan dialoguemanager vao nhieu hon 1 object se bi loi
            return;
        }
        instance = this;//make this shit singleton
    }
    public static DialogueManager GetInstance()
    {
        return instance;
    }
    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePannel.SetActive(false);
        //get all of the choices text
        choiceTexts = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choiceTexts[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            //text is a child object for each choice
            index++;
        }
    }
    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ContinueStory();
        }
        // if (nếu bấm nút submit(E) && state bắt m phải bấm)
        // {
        //     ContinueStory();
        // }
        // else if (state không bắt m phải bấm)
        // {
        //     ContinueStory();//cần phải tạo 1 cách để chặn
        // }
    }

    public void EnterDialougueMode(TextAsset inkJSON)//bat dau trang thai dialogue
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePannel.SetActive(true);
        ContinueStory();
    }
    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f); //method này đợi 0,2 s hẵng chạy
        dialogueIsPlaying = false;
        dialoguePannel.SetActive(false);
        dialogueText.text = "";
    }
    private void ContinueStory()
    {
        if (currentStory.canContinue)//can continue la trang thai co the tiep tuc hay khong thuoc extension ink
        {
            dialogueText.text = currentStory.Continue();//lay text trong ink
            DisplayChoices();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("more choices than UI");
        }
        int intdex = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[intdex].gameObject.SetActive(true);//active la tao cho no hien ra = choice.text;
            choiceTexts[intdex].text = choice.text;
            intdex++;
        }
        //go through the remaining choices the UI supports and hide them
        for (int i = currentChoices.Count; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        //Event System requires we clear it first??? then wait
        //for one frame before we set the current selected object
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
    }
}