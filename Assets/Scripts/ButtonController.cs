using UnityEngine;
using MusicalTails.Model.Core;
using System;

public class ButtonController : MonoBehaviour
{
    private BaseButton _model;
    public bool isHeld = false;
    private SpriteRenderer _renderer;
    private float _currentHeight;
    public static event Action OnNoteMissed;

    public static float globalScrollSpeed = 5.0f;
    public static float globalShrinkSpeed = 5.0f;

    public void Initialize(BaseButton model)
    {
        _model = model;
        _renderer = GetComponent<SpriteRenderer>();
        _currentHeight = (_model is Long) ? 4.0f : 2.5f;
        _renderer.color = (_model is Long) ? Color.blue : Color.black;
        if (model is TrapButton) _renderer.color = Color.red;
        if (model is DoubleButton) _renderer.color = Color.chocolate;
        _renderer.size = new Vector2(1.0f, _currentHeight);
    }

    void Update()
    {
        if (_model is Long && isHeld)
        {
            ApplyShrinkEffect();
            return;
        }

        transform.position -= new Vector3(0, globalScrollSpeed * Time.deltaTime, 0);
        if (transform.position.y < -6f)
        {
            if (_model is DoubleButton doubleBtn && doubleBtn.IsPartiallyHit)
            {
                FindMusicManager()?.RegisterHit(doubleBtn.GetScore() / 2);
            }
            else if (!(_model is TrapButton))
            {
                OnNoteMissed?.Invoke();
            }

            Destroy(gameObject);
        }
    }

    private void ApplyShrinkEffect()
    {
        _currentHeight -= globalShrinkSpeed * Time.deltaTime;
        if (_currentHeight <= 0) { Destroy(gameObject); return; }
        _renderer.size = new Vector2(1.0f, _currentHeight);
    }

    public int GetLane() => _model != null ? _model.Lane : -1;

    static MusicManager FindMusicManager() => FindAnyObjectByType<MusicManager>();

    public void HandleClick()
    {
        if (_model is TrapButton)
        {
            FindMusicManager()?.TriggerGameOver("Ловушка!");
            return;
        }

        if (_model is DoubleButton doubleBtn)
        {
            doubleBtn.ExecuteAction();
            if (doubleBtn.HitsReceived == 2)
            {
                FindMusicManager()?.RegisterHit(doubleBtn.GetScore());
                Destroy(gameObject);
            }
            else
            {
                _renderer.color = Color.green;
            }
            return;
        }

        int points = _model.GetScore();
        FindMusicManager()?.RegisterHit(points);
        if (_model is Short) Destroy(gameObject);
        else isHeld = true;
    }

    public void ReleaseClick()
    {
        if (_model is Long longBtn && isHeld)
        {
            FindMusicManager()?.RegisterHit(longBtn.GetScore());
            Destroy(gameObject);
        }
    }
}
