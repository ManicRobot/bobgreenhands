using System;
using BobGreenhands.Map.MapObjects;
using BobGreenhands.Map.Tiles;
using BobGreenhands.Scenes;
using BobGreenhands.Utils.CultureUtils;


namespace BobGreenhands.Map.Items
{
    public class WateringCan : Item
    {
        public const int MaxCapacity = 5;

        public int Capacity;

        public WateringCan()
        {
            _type = ItemType.WateringCan;
            Capacity = MaxCapacity;
        }

        public override string? GetTooltipString()
        {
            return Language.Translate("wateringCan");
        }

        public override bool UsedOnTile(int tileX, int tileY, TileType tile, PlayScene playScene)
        {
            return false;
        }

        public override void UsedOnMapObject(MapObject mapObject, PlayScene playScene)
        {
            if(mapObject is Plant && Capacity > 0)
            {
                Plant plant = (Plant) mapObject;
                plant.Water = 1f;
                Capacity--;
            }
        }

        public override string? GetInfoString()
        {
            return "" + Capacity;
        }
    }
}