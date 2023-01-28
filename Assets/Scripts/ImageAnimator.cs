using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private bool _loop = true;
    [SerializeField] private bool _destroyOnEnd = false;

    [Tooltip("Framerate of the displayed animation")]
    [SerializeField]
    private int _fps = 8;    

    private Image image;
    private float _clock = 0f;
    private int _index = 0;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (!_loop && _index == _sprites.Length)
        {
            return;
        }

        _clock += Time.deltaTime;
        var secondsPerFrame = 1 / (float)_fps;
        while (_clock >= secondsPerFrame)
        {
            _clock -= secondsPerFrame;
            UpdateFrame();
        }
    }

    void UpdateFrame()
    {
        image.sprite = _sprites[_index];
        _index++;

        if (_index >= _sprites.Length)
        {
            if (_loop) _index = 0;
            if (_destroyOnEnd) Destroy(gameObject);
        }
    }
}
