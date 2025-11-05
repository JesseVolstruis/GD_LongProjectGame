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

    private float _saveMoveSpeed;
    private float _saveRotateSpeed;

    void Start()
    {
        _saveMoveSpeed = moveSpeed;
        _saveRotateSpeed = rotateSpeed;
    }
    void Update()
    {
        //ROATE
        if (rotate)
        {
            transform.Rotate(rotationAxis * rotateSpeed * Time.deltaTime);
        }
        //MOVE
        if (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, targets[_currentTarget].position, moveSpeed * Time.deltaTime);
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
        if (colorOfLight == lightProperties.ColorOfLight.CyanLight)
        {
             moveSpeed = 0f;
             rotateSpeed = 0f;
        }
    }
    public void UnChange(bool immediately)
    {
        moveSpeed = _saveMoveSpeed;
        rotateSpeed = _saveRotateSpeed;
    }
}
