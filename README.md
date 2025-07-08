# Rimworld Teleporter Room

[![Atlantean Teleporter Room Demo](https://raw.githubusercontent.com/BetterRimworlds/TeleporterRoom/trunk/TeleporterRoom/About/Preview.png)](https://youtu.be/tGBTEr2PZH4)

The Teleporter Room allows you to teleport materials and pawns on the current map directly into another 
Teleporter Room, anywhere on the Planet.

[**Demo Video**](https://youtu.be/tGBTEr2PZH4)

When solar storms occur, the Teleporter Room is fully powered by the solar wind.

Conditions for a functional Teleporter Room:

1. It must be placed in a room that is at least 6x6 to fully fit the Teleporter.
2. The Room must not be bigger than 12x24 (288 cells).
3. The Room must be completely surrounded by Plasteel walls and doors, to facilitate the Zero-Point Energy transfer.
4. The Room must be completely roofed. No unroofed tiles.
5. The Room's floor must be completely covered in Sterile Tiles.
6. All requirements also apply for the remote Teleporter Room, as well.



## Change Log

**v1.2.0: 2025-07-08**
* **[2025-07-08 18:31:37 CDT]** General refactorings.
* **[2025-07-08 18:28:38 CDT]** Dramatically refactored the teleporter room validating mechanism.
* **[2025-07-08 18:26:17 CDT]** Improvement: Teleporter transmit buttons are now sorted alphabetically.
* **[2025-07-08 09:16:03 CDT]** Added support for Rimworld v1.6.
* **[2025-07-08 09:13:25 CDT]** Made the Teleporter unflickable.
* **[2025-07-08 09:00:13 CDT]** Increased the cost to build Teleporter by 2,500 silver.
* **[2025-07-08 08:59:35 CDT]** Majorly improved ./build.sh to handle XML changes as well.
* **[2025-07-08 08:58:41 CDT]** Migrated to a modern dotnet SDK project.
* **[2025-07-08 08:21:42 CDT]** Refactored the building requirements report.
* **[2025-07-08 08:20:45 CDT]** Fixed a NULL pointer when the teleporter was not connected to the power grid.
* **[2025-07-08 08:20:03 CDT]** Refactored the powered-on-during-solar-flares logic.
* **[2025-07-07 18:27:18 CDT]** Fixed logic relating to secondary teleporters not functioning.
* **[2025-07-07 13:46:33 CDT]** DeepSeek's bug fix with how new Teleporters are named.

**v1.1.0: 2025-03-15**
* **[2024-12-29 02:43:00 CST]** Greatly refactored power accumulation in line with the Stargate. origin/trunk
* **[2025-03-06 06:21:36 CDT]** [m] Updated the preview image.
* **[2025-03-15 08:18:22 CDT]** Allow the Stargate to be an allowable wall.
* **[2025-03-15 08:21:27 CDT]** Implemented the GetInspectString to show charging level.
* **[2025-03-15 08:32:22 CDT]** Ported to .NET v9.0 and C# v10.0.
* **[2025-03-15 08:50:53 CDT]** Majorly rewrote the build script.
* **[2025-03-15 10:16:02 CDT]** Added Settings to hide debug messages.
* **[2025-03-15 10:16:19 CDT]** Greatly increased the amount of work needed to build.

**v1.0.0: 2024-04-29**
* Initial Release

## Better Rimworlds Stargate Mods

1. [**Stargate**](https://github.com/BetterRimworlds/Stargate) — Send Pawns and Items to other Savegames on the same computer.
2. [**CryoRegenesis**](https://github.com/BetterRimworlds/CryoRegenesis) — Forever Young Glittertech (a Rimworld take on the Goa'uld Sarcophagus).
3. [**ZPM**](https://github.com/BetterRimworlds/ZPM) — Build your own or buy an Archotech Zero-Point Module (Stargate Atlantis).
4. [**ZatGun**](https://github.com/BetterRimworlds/ZatGun) — An actual Zat'nik'tel, from the Stargate Universe. One shot stuns. Two shots kills.

## Other Better Rimworlds Mods

1. [**WakeUp Implant**](https://github.com/BetterRimworlds/WakeUpImplant) — Installs a brain implant that gives the effects of a permanent wakeup high.
2. [**Savegame Shrinker**](https://github.com/BetterRimworlds/RimworldSavegameShrinker) — Cleans up unnecessary data from long-running Savegames.
3. [**DeMaterializer**](https://github.com/BetterRimworlds/DeMaterializer) — Precursor to teleportation. Late-stage raid defense system.

## Contributors

This mod is forked off of the incredible [**Stargate mod**](https://github.com/BetterRimworlds/Stargate).

# Contributors

[Theodore R. Smith](https://github.com/hopeseekr/]) <hopeseekr@gmail.com>  
GPG Fingerprint: D8EA 6E4D 5952 159D 7759  2BB4 EEB6 CE72 F441 EC41  
WhatsApp / Signal: +1 832-303-9477

## License

**CC-BY-ND-4.0**
Creative Commons NoDerivations v4.0: Please see the [license file](LICENSE.md) for more information.

**YOU MAY FORK THIS PROJECT.**

**YOU MAY NOT PUBLISH ANY DERIVATION of this project to either your own website or a third-party host.**

**YOU MAY NOT PUBLISH ANY DERIVATION ON STEAM WORKSHOP WITHOUT EXPLICIT APPROVAL.**
