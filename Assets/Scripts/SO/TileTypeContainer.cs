using UnityEngine;

namespace MatchThreeEngine
{
    [CreateAssetMenu(menuName = "Match 3 Engine/Tile Type Container")]
    public sealed class TileTypeContainer : ScriptableObject
    {
        public TileTypeAsset[] typeAssets;
    }
}