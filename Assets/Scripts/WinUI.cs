using UnityEngine ;
using UnityEngine.UI ;
using UnityEngine.SceneManagement ;

public class WinUI : MonoBehaviour {
   [Header ("UI References :")]
   [SerializeField] private GameObject uiCanvas ;
   [SerializeField] private Text uiWinnerText ;
   [SerializeField] private Button uiRestartButton ;

   [Header ("Board Reference :")]
   [SerializeField] private Board board ;

    // english text
    /*
    public readonly string W_NOBODY_WINS = "Nobody Wins";
    public readonly string W_WINS = " Wins";
    */
    // spanish text
    public readonly string W_NOBODY_WINS = "Nadie gana";
    public readonly string W_WINS = " Gana";
    
   private void Start () {
      uiRestartButton.onClick.AddListener (() => SceneManager.LoadScene (0)) ;
      board.OnWinAction += OnWinEvent ;

      uiCanvas.SetActive (false) ;
   }

   private void OnWinEvent (Mark mark, Color color) {
      uiWinnerText.text = (mark == Mark.None) ? W_NOBODY_WINS : mark.ToString () + W_WINS ;
      uiWinnerText.color = color ;

      uiCanvas.SetActive (true) ;
   }

   private void OnDestroy () {
      uiRestartButton.onClick.RemoveAllListeners () ;
      board.OnWinAction -= OnWinEvent ;
   }
}
