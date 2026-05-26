## Technical Context

### Technology stack
- Language: C#
- Framework/runtime target: .NET 8 (`net8.0`)
- Game API dependency: `VintagestoryAPI.dll`

### Project structure and build
- Solution: `ArcaneEquipment.sln`
- Main mod project: `ArcaneEquipment/ArcaneEquipment.csproj`
- Build output is configured to copy mod files to `bin/<Configuration>/Mods/mod`.

### Relevant files for current task
- `ArcaneEquipment/ArcaneEquipmentModSystem.cs` (mod lifecycle and server logic)
- `ArcaneEquipment/assets/arcaneequipment/lang/en.json` (localization)
- `ArcaneEquipment/assets/arcaneequipment/itemtypes/` (item definitions to create)

### Technical constraints
- Must follow Vintage Story asset path conventions and modid namespace (`arcaneequipment`).
- Should keep gameplay logic on server side where state changes occur.
- Existing codebase is minimal, so implementation should be straightforward and maintainable.