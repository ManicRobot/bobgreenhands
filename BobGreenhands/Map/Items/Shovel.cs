using System;
using BobGreenhands.Map.MapObjects;
using BobGreenhands.Map.Tiles;
using BobGreenhands.Scenes;
using BobGreenhands.Utils.CultureUtils;


namespace BobGreenhands.Map.Items
{
    public class Shovel : BreakableItem
    {
        public Shovel()
        {
            _type = ItemType.Shovel;
            Durability = 120;
            MaxDurability = 120;
        }

        public override string? GetInfoText()
        {
            return String.Format("{0}\n{1}", Language.Translate("item.shovel.name"), Language.Translate("game.quickInfo.durability", "" + Durability, "" + MaxDurability));
        }

        public override bool UsedOnTile(int tileX, int tileY, TileType tile, PlayScene playScene)
        {
            if(Durability <= 0)
            {
                return false;
            }
            if(tile == TileType.Grass)
            {
                PlayScene.CurrentSavegame.SetTileAt(tileX, tileY, TileType.Farmland);
                Durability--;
                return true;
            }
            return false;
        }

        public override void UsedOnMapObject(MapObject mapObject, PlayScene playScene)
        {
            if(Durability <= 0)
            {
                return;
            }
            if(mapObject is Plant)
            {
                playScene.DestroyMapObject(mapObject);
                Durability--;
            }
        }
    }
}