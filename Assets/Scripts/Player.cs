using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Threading.Tasks;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEngine.Events;



public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [Header("Camera関係")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private GameObject _lockonTargetObj;

    [Header("Move")]
    [SerializeField] private bool _isFixedUpdate = false;
    [SerializeField] private MoveMode _moveMode;
    [SerializeField] private TransformMove.Params _moveTrParams;
    [SerializeField] private TransformMove.Params _dashTrParams;
    [SerializeField] private RigidVelocityImpulseMove.Params _moveRbParams;
    [SerializeField] private RigidVelocityImpulseMove.Params _dashRbParams;
    [SerializeField] private float _dashTime;

    [Header("入力ドリフト対処用バッファー")]
    [SerializeField] private float _inputThreshold = 0.125f;


    public enum MoveMode {
        Transform,
        Rigidbody
    }

    public Dictionary<MoveMode, IMove> _moveDic = new();
    public Dictionary<MoveMode, IMove> _dashDic = new();



    private float _inputThreshold_SqrMag;

    private Rigidbody _rb;


    private bool _isDash = false;
    private Vector3 _dashDirection;

    private void Awake()
    {
        _inputThreshold_SqrMag = _inputThreshold * _inputThreshold;
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody>();
        

    }

    private void Start()
    {
        _moveDic.Add(MoveMode.Transform, new TransformMove(_moveTrParams, transform));
        _moveDic.Add(MoveMode.Rigidbody, new RigidVelocityImpulseMove(_moveRbParams, _rb));

        _dashDic.Add(MoveMode.Transform, new TransformMove(_dashTrParams, transform));
        _dashDic.Add(MoveMode.Rigidbody, new RigidVelocityImpulseMove(_dashRbParams, _rb));
     }

    private void Update()
    {
        if (_isFixedUpdate) return;
        if (_isDash) Dash();
        else Move();
    }

    private void FixedUpdate()
    {
        if (!_isFixedUpdate) return;
        if(_isDash) Dash();
        else Move();
    }

    private void OnEnable()
    {
        _playerInput.actions[StaticCommonParams.DASHINPUT].performed += OnDash;


    }

    private void OnDisable()
    {
        _playerInput.actions[StaticCommonParams.DASHINPUT].performed -= OnDash;

    }



    private void OnDash(InputAction.CallbackContext context)
    {
          _isDash = true;
          _dashDirection = GetInputMoveValueNomalize();
    }


    private float _dashTimeWatcher = 0;
    private void Dash()
    {
        _dashDic[_moveMode].Move(GetMoveDirection(_dashDirection));
        _dashTimeWatcher += Time.deltaTime;
        if(_dashTimeWatcher >= _dashTime)
        {
            _isDash = false;
            _dashTimeWatcher = 0;
        }

    }
    private void Move(){
        Vector2 moveInput = GetInputMoveValueNomalize();
        Vector3 moveDirection = GetMoveDirection(moveInput);
        _moveDic[_moveMode].Move(moveDirection);
    }



    
    

    /// <summary>
    /// 入力値をカメラの向きを前にするように回転したベクトルを出力
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMoveDirection(Vector2 moveInput)
    {
        return LockonDirection(moveInput);
  
    }


    /// <summary>
    /// ロックオン状態の方向
    /// </summary>
    /// <returns></returns>
    private Vector2 LockonDirection(Vector2 inputVector)
    {
        Vector3 targetDirection = _lockonTargetObj.transform.position - transform.position;
        Vector2 moveFront = new Vector2(targetDirection.x , targetDirection.z);
        //ワールドの座標(2次元)の前とカメラの前方方向の角度差分
        float offsetAngle =  Vector2.SignedAngle(Vector2.up, moveFront) * (Mathf.PI/180);
        //2次元の回転
        float sin = Mathf.Sin(offsetAngle);
        float cos = Mathf.Cos(offsetAngle);
        return new Vector2(inputVector.x * cos - inputVector.y * sin , inputVector.x * sin + inputVector.y * cos);
    }


    private Vector2 GetInputMoveValueNomalize()
    {
        return GetInputMoveValueNomalize8Snap();
    }

    /// <summary>
    /// 入力の8方向スナップ
    /// </summary>
    /// <returns></returns>
    private Vector2 GetInputMoveValueNomalize8Snap()
    {
        //シーンのリロード時はまだ設定されていないので
        Vector2? readValue = _playerInput?.actions[StaticCommonParams.MOVEINPUT].ReadValue<Vector2>();
        if(!readValue.HasValue) return Vector2.zero;

        if ( _inputThreshold_SqrMag> readValue.Value.sqrMagnitude) return  Vector2.zero;

        Vector2 nomalize  = readValue.Value.normalized;
        float angle = Vector2.Angle(Vector2.up, nomalize);
        float xSignal = 1;

        if (nomalize.x < 0) { 
            xSignal = -1;
        }
        if(angle <= 22.5f)
        {
            return new Vector2(0, 1);
        }
        if (angle <= 67.5f)
        {
            //大体√0.5
            return new Vector2(xSignal * 0.707f, 0.707f);
        }
        if(angle <= 112.5f)
        {

            return new Vector2( xSignal *1, 0);
        }
        if( angle <= 157.5f)
        {
            return new Vector2(xSignal * 0.707f, - 0.707f);
        }
        return new Vector2(0, -1);

    }
}
