## Product Context

### Why this project exists
- This repository contains a Vintage Story mod named **ArcaneEquipment**.
- The mod description states it provides magical DnD/Pathfinder-inspired wearable items with unique passive effects.

### What problems it solves
- It adds magical equipment content to the game, including item definitions, localization text, and gameplay logic.

### How it should work
- The mod should load as a code mod (`modid: arcaneequipment`) and include matching assets under `assets/arcaneequipment`.
- New items should have:
  - A JSON item type definition under `assets/arcaneequipment/itemtypes/`
  - Localization entries in `assets/arcaneequipment/lang/en.json`
  - C# logic in `ArcaneEquipmentModSystem.cs` for passive behavior when relevant.

### Current requested feature
- Add a new item: `frostboots`
- Add English name/description localization
- Implement freeze/thaw behavior in the mod system