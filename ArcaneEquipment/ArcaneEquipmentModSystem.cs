using ArcaneEquipment.Behaviors;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
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
            IInventory? inventory = player.InventoryManager.GetOwnInventory(GlobalConstants.characterInvClassName);
            if (inventory == null)
            {
                continue;
            }

            foreach (ItemSlot slot in inventory)
            {
                ItemStack? stack = slot.Itemstack;
                if (stack?.Collectible?.Code?.Domain == "arcaneequipment")
                {
                    sapi.Logger.Debug($"[ArcaneEquipment] Found arcaneequipment item '{stack.Collectible.Code}' on player '{player.PlayerName}'.");
                }
            }
        }

        sapi.Event.Timer(() => OnServerPassiveTick(), 500);
    }
}
