# ArcaneEquipment — Architecture Document
Version: 0.1
Status: Approved for implementation

## Overview
ArcaneEquipment adds named magic items inspired by DnD and Pathfinder to Vintage Story.
Each item has a unique effect that interacts visibly with the world. The mod is designed
for multiplayer RP servers and must be performant, configurable, and safe for live saves.

## Core Design Principles
- **Data-driven**: item behavior is defined in JSON, C# only implements trigger type classes
- **Performant**: single central tick loop, no per-item ticking
- **Scalable**: adding a new item requires JSON only, unless it needs a brand new trigger type
- **Safe**: all item state stored on ItemStack.Attributes, never server-side, to protect saves
- **Configurable**: every tunable value lives in a per-item ModConfig JSON file

---

## Trigger Types
Each trigger type is a CollectibleBehavior subclass. Items declare which behavior they use
in their item JSON. The behavior receives its config via the properties block in that JSON.

### 1. ArcanePassiveBehavior
- Always active while item is in the correct equipment slot
- Detected via server tick checking characterInvClassName inventory
- Example: Frost Walker Boots (freeze nearby water while equipped)

### 2. ArcaneUseActivatedBehavior  
- Fires on right-click or right-click hold
- Uses OnHeldInteractStart / OnHeldInteractStep hooks on the Item class
- Example: Staff of Tree Growth, Flask of Infinite Water

### 3. ArcaneToggleBehavior
- Player keybind turns effect on/off
- Toggle state stored in ItemStack.Attributes as a bool
- Client sends toggle packet to server via network channel
- Example: Cloak of Invisibility

### 4. ArcaneEnvironmentalBehavior
- Reacts to world state: biome, Y-level, temporal stability, weather
- Checked during the central server tick
- Example: Amulet that activates only during temporal storms

### 5. ArcaneEventBehavior
- Reacts to specific game events: fall, hit received, hit dealt, player death
- Hooks into VS event API in StartServerSide
- Example: Boots that negate fall damage only after falling 10+ blocks

### 6. ArcaneFuelBehavior
- Consumes durability or a specific item from player inventory on use or over time
- Fuel item and consumption rate defined in item config
- Example: A staff that consumes temporal gears to function

---

## Central ModSystem (ArcaneEquipmentModSystem)

### Responsibilities
- Registers all CollectibleBehavior classes on startup
- Runs a single server-side tick every 500ms via api.Event.Timer
- On each tick:
  - Loops through all online players
  - Gets their characterInvClassName inventory
  - Checks each slot for items with arcane behaviors
  - Calls the appropriate behavior handler if found
- Manages world-mutating effect tracking (e.g. frozen blocks, spawned trees)
- Loads and caches per-item config files from ModConfig

### What it does NOT do
- Does not run any logic client-side (effects are server-authoritative)
- Does not store player state server-side (all state on ItemStack.Attributes)
- Does not tick more than once per 500ms regardless of player count

---

## Item State Storage
All stateful data is stored on the ItemStack using TreeAttribute pattern:
- Toggle state: ItemStack.Attributes.SetBool("arcane_active", true)
- Charge count: ItemStack.Attributes.SetInt("arcane_charges", 5)
- Fuel remaining: ItemStack.Attributes.SetFloat("arcane_fuel", 1.0f)
- Cooldown: ItemStack.Attributes.SetDouble("arcane_cooldown_until", unixTime)

This ensures state survives server restarts, player disconnects, and mod updates.

---

## Config System
Each item has a config file at:
VintagestoryData/ModConfig/ArcaneEquipment/{itemid}.json

Example for Frost Walker Boots:
{
    "enabled": true,
    "freezeRadius": 2,
    "iceDurationSeconds": 30,
    "tickIntervalMs": 500
}

Defaults are baked into the behavior class and written to disk on first load.
Server owners can edit these without recompiling.

---

## File Structure
ArcaneEquipment/
  src/
    ArcaneEquipmentModSystem.cs       — central ModSystem, tick loop, startup
    Behaviors/
      ArcanePassiveBehavior.cs
      ArcaneUseActivatedBehavior.cs
      ArcaneToggleBehavior.cs
      ArcaneEnvironmentalBehavior.cs
      ArcaneEventBehavior.cs
      ArcaneFuelBehavior.cs
    Items/
      ItemFrostWalkerBoots.cs         — only needed if item requires unique logic
                                        beyond what the behavior class provides
  assets/arcaneequipment/
    itemtypes/
      equipment/
        frostwalkerboots.json
    lang/
      en.json
    config/                           — default config templates (copied to ModConfig)

---

## Adding a New Item — Checklist
1. Create itemtypes JSON declaring clothescategory, behaviors, and properties
2. Add lang entry in en.json
3. Create config JSON with tunables
4. If a new trigger type is needed, create a new CollectibleBehavior subclass
5. Register the behavior class in ArcaneEquipmentModSystem.Start()
6. If the item mutates the world (places/removes blocks), register its
   cleanup tracking in the central ModSystem

---

## Known Gaps / Future Work
- Client-side visual effects (particles, sounds) not yet designed
- Network channel for toggle keybind not yet implemented
- No equip/unequip events exist in VS API — passive detection relies on tick polling
- Cursed items (cannot be removed) will need custom ItemSlot logic — deferred
- Class-restricted items deferred until server class system is confirmed