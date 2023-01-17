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
                textWritter.SetActive(true);

                textWriterScript = textWritter.GetComponent<TextWriter>();

                StartCoroutine(textWriterScript.DisplayText());

                gameObject.SetActive(false);
            }
        }
    }
}
