using System.Collections.Generic;
using UnityEngine;

public class cyanChangeable : MonoBehaviour, IChangeable
{
    [Header("Assign the waypoints IN ORDER")]
    public List<Transform> targets = new List<Transform>();
    private int _currentTarget = 0;

    [Header("Move?")]
    public bool move = false;

    [Header("Move Speed")]
    public float moveSpeed = 2f;

    [Header("Rotate?")]
    public bool rotate = false;

    [Header("Rotation Axis")]
    public Vector3 rotationAxis = new Vector3(0f, 1f, 0f);

    [Header("Rotate: Deg per Second")]
    public float rotateSpeed = 2f;

    [SerializeField] private AudioClip cyanSound;
    
    [SerializeField] private GameObject moveObject;

    private float _saveMoveSpeed;
    private float _saveRotateSpeed;
    
    private bool _isCyan; 
    public bool rotating = false;

    void Start()
    {
        _saveMoveSpeed = moveSpeed;
        _saveRotateSpeed = rotateSpeed;
    }

    void Update()
    {
        // ROTATE
        if (rotate)
        {
            rotating = true;
            moveObject.transform.Rotate(rotationAxis * rotateSpeed * Time.deltaTime);
        }

        // MOVE
        if (move)
        {
            moveObject.transform.position = Vector3.MoveTowards(moveObject.transform.position, targets[_currentTarget].position, moveSpeed * Time.deltaTime);
            if (transform.position == targets[_currentTarget].position)
            {
                NextTarget();
            }
        }
    }

    private void NextTarget()
    {
        if (_currentTarget == targets.Count - 1)
        {
            _currentTarget = 0;
        }
        else
        {
            _currentTarget++;
        }
    }

    public void Change(lightProperties.ColorOfLight colorOfLight, Transform none)
    {
        bool shouldBeCyan = (colorOfLight == lightProperties.ColorOfLight.CyanLight);

        if (shouldBeCyan && !_isCyan)
        {
            _isCyan = true;
            if(SoundManager.Instance != null) SoundManager.Instance.PlaySoundFX(cyanSound, transform, 1f);
            moveSpeed = 0f;
            rotateSpeed = 0f;
            if (rotate)
            {
                rotating = false;
            }
        }
        else if (!shouldBeCyan && _isCyan)
        {
            // switched away from cyan
            _isCyan = false;
            moveSpeed = _saveMoveSpeed;
            rotateSpeed = _saveRotateSpeed;
        }
    }

    public void UnChange(bool immediately)
    {
        _isCyan = false;
        moveSpeed = _saveMoveSpeed;
        rotateSpeed = _saveRotateSpeed;
        if (rotate)
        {
            rotating = true;
        }
    }
}
