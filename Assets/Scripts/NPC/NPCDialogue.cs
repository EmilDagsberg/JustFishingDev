using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue")]
    [TextArea(2, 4)]
    public string[] lines;
    public float typingSpeed = 0.03f;

    [Header("UI")]
    public GameObject speechBubble;
    public TMP_Text dialogueText;
    public GameObject interactPrompt;

    [Header("Voice")]
    public AudioSource audioSource;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool isTalking = false;
    private bool isTyping = false;

    private Coroutine typingCoroutine;

    private NavMeshAgent agent;
    private Animator animator;
    private NPCPathWalker pathWalker;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        pathWalker = GetComponent<NPCPathWalker>();

        if (speechBubble != null)
            speechBubble.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                StartDialogue();
            }
            else
            {
                if (isTyping)
                {
                    FinishTypingInstantly();
                }
                else
                {
                    NextLine();
                }
            }
        }
    }

    void StartDialogue()
    {
        if (lines == null || lines.Length == 0)
            return;

        isTalking = true;
        currentLine = 0;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        if (pathWalker != null)
            pathWalker.enabled = false;

        if (animator != null)
            animator.SetFloat("Speed", 0f);

        if (speechBubble != null)
            speechBubble.SetActive(true);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(lines[currentLine]));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        if (audioSource != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.Play();
        }

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (audioSource != null)
            audioSource.Stop();

        isTyping = false;
        typingCoroutine = null;
    }

    void FinishTypingInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = lines[currentLine];

        if (audioSource != null)
            audioSource.Stop();

        isTyping = false;
        typingCoroutine = null;
    }

    void NextLine()
    {
        currentLine++;

        if (currentLine >= lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    void EndDialogue()
    {
        isTalking = false;
        isTyping = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (audioSource != null)
            audioSource.Stop();

        if (speechBubble != null)
            speechBubble.SetActive(false);

        if (playerInRange && interactPrompt != null)
            interactPrompt.SetActive(true);

        if (pathWalker != null)
            pathWalker.enabled = true;

        if (agent != null)
            agent.isStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (!isTalking && interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (isTalking)
            EndDialogue();
    }
}