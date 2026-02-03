using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton;
    [SerializeField] private AudioSource textSound;
    [SerializeField] private float fadeSpeed = 4f;

    void Awake()
    {
        nextButton.onClick.AddListener(() =>
        {
            DialogueManager.Instance.NextLine();
        });
    }
        void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        nextButton.gameObject.SetActive(false);
    }


        public void Show()
    {
        gameObject.SetActive(true); // ensure enabled
        nextButton.gameObject.SetActive(true);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        StopAllCoroutines();
        StartCoroutine(Fade(1));
    }

        public void Hide()
    {
        nextButton.gameObject.SetActive(false);

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        StopAllCoroutines();
        StartCoroutine(Fade(0));
    }



    public void SetText(string text)
    {
        dialogueText.text = text;
        if (textSound) textSound.Play();
    }

        IEnumerator Fade(float target)
    {
        while (!Mathf.Approximately(canvasGroup.alpha, target))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime * fadeSpeed);
            yield return null;
        }
    }

}
