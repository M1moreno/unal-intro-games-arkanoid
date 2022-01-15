using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArkanoidController : MonoBehaviour
{
    [SerializeField] private GridController _gridController;
    [Space(20)]
    [SerializeField] private List<LevelData> _levels = new List<LevelData>();
    private const string BALL_PREFAB_PATH = "Prefabs/Ball";
    private readonly Vector2 BALL_INIT_POSITION = new Vector2(0, -0.86f);
    private Ball _ballPrefab = null;
    public List<Ball> _balls = new List<Ball>();

    private int _currentLevel = 0;
    [SerializeField]private int _totalScore = 0;


    private void Start()
    {
        ArkanoidEvent.OnBallReachDeadZoneEvent += OnBallReachDeadZone;
        ArkanoidEvent.OnBlockDestroyedEvent += OnBlockDestroyed;
        ArkanoidEvent.OnPowerUpSpawn += OnSpawnPowerUp;
    }

    private void OnDestroy()
    {
        ArkanoidEvent.OnBallReachDeadZoneEvent -= OnBallReachDeadZone;
        ArkanoidEvent.OnBlockDestroyedEvent -= OnBlockDestroyed;
        ArkanoidEvent.OnPowerUpSpawn -= OnSpawnPowerUp;
    }

    private void OnBlockDestroyed(int blockId)
    {
        BlockTile blockDestroyed = _gridController.GetBlockBy(blockId);
        if (blockDestroyed != null)
        {
            _totalScore += blockDestroyed.Score;
            ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(blockDestroyed.Score, _totalScore);
        }

        if (_gridController.GetBlocksActive() == 0)
        {
            _currentLevel++;
            if (_currentLevel >= _levels.Count)
            {
                ClearBalls();
                Debug.LogError("Game Over: WIN!!!!");
            }

            else
            {
                SetInitialBall();
                ArkanoidEvent.OnLevelUpdatedEvent?.Invoke(_currentLevel);
                _gridController.BuildGrid(_levels[_currentLevel]);
            }

        }
    }

    private void OnBallReachDeadZone(Ball ball)
    {
        ball.Hide();
        _balls.Remove(ball);
        Destroy(ball.gameObject);

        CheckGameOver();
    }
    
    private void CheckGameOver()
    {
        //Game over
        if (_balls.Count == 0)
        {
            ClearBalls();
            
            Debug.Log("Game Over: LOSE!!!");
            ArkanoidEvent.OnGameOverEvent?.Invoke();
        }
    }

    private void SetInitialBall()
    {
        ClearBalls();
        Ball ball = CreateBallAt(BALL_INIT_POSITION);
        ball.Init();
        _balls.Add(ball);
    }

    private Ball CreateBallAt(Vector2 position)
    {
        if(_ballPrefab == null)
        {
            _ballPrefab = Resources.Load<Ball>(BALL_PREFAB_PATH);
        }
        return Instantiate(_ballPrefab, position, Quaternion.identity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InitGame();
        }
    }

    private void InitGame()
    {
        _currentLevel = 0;
        _totalScore = 0;
        _gridController.BuildGrid(_levels[0]);
        SetInitialBall();
        ArkanoidEvent.OnGameStartEvent?.Invoke();
        ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(0, _totalScore);
    }

    private void ClearBalls()
    {
        for (int i = _balls.Count - 1; i>=0; i--)
        {
            _balls[i].gameObject.SetActive(false);
            Destroy(_balls[i]);
        }

        _balls.Clear();
    }

    private void OnSpawnPowerUp(Vector3 position)
    {
        
        if(Random.Range(1f, 100f) < 30f)
        {
            MultiBall multiballPrefab = Resources.Load<MultiBall>("Prefabs/MultiBall");
            Instantiate(multiballPrefab,position,Quaternion.identity);
        }

        else if(Random.Range(1f, 10f) < 3f)
        {
            Fast FastPrefab = Resources.Load<Fast>("Prefabs/Fast");
            Instantiate(FastPrefab,position,Quaternion.identity);
        }

        else if(Random.Range(1f, 10f) < 9f)
        {
            Slow Prefab = Resources.Load<Slow>("Prefabs/Slow");
        }

        else if(Random.Range(1f, 1000f) < 300f)
        {
            ExtraPoints Prefab = Resources.Load<ExtraPoints>("Prefabs/ExtraPoints");
            Instantiate(Prefab,position,Quaternion.identity);
        }
    }

    public void powerup(string type)
    {
        if(type == "multiball")
        {
            if(_balls.Count < 3)
            {
                Ball ball;
                ball = Instantiate(_ballPrefab,transform.position,Quaternion.identity);
                ball.Init();
                _balls.Add(ball);
            }

            if(_balls.Count < 3)
            {
                Ball ball2;
                ball2 = Instantiate(_ballPrefab,transform.position,Quaternion.identity);
                ball2.Init();
                _balls.Add(ball2);
            }
        }

        else if(type == "fast")
        {
            for(int i=0; i < _balls.Count; i++)
            {
                float speed = _balls[i]._rb.velocity.magnitude + 1;
                if(speed < _balls[i]._maxSpeed)
                {
                    _balls[i]._rb.velocity *= speed;
                }
            }
        }

        else if(type == "slow")
        {
            for(int i=0; i < _balls.Count; i++)
            {
                float speed = _balls[i]._rb.velocity.magnitude - 1;
                if(speed > _balls[i]._minSpeed)
                {
                    _balls[i]._rb.velocity *= speed;
                }
            }
        }

        else if(type == "extrapoints")
        {
            _totalScore += 250;
            ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(250, _totalScore);
        }
    }
}
