using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthTankBody : TankBody
{
    private SpriteRenderer _baseSpriteRenderer = null;
    private Sprite _baseSprite;

    private SpriteRenderer _turretSpriteRenderer = null;
    private Sprite _turretSprite;

    [SerializeField]
    private float _stealthTime = 3f;

    [SerializeField]
    private float _stealthLockTime = 2f;

    [SerializeField]
    private float _stealthProbability = 1;

    private float _stealthTimer = 0f;
    private float _stealthLockTimer = 0f;
    private bool _canStealth = true;
    private bool _isStealth = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _baseSpriteRenderer = GetComponent<SpriteRenderer>();
        if(_baseSpriteRenderer == null)
        {
            Debug.Log("Base SpriteRendererÇ™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩÅB");
        }

        _turretSpriteRenderer = _turret.transform.Find("sprite").gameObject.GetComponent<SpriteRenderer>();
        if(_turretSpriteRenderer == null)
        {
            Debug.Log("Turret SpriteRendererÇ™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩÅB");
        }

        _baseSprite = _baseSpriteRenderer.sprite;
        _turretSprite = _turretSpriteRenderer.sprite;
    }

    public override void UpdateBody()
    {
        if (_isStealth)
        {
            _stealthTimer += Time.deltaTime;
            if(_stealthTimer >= _stealthTime)
            {
                _baseSpriteRenderer.sprite = _baseSprite;
                _turretSpriteRenderer.sprite = _turretSprite;
                _isStealth = false;
            }
        }
        else
        {

            if (!_canStealth)
            {
                _stealthLockTimer += Time.deltaTime;
                if (_stealthLockTimer >= _stealthLockTime)
                {
                    _canStealth = true;
                    _stealthLockTimer = 0f;
                }
            }

            if (_canStealth)
            {
                int random = Random.Range(0, 100);
                if (random <= _stealthProbability)
                {
                    _baseSpriteRenderer.sprite = null;
                    _turretSpriteRenderer.sprite = null;
                    _isStealth = true;
                    _stealthTimer = 0f;
                    _canStealth = false;
                }
            }
        }
    }
}
