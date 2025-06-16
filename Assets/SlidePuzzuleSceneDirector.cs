using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlidePuzzuleSceneDirector : MonoBehaviour
{
    // ピース
    [SerializeField] List<GameObject> pieces;
    // ゲームクリア時に表示されるボタン
    [SerializeField] GameObject buttonRetry;
    // シャッフル回数
    [SerializeField] int shuffleCount;
    // ゲーム終了画面
    [SerializeField] GameObject panelResult;
    // サウンド
    [SerializeField] AudioClip seMove;

    // 初期位置
    List<Vector2> startPositions;

    // サウンド
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // サウンド
        audioSource = GetComponent<AudioSource>();
        // 初期位置を保存
        startPositions = new List<Vector2>();
        foreach (var item in pieces)
        {
            startPositions.Add(item.transform.position);
        }
        // 指定回数シャッフル
        for (int i = 0; i < shuffleCount; i++)
        {
            // 0番と隣接するピース
            List<GameObject> movablePieces = new List<GameObject>();

            // 0番と隣接するピースをリストに追加
            foreach (var item in pieces)
            {
                if (GetEmptyPiece(item) != null)
                {
                    movablePieces.Add(item);
                }
            }

            // 隣接するピースをランダムで入れ替える
            int rnd = Random.Range(0, movablePieces.Count);
            GameObject piece = movablePieces[rnd];
            SwapPiece(piece, pieces[0]);
        }

        // ボタン非表示
        buttonRetry.SetActive(false);

        // リザルト画面非表示
        panelResult.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // タッチ処理
        if (Input.GetMouseButtonUp(0))
        {
            // SE再生
            audioSource.PlayOneShot(seMove);
            // スクリーン座標からワールド座標に変換
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // レイを飛ばす
            RaycastHit2D hit2d = Physics2D.Raycast(worldPoint, Vector2.zero);

            // 当たり判定があった
            if (hit2d)
            {
                // ヒットしたゲームオブジェクト
                GameObject hitPiece = hit2d.collider.gameObject;
                // 0番のピースと隣接していればデータが入る
                GameObject emptyPiece = GetEmptyPiece(hitPiece);
                // 選んだピースと0番のピースを入れ替える
                SwapPiece(hitPiece, emptyPiece);

                // クリア判定
                buttonRetry.SetActive(true);

                // 正解の位置と違うピースを探す
                for (int i = 0; i < pieces.Count; i++)
                {
                    // 現在のポジション
                    Vector2 position = pieces[i].transform.position;
                    // 初期位置と違ったらボタンを非表示
                    if (position != startPositions[i])
                    {
                        buttonRetry.SetActive(false);
                    }
                }

                // クリア状態
                if (buttonRetry.activeSelf)
                {
                    // ゲームクリア
                    GameResult();
                }
            }
        }
    }

    // 引数のピースが0番のピースと隣接していたら０番のピースを返す
    GameObject GetEmptyPiece(GameObject piece)
    {
        float dist = Vector2.Distance(piece.transform.position, pieces[0].transform.position);

        if (dist == 1)
        {
            return pieces[0];
        }

        return null;
    }

    // 2つのピースの位置を入れ替える
    void SwapPiece(GameObject pieceA, GameObject pieceB)
    {
        // どちらかがnullなら処理をしない
        if (pieceA == null || pieceB == null)
        {
            return;
        }

        // AとBのポジションを入れ替える
        Vector2 position = pieceA.transform.position;
        pieceA.transform.position = pieceB.transform.position;
        pieceB.transform.position = position;
    }

    // リトライボタン
    public void OnClickRetry()
    {
        SceneManager.LoadScene("SlidePuzzleScene");
    }

    void GameResult()
    {
        // リザルトパネル表示
        panelResult.SetActive(true);

        // Updateを停止
        enabled = false;
    }
}
