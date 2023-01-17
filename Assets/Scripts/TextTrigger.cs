using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    [SerializeField] private GameObject textWritter;

    private TextWriter textWriterScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (textWritter != null)
            {
                textWriterScript = textWritter.GetComponent<TextWriter>();

                textWriterScript.canWrite = true;
                textWriterScript.triggerText = true;

                gameObject.SetActive(false);
            }
        }
    }
}
