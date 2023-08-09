using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace MatchThreeEngine
{
	public sealed class Board : MonoBehaviour
	{
		public Row rowPrefab;
		private UI_GameplayManager gameplayManager;
        [SerializeField] private TileTypeAsset[] tileTypes;

		[SerializeField] public Row[] rows;

		[SerializeField] private float tweenDuration;

		[SerializeField] private Transform swappingOverlay;

		[SerializeField] private bool ensureNoStartingMatches;

		private readonly List<Tile> _selection = new List<Tile>();

		private bool _isSwapping;
		private bool _isMatching;
		private bool _isShuffling;

		public event Action<TileTypeAsset, int> OnMatch;
        public event Action<int> OnAddScore;
        public event Action OnMove;
        private TileData[,] Matrix
		{
			get
			{
				var width = rows.Max(row => row.tiles.Length);
				var height = rows.Length;

				var data = new TileData[width, height];

				for (var y = 0; y < height; y++)
					for (var x = 0; x < width; x++)
						data[x, y] = GetTile(x, y).Data;

				return data;
			}
		}

        public void Initialize(UI_GameplayManager gameplayManager, LevelData levelData)
        {
			this.gameplayManager = gameplayManager;
			//ClearBoard
			if(rows.Length > 0)
			{
                for (int i = 0; i < rows.Length; i++)
                {
					Destroy(rows[i].gameObject);
                }
            }

			//Setup Board
			rows = new Row[levelData.rowLength];
			for (int i = 0; i < levelData.rowLength; i++)
			{
				var row = Instantiate(rowPrefab, this.transform);
                row.Initialize(levelData.colLength);
				rows[i] = row;
			}
			tileTypes = new TileTypeAsset[levelData.numberOfType];
			//Setup Difficulty
			for (int i = 0; i < levelData.numberOfType; i++)
			{
				tileTypes[i] = DataManager.Instance.tileTypeContainer.typeAssets[i];
			}

			//Start Game
			GameStart();
        }


		private void GameStart()
		{
            for (var y = 0; y < rows.Length; y++)
            {
                for (var x = 0; x < rows.Max(row => row.tiles.Length); x++)
                {
                    var tile = GetTile(x, y);
                    tile.x = x;
                    tile.y = y;
                    tile.Type = tileTypes[Random.Range(0, tileTypes.Length)];
					tile.board = this;
                    tile.button.onClick.AddListener(() => Select(tile));
                }
            }

            if (ensureNoStartingMatches) 
				StartCoroutine(EnsureNoStartingMatches());
            OnMatch += OnMatching;
			OnMove += OnMoving;
        }

        public void Reset()
        {
			OnMatch -= OnMatching;
            OnMove -= OnMoving;
        }

        private void OnMatching(TileTypeAsset type, int count)
		{
            Debug.Log($"Matched {count}x {type.name}.");
            int score = type.value * count;
            OnAddScore?.Invoke(score);
        }

		private void OnMoving()
		{
			gameplayManager.gameTurn--;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				var bestMove = TileDataMatrixUtility.FindBestMove(Matrix);

				if (bestMove != null)
				{
					Select(GetTile(bestMove.X1, bestMove.Y1));
					Select(GetTile(bestMove.X2, bestMove.Y2));
				}
			}
		}

		private IEnumerator EnsureNoStartingMatches()
		{
			var wait = new WaitForEndOfFrame();

			while (TileDataMatrixUtility.FindBestMatch(Matrix) != null)
			{
				Shuffle();

				yield return wait;
			}
		}

		public Tile GetTile(int x, int y) => rows[y].tiles[x];

		private Tile[] GetTiles(IList<TileData> tileData)
		{
			var length = tileData.Count;

			var tiles = new Tile[length];

			for (var i = 0; i < length; i++) tiles[i] = GetTile(tileData[i].X, tileData[i].Y);

			return tiles;
		}

		private async void Select(Tile tile)
		{
			if (_isSwapping || _isMatching || _isShuffling) return;

			if (!_selection.Contains(tile))
			{
				if (_selection.Count > 0)
				{
					if (Math.Abs(tile.x - _selection[0].x) == 1 && Math.Abs(tile.y - _selection[0].y) == 0
					    || Math.Abs(tile.y - _selection[0].y) == 1 && Math.Abs(tile.x - _selection[0].x) == 0)
						_selection.Add(tile);
				}
				else
				{
					_selection.Add(tile);
				}
			}

			if (_selection.Count < 2) return;
			OnMove?.Invoke();
			await SwapAsync(_selection[0], _selection[1]);

			if (!await TryMatchAsync()) await SwapAsync(_selection[0], _selection[1]);

			var matrix = Matrix;

			while (TileDataMatrixUtility.FindBestMove(matrix) == null || TileDataMatrixUtility.FindBestMatch(matrix) != null)
			{
				Shuffle();
				matrix = Matrix;
			}
			gameplayManager.CheckWinCondition();
			_selection.Clear();
		}

        public async void Select2(Tile tile1, Tile tile2)
        {
            if (_isSwapping || _isMatching || _isShuffling) return;
            if (!_selection.Contains(tile1))
                _selection.Add(tile1);
            if (!_selection.Contains(tile2))
                _selection.Add(tile2);

            OnMove?.Invoke();
            await SwapAsync(_selection[0], _selection[1]);

            if (!await TryMatchAsync()) await SwapAsync(_selection[0], _selection[1]);

            var matrix = Matrix;

            while (TileDataMatrixUtility.FindBestMove(matrix) == null || TileDataMatrixUtility.FindBestMatch(matrix) != null)
            {
                Shuffle();
                matrix = Matrix;
            }
            gameplayManager.CheckWinCondition();
            _selection.Clear();
        }

        private async Task SwapAsync(Tile tile1, Tile tile2)
		{
			_isSwapping = true;

			var icon1 = tile1.icon;
			var icon2 = tile2.icon;

			var icon1Transform = icon1.transform;
			var icon2Transform = icon2.transform;

			icon1Transform.SetParent(swappingOverlay);
			icon2Transform.SetParent(swappingOverlay);

			icon1Transform.SetAsLastSibling();
			icon2Transform.SetAsLastSibling();

			var sequence = DOTween.Sequence();

			sequence.Join(icon1Transform.DOMove(icon2Transform.position, tweenDuration).SetEase(Ease.OutBack))
			        .Join(icon2Transform.DOMove(icon1Transform.position, tweenDuration).SetEase(Ease.OutBack));

			await sequence.Play()
			              .AsyncWaitForCompletion();

			icon1Transform.SetParent(tile2.transform);
			icon2Transform.SetParent(tile1.transform);

			tile1.icon = icon2;
			tile2.icon = icon1;

			var tile1Item = tile1.Type;

			tile1.Type = tile2.Type;

			tile2.Type = tile1Item;

			_isSwapping = false;
		}

		

        private async Task<bool> TryMatchAsync()
		{
			var didMatch = false;

			_isMatching = true;

			var match = TileDataMatrixUtility.FindBestMatch(Matrix);

            while (match != null)
            {
                didMatch = true;

                var tiles = GetTiles(match.Tiles);

                var deflateSequence = DOTween.Sequence();

                foreach (var tile in tiles) deflateSequence.Join(tile.icon.transform.DOScale(Vector3.zero, tweenDuration).SetEase(Ease.InBack));

                AudioManager.Instance.sfx.PlayOneShot(AudioManager.Instance.matchSound);

                await deflateSequence.Play()
                                     .AsyncWaitForCompletion();

                var inflateSequence = DOTween.Sequence();

                foreach (var tile in tiles)
                {
                    for (int i = tile.y; i >= 0; i--)
                    {
                        if (i > 0)
                            GetTile(tile.x, i).Type = GetTile(tile.x, i - 1).Type;
                        if (i == 0)
                            GetTile(tile.x, i).Type = tileTypes[Random.Range(0, tileTypes.Length)];
                    }

                    inflateSequence.Join(tile.icon.transform.DOScale(Vector3.one, tweenDuration).SetEase(Ease.OutBack));
                }

                await inflateSequence.Play()
                                     .AsyncWaitForCompletion();

                OnMatch?.Invoke(Array.Find(tileTypes, tileType => tileType.id == match.TypeId), match.Tiles.Length);

                match = TileDataMatrixUtility.FindBestMatch(Matrix);
            }

            _isMatching = false;

			return didMatch;
		}

		private void Shuffle()
		{
			_isShuffling = true;

			foreach (var row in rows)
				foreach (var tile in row.tiles)
					tile.Type = tileTypes[Random.Range(0, tileTypes.Length)];

			_isShuffling = false;
		}
	}
}
