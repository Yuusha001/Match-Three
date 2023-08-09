using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MatchThreeEngine
{
    public sealed class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public int x;
        public int y;

        public Image icon;

        public Button button;
        public Board board;
        private TileTypeAsset _type;

        public TileTypeAsset Type
        {
            get => _type;

            set
            {
                if (_type == value) return;

                _type = value;

                icon.sprite = _type.sprite;
            }
        }

        public TileData Data => new TileData(x, y, _type.id);

        private Vector2 firstTouchPosition;
        private Vector2 finalTouchPosition;

        [Header("Swipe Stuff")]
        public float swipeAngle = 0;
        public float swipeResist = 1f;

        void CalculateAngle()
        {
            if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
            {
                swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
                if (swipeAngle > -45 && swipeAngle <= 45 && x < board.rows[y].tiles.Length - 1)
                {
                    //Right Swipe
                    Debug.Log($"[{x},{y}] Right");
                    board.Select2(this, board.GetTile(x + 1, y));

                }
                else if (swipeAngle > 45 && swipeAngle <= 135 && y < board.rows.Length - 1)
                {
                    //Up Swipe
                    Debug.Log($"[{x},{y}] Up");
                    board.Select2(this, board.GetTile(x, y - 1));

                }
                else if ((swipeAngle > 135 || swipeAngle <= -135) && x > 0)
                {
                    //Left Swipe
                    Debug.Log($"[{x},{y}] Left");
                    board.Select2(this, board.GetTile(x - 1, y));

                }
                else if (swipeAngle < -45 && swipeAngle >= -135 && y > 0)
                {
                    //Down Swipe
                    Debug.Log($"[{x},{y}] Down");
                    board.Select2(this, board.GetTile(x, y + 1));

                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
}