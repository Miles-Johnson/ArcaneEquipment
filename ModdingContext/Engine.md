

### How the server tick system works
- The docs describe server-side systems as **event-driven** (`StartServerSide`, event registration), not a step-by-step tick loop.
- World generation handlers (`MapRegionGeneration`, `MapChunkGeneration`, `ChunkColumnGeneration`) run when chunks are needed.
- Heavy worldgen runs on a **background worldgen thread**, not the main server/game thread.
- Worldgen is split into ordered **passes** (`Terrain` → `TerrainFeatures` → `Vegetation` → `PreDone` etc.), and handlers run in registration/execution order.

### Performance considerations for multiplayer
- Keep expensive generation work off the main thread (as intended by worldgen API) to avoid frame/server hitching.
- Use the **worldgen block accessor** from `GetWorldgenBlockAccessor` for generation-thread safety; default accessor use there is described as questionable.
- Call `IWorldGenBlockAccessor.BeginColumn()` per handler call to avoid stale accessor cache issues.
- In hot loops, avoid excess allocations (example explicitly reuses `BlockPos`) to reduce GC pressure.
- Bound workload per chunk (example uses spawn probability + max chests per chunk).
- Use `updateHeightmap = true` when appropriate so rain/terrain height maps stay consistent.

### How block manipulation works from mod code
- Resolve blocks by code: e.g. `api.World.GetBlock(new AssetLocation("chest-south"))`.
- Place with `TryPlaceBlockForWorldGen(...)` using an appropriate `IBlockAccessor`.
- Use **different accessors by context**:
  - main/server command path: `api.World.BlockAccessor`
  - worldgen thread path: accessor from `chunkProvider.GetBlockAccessor(true)`
- After placement, fetch block entity via `GetBlockEntity(pos)`, initialize if needed, cast to `IBlockEntityContainer`, then edit inventory/item slots.

### How client/server sync works
- In these Engine docs, worldgen/mod example logic is explicitly **server-side** (`ShouldLoad` returning server-only).
- Server generates/stores chunk/map data once and reuses it; clients consume server world state rather than running that same mod logic client-side.
- Neighbor-chunk availability is pass-dependent (important for deterministic server-side generation before clients see results).
- No detailed packet-level networking sync model is provided in these specific Engine files.
