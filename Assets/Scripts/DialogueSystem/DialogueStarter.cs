using UnityEngine;
using Yarn.Unity;

public class DialogueStarter: MonoBehaviour
{
    public DialogueRunner runner;

    void Start()
    {
        runner.StartDialogue(
            "Test"
        );
    }
}