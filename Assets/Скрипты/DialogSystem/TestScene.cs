using UnityEngine;

public class TestScene : MonoBehaviour
{

    void OnTriggerStay2D(Collider2D other)
    {

        if (Input.GetKeyDown(KeyCode.E))
        {

            DialogueTrigger tr = transform.GetComponent<DialogueTrigger>();
            if (tr != null && tr.fileName != string.Empty)
            {
                DialogueManager.Internal.DialogueStart(tr.fileName);
            }

        }

        if (Input.GetKeyDown(KeyCode.E) && !DialogueManager.isActive)
        {
            transform.gameObject.SetActive(false);
        }
    }
}
