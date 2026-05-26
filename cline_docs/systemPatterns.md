## System Patterns

### Architecture overview
- The mod currently follows a simple `ModSystem` entrypoint pattern in `ArcaneEquipmentModSystem.cs`.
- Lifecycle hooks currently used:
  - `Start(ICoreAPI api)`
  - `StartServerSide(ICoreServerAPI api)`
  - `StartClientSide(ICoreClientAPI api)`

### Asset/content structure
- Assets are organized under `ArcaneEquipment/assets/arcaneequipment/`.
- Localization file location follows Vintage Story convention:
  - `assets/arcaneequipment/lang/en.json`
- Planned item JSON location:
  - `assets/arcaneequipment/itemtypes/frostboots.json`

### Gameplay logic pattern (current and intended)
- Current code has no gameplay state mutation; only logger output.
- Intended pattern for this feature:
  - Register periodic server-side checks for equipped item state.
  - Apply or remove freeze/thaw effects based on conditions.
  - Keep logic centralized in `ArcaneEquipmentModSystem` until more systems exist.