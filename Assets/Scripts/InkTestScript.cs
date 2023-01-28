using Ink.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InkTestScript : MonoBehaviour
{
    [SerializeField] private TextAsset _inkJson;

    private Story _inkStory;

    // Start is called before the first frame update
    void Start()
    {
        _inkStory = new Story(_inkJson.text);

    }

    // Update is called once per frame
    void Update()
    {
    }

    public async Task ReadStory()
    {
        while (_inkStory.canContinue)
        {
            var storyText = _inkStory.Continue();
            await ReadNextLine(storyText);
        }

        Debug.Log("The End");
    }

    public async Task ReadNextLine(
        string text)
    {
        Debug.Log(text);

        var end = Time.time + 10;
        while (Time.time < end)
        {
            await Task.Yield();
        }
    }
}
