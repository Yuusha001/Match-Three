using UnityEngine;

namespace MatchThreeEngine
{
	public sealed class Row : MonoBehaviour
	{
		public Tile[] tiles;
		public Tile tilePrefab;
        public void Initialize(int number)
        {
			tiles = new Tile[number];
			for (int i = 0; i < number; i++)
			{
				tiles[i] = Instantiate(tilePrefab, this.transform);
				
			}
        }
    }
}
