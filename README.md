# Project Polyhedron

A procedural planetary voxel game built in Unity using the Data-Oriented Technology Stack (DOTS) and the Burst Compiler.

## Technical Overview

The core world generation bypasses standard 3D Cartesian voxel grids in favor of a geodesic discrete global grid (a Goldberg polyhedron). The planet is mathematically generated starting from a subdivided icosahedron, resulting in a world composed of hexagonal voxels (and 12 structural pentagons).

### Engine & Architecture

* **Engine:** Unity 6 (URP)
* **Graphics API:** Vulkan
* **Architecture:** Entity Component System (ECS) / Unity DOTS
* **Language:** C# (Burst Compiled Jobs)

## Current Development Phase

### Phase 1: Base Geometry & Mathematics

* [ ] Generate base icosahedron vertices via Burst Jobs.
* [ ] Subdivide faces to target LOD radius.
* [ ] Calculate dual graph centers for hexagonal cell generation.
* [ ] Extrude surface geometry for 3D voxel representation.
