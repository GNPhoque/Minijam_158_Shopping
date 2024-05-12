using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextEffects : MonoBehaviour
{
    [SerializeField] bool typeWriter;
    [SerializeField] bool waveAllText;

    [SerializeField] float typeWriterTimeBetweenChars;
    [SerializeField] float typeWriterStartDelay;

	bool processingTypeWriter;
    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (typeWriter)
        {
			StartCoroutine(TypeWriter());
        }
    }

    void Update()
    {
        if(waveAllText) WaveAllText();
    }

    IEnumerator TypeWriter()
    {
        text.maxVisibleCharacters = 0;
        if(typeWriterStartDelay > 0f) yield return new WaitForSeconds(typeWriterStartDelay);

        text.ForceMeshUpdate();
        int totalChars = text.textInfo.characterCount;
        int counter = 0;
        processingTypeWriter = true;

		while (processingTypeWriter)
        {
            int visibleChars = counter % (totalChars + 1);
            text.maxVisibleCharacters = visibleChars;

            if (visibleChars >= totalChars)
            {
        processingTypeWriter = false;
                break;
			}

            counter++;
            yield return new WaitForSeconds(typeWriterTimeBetweenChars);
        }

        text.maxVisibleCharacters = totalChars;
	}

	[ContextMenu("SkipTypeWriter")]
    public void SkipTypeWriter()
    {
        processingTypeWriter = false;
    }

    private void WaveAllText()
	{
		text.ForceMeshUpdate();
		TMP_TextInfo info = text.textInfo;

		for (int i = 0; i < info.characterCount; i++)
        {
            var charInfo = info.characterInfo[i];

            if (!charInfo.isVisible) continue;

            var verts = info.meshInfo[charInfo.materialReferenceIndex].vertices;
            for (int j = 0; j < 4; j++)
            {
                var origin = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = origin + new Vector3(0, Mathf.Sin(Time.time * 2f + origin.x * 0.01f) * 10f, 0);
            }
		}

		for (int i = 0; i < info.meshInfo.Length; i++)
		{
			var meshInfo = info.meshInfo[i];
			meshInfo.mesh.vertices = meshInfo.vertices;
			text.UpdateGeometry(meshInfo.mesh, i);
		}
	}
}
