using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    // la implementación de esta función está en javascript
    [DllImport("__Internal")]
    private static extern void SendMessageJS(string str);

    // la implementación de esta función está en javascript
    [DllImport("__Internal")]
    private static extern void ChangeButtonLabelJS(
        string buttonId,
        string buttonLabel
    );

    // textos en inglés
    /*
    public readonly string MARKED_WITH = " marked with ";

    public readonly string MARKED = "marked";

    public readonly string PLAYER_WITH_MARK = "Player with mark ";

    public readonly string WINS = " wins";

    public readonly string NOBODY_WINS = "Nobody Wins";

    public readonly string UPPER_LEFT = "Upper row left column";

    public readonly string UPPER_MIDDLE = "Upper row middle column";

    public readonly string UPPER_RIGHT = "Upper row right column";

    public readonly string MIDDLE_LEFT = "middle row left column";

    public readonly string MIDDLE_MIDDLE = "middle row middle column";

    public readonly string MIDDLE_RIGHT = "middle row right column";

    public readonly string LOW_LEFT = "lower row left column";

    public readonly string LOW_MIDDLE = "lower row middle column";

    public readonly string LOW_RIGHT = "lower row right column";

    public readonly string IT_IS_NOW_TURN_OF_PLAYER = "It is now turn of player with mark ";
    */

    // textos latinos
    public readonly string MARKED_WITH = " marcado con ";

    public readonly string MARKED = "marcado";

    public readonly string PLAYER_WITH_MARK = "Jugador con marca ";

    public readonly string WINS = " gana";

    public readonly string NOBODY_WINS = "Nadie gana";

    public readonly string UPPER_LEFT = "fila superior columna izquierda";

    public readonly string UPPER_MIDDLE = "fila superior columna central";

    public readonly string UPPER_RIGHT = "fila superior columna derecha";

    public readonly string MIDDLE_LEFT = "fila central columna derecha";

    public readonly string MIDDLE_MIDDLE = "fila central columna derecha";

    public readonly string MIDDLE_RIGHT = "fila central columna derecha";

    public readonly string LOW_LEFT = "fila inferior columna derecha";

    public readonly string LOW_MIDDLE = "fila inferior columna derecha";

    public readonly string LOW_RIGHT = "fila inferior columna derecha";

    public readonly string
        IT_IS_NOW_TURN_OF_PLAYER =
            "Ahora es el turno del jugador con la marca ";

    [Header("Input Settings : ")]
    [SerializeField]
    private LayerMask boxesLayerMask;

    [SerializeField]
    private float touchRadius;

    [Header("Mark Sprites : ")]
    [SerializeField]
    private Sprite spriteX;

    [SerializeField]
    private Sprite spriteO;

    [Header("Mark Colors : ")]
    [SerializeField]
    private Color colorX;

    [SerializeField]
    private Color colorO;

    public UnityAction<Mark, Color> OnWinAction;

    public Mark[] marks;

    private Camera cam;

    private Mark currentMark;

    private bool canPlay;

    private LineRenderer lineRenderer;

    private int marksCount = 0;

    private void Start()
    {
        cam = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        currentMark = Mark.X;

        marks = new Mark[9];

        canPlay = true;
    }

    private void Update()
    {
        if (canPlay && Input.GetMouseButtonUp(0))
        {
            Vector2 touchPosition = cam.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hit =
                Physics2D
                    .OverlapCircle(touchPosition, touchRadius, boxesLayerMask);

            if (
                hit //box is touched.
            ) HitBox(hit.GetComponent<Box>());
        }
    }

    public void MyCustomHit(int boxIndex)
    {
        string boxName = "Box" + boxIndex.ToString();
        Debug.Log("MyCustomHit " + boxName);

        GameObject playerGameObj = GameObject.Find(boxName);
        if (playerGameObj != null)
        {
            Box playerVar = playerGameObj.GetComponent<Box>();
            HitBox (playerVar);
        }
    }

    private void HitBox(Box box)
    {
        if (!box.isMarked)
        {
            marks[box.index] = currentMark;

            box.SetAsMarked(GetSprite(), currentMark, GetColor());
            marksCount++;

            string btnloc = GetButtonLocation(box.index);
            btnloc = btnloc + MARKED_WITH + currentMark.ToString();
            ChangeButtonLabelJS(box.index.ToString(), btnloc);

            SendMessageJS (MARKED);

            //check if anybody wins:
            bool won = CheckIfWin();
            if (won)
            {
                if (OnWinAction != null)
                    OnWinAction.Invoke(currentMark, GetColor());

                Debug.Log(currentMark.ToString() + " Wins.");
                SendMessageJS(PLAYER_WITH_MARK + currentMark.ToString() + WINS);

                canPlay = false;
                return;
            }

            if (marksCount == 9)
            {
                if (OnWinAction != null)
                    OnWinAction.Invoke(Mark.None, Color.white);

                Debug.Log("Nobody Wins.");
                SendMessageJS (NOBODY_WINS);

                canPlay = false;
                return;
            }

            SwitchPlayer();
        }
    }

    private string GetButtonLocation(int position)
    {
        string result;
        switch (position)
        {
            case 0:
                result = UPPER_LEFT;
                break;
            case 1:
                result = UPPER_MIDDLE;
                break;
            case 2:
                result = UPPER_RIGHT;
                break;
            case 3:
                result = MIDDLE_LEFT;
                break;
            case 4:
                result = MIDDLE_MIDDLE;
                break;
            case 5:
                result = MIDDLE_RIGHT;
                break;
            case 6:
                result = LOW_LEFT;
                break;
            case 7:
                result = LOW_MIDDLE;
                break;
            case 8:
                result = LOW_RIGHT;
                break;
            default:
                result = "unknown";
                break;
        }

        return result;
    }

    private bool CheckIfWin()
    {
        return AreBoxesMatched(0, 1, 2) ||
        AreBoxesMatched(3, 4, 5) ||
        AreBoxesMatched(6, 7, 8) ||
        AreBoxesMatched(0, 3, 6) ||
        AreBoxesMatched(1, 4, 7) ||
        AreBoxesMatched(2, 5, 8) ||
        AreBoxesMatched(0, 4, 8) ||
        AreBoxesMatched(2, 4, 6);
    }

    private bool AreBoxesMatched(int i, int j, int k)
    {
        Mark m = currentMark;
        bool matched = (marks[i] == m && marks[j] == m && marks[k] == m);

        if (matched) DrawLine(i, k);

        return matched;
    }

    private void DrawLine(int i, int k)
    {
        lineRenderer.SetPosition(0, transform.GetChild(i).position);
        lineRenderer.SetPosition(1, transform.GetChild(k).position);
        Color color = GetColor();
        color.a = .3f;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        lineRenderer.enabled = true;
    }

    private void SwitchPlayer()
    {
        currentMark = (currentMark == Mark.X) ? Mark.O : Mark.X;
        SendMessageJS(IT_IS_NOW_TURN_OF_PLAYER + currentMark.ToString());
    }

    private Color GetColor()
    {
        return (currentMark == Mark.X) ? colorX : colorO;
    }

    private Sprite GetSprite()
    {
        return (currentMark == Mark.X) ? spriteX : spriteO;
    }
}
