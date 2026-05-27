using ArcaneEquipment.Behaviors;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace ArcaneEquipment;

public class ArcaneEquipmentModSystem : ModSystem
{
    private ICoreServerAPI? sapi;

    public override void Start(ICoreAPI api)
    {
        api.RegisterCollectibleBehaviorClass("ArcanePassiveBehavior", typeof(ArcanePassiveBehavior));
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        sapi = api;
        api.Event.Timer(() => OnServerPassiveTick(), 500);
    }

    private void OnServerPassiveTick()
    {
        if (sapi == null)
        {
            return;
        }

        foreach (IServerPlayer player in sapi.World.AllOnlinePlayers)
        {
            if (HasFrostBootsEquipped(player))
            {
                FreezeSurroundingWater(sapi.World, player);
            }
        }

        sapi.Event.Timer(() => OnServerPassiveTick(), 500);
    }

    private bool HasFrostBootsEquipped(IPlayer player)
    {
        IInventory? inventory = player.InventoryManager.GetOwnInventory(GlobalConstants.characterInvClassName);
        if (inventory == null)
        {
            return false;
        }

        foreach (ItemSlot slot in inventory)
        {
            ItemStack? stack = slot.Itemstack;
            if (stack?.Collectible?.Code?.Path == "frostwalkerboots")
            {
                return true;
            }
        }

        return false;
    }

    private void FreezeSurroundingWater(IWorldAccessor world, IPlayer player)
    {
        Block? iceBlock = world.GetBlock(new AssetLocation("game:ice"));
        if (iceBlock == null || iceBlock.BlockId == 0)
        {
            sapi?.Logger.Warning("[ArcaneEquipment] Could not resolve game:ice block. Frost Walker freeze skipped.");
            return;
        }

        BlockPos feetPos = player.Entity.ServerPos.AsBlockPos;

        for (int dx = -4; dx <= 4; dx++)
        {
            for (int dz = -4; dz <= 4; dz++)
            {
                if ((dx * dx) + (dz * dz) > 16)
                {
                    continue;
                }

                for (int dy = -2; dy <= 2; dy++)
                {
                    BlockPos pos = new BlockPos(feetPos.X + dx, feetPos.Y + dy, feetPos.Z + dz);
                    if (IsFreezableWater(world, pos))
                    {
                        world.BlockAccessor.SetBlock(iceBlock.BlockId, pos);
                    }
                }
            }
        }
    }

    private bool IsFreezableWater(IWorldAccessor world, BlockPos pos)
    {
        Block block = world.BlockAccessor.GetBlock(pos);
        string? path = block.Code?.Path;

        if (block.Code?.Domain != "game") return false;
        if (path == null || !path.StartsWith("water-still", System.StringComparison.Ordinal)) return false;
        if (!path.EndsWith("-7", System.StringComparison.Ordinal)) return false;

        BlockPos abovePos = pos.UpCopy();
        Block aboveBlock = world.BlockAccessor.GetBlock(abovePos);
        return aboveBlock.BlockId == 0;
    }
}
