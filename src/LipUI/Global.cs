using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LipUI
{
    internal static class Global
    {
        internal static LipNETWrapper.ILipWrapper Lip = new LipNETWrapper.LipConsoleWrapper(
#if DEBUG
            "A:\\Documents\\GitHub\\BDS\\Latest\\lip.exe"
#endif
            );
        internal static async Task DispatcherInvokeAsync(Action act)
        {
            await Application.Current.Dispatcher.InvokeAsync(act);
        }
        internal static void DispatcherInvoke(Action act)
        {

            Application.Current.Dispatcher.Invoke(act);
        } 
    }
    /*
     * {
    "format_version": 1,
    "index": {
        "7zip": {
            "author": "Futrime",
            "description": "A Lip tool adaptation of 7-Zip",
            "homepage": "https://www.7-zip.org/",
            "license": "LGPL-2.1-or-later",
            "name": "7-Zip Lip Tool",
            "repository": "github.com/Tooth-Hub/7zip",
            "tooth": "github.com/Tooth-Hub/7zip"
        },
        "antitoolbox": {
            "author": "ShrBox",
            "description": "Block toolbox players",
            "homepage": "https://github.com/ShrBox/AntiToolbox",
            "license": "MIT",
            "name": "AntiToolbox",
            "repository": "github.com/ShrBox/AntiToolbox",
            "tooth": "github.com/Tooth-Hub/AntiToolbox"
        },
        "bds": {
            "author": "Mojang",
            "description": "Bedrock Dedicated Servers allow Minecraft players on Windows and Linux computers to set up their own server at home, or host their server using a cloud-based service.",
            "homepage": "https://www.minecraft.net",
            "license": "",
            "name": "Minecraft Bedrock Dedicated Server",
            "repository": "github.com/Tooth-Hub/BDS",
            "tooth": "github.com/Tooth-Hub/BDS"
        },
        "bdsdownloader": {
            "author": "Jasonzyt",
            "description": "A CLI tool to download BDS",
            "homepage": "https://github.com/Jasonzyt/BDSDownloader",
            "license": "MIT",
            "name": "BDS Donwloader",
            "repository": "github.com/Jasonzyt/BDSDownloader",
            "tooth": "github.com/Tooth-Hub/BDSDownloader"
        },
        "bdsx": {
            "author": "bdsx",
            "description": "Minecraft, Bedrock Dedicated Server mod with node.js ",
            "homepage": "https://github.com/bdsx/bdsx",
            "license": "MIT",
            "name": "bdsx",
            "repository": "github.com/bdsx/bdsx",
            "tooth": "github.com/Tooth-Hub/bdsx"
        },
        "helperlib": {
            "author": "LiteLScript-Dev",
            "description": "TypeScript.d.ts file with auto-completion and code hints for LiteLScript developers",
            "homepage": "https://github.com/LiteLScript-Dev/HelperLib/",
            "license": "",
            "name": "HelperLib",
            "repository": "github.com/LiteLScript-Dev/HelperLib",
            "tooth": "github.com/LiteLScript-Dev/HelperLib"
        },
        "lip": {
            "author": "LiteLDev",
            "description": "A package installer, not only for LiteLoaderBDS",
            "homepage": "https://github.com/LiteLDev/Lip",
            "license": "GPL-3.0-only",
            "name": "Lip",
            "repository": "github.com/LiteLDev/Lip",
            "tooth": "github.com/Tooth-Hub/Lip"
        },
        "liteloaderbds": {
            "author": "LiteLDev",
            "description": "An epoch-making cross-language plugin loader for Minecraft Bedrock Dedicated Server (BDS). ",
            "homepage": "https://docs.litebds.com",
            "license": "LGPL-3.0-only",
            "name": "LiteLoaderBDS",
            "repository": "github.com/LiteLDev/LiteLoaderBDS",
            "tooth": "github.com/Tooth-Hub/LiteLoaderBDS"
        },
        "ll": {
            "author": "LiteLDev",
            "description": "An epoch-making cross-language plugin loader for Minecraft Bedrock Dedicated Server (BDS). ",
            "homepage": "https://docs.litebds.com",
            "license": "LGPL-3.0-only",
            "name": "LiteLoaderBDS",
            "repository": "github.com/LiteLDev/LiteLoaderBDS",
            "tooth": "github.com/Tooth-Hub/LiteLoaderBDS"
        },
        "llanticheat": {
            "author": "LiteLDev",
            "description": "Free AntiCheat plugin for LiteLoaderBDS 2",
            "homepage": "https://github.com/LiteLDev/LLAntiCheat-release",
            "license": "",
            "name": "LLAntiCheat",
            "repository": "github.com/LiteLDev/LLAntiCheat-release",
            "tooth": "github.com/Tooth-Hub/LLAntiCheat"
        },
        "llbds": {
            "author": "LiteLDev",
            "description": "An epoch-making cross-language plugin loader for Minecraft Bedrock Dedicated Server (BDS). ",
            "homepage": "https://docs.litebds.com",
            "license": "LGPL-3.0-only",
            "name": "LiteLoaderBDS",
            "repository": "github.com/LiteLDev/LiteLoaderBDS",
            "tooth": "github.com/Tooth-Hub/LiteLoaderBDS"
        },
        "llcheckbag": {
            "author": "LiteLDev",
            "description": "Player Data Management",
            "homepage": "https://github.com/quizhizhe/LLCheckBag",
            "license": "",
            "name": "LLCheckBag",
            "repository": "github.com/quizhizhe/LLCheckBag",
            "tooth": "github.com/quizhizhe/Tooth-LLCheckBag"
        },
        "lldotnet": {
            "author": "LiteLDev",
            "description": "LiteLoader bindings for .NET",
            "homepage": "https://github.com/LiteLDev/LiteLoader.NET",
            "license": "LGPL-3.0-only",
            "name": "LiteLoader.NET",
            "repository": "github.com/LiteLDev/LiteLoader.NET",
            "tooth": "github.com/Tooth-Hub/LLDotNET"
        },
        "llessentials": {
            "author": "LiteLDev",
            "description": "Essentials plugin based on LiteLoaderBDS",
            "homepage": "https://github.com/LiteLDev/LLEssentials",
            "license": "GPL-3.0-only",
            "name": "LLEssentials",
            "repository": "github.com/LiteLDev/LLEssentials",
            "tooth": "github.com/Tooth-Hub/LLEssentials"
        },
        "llmoney": {
            "author": "LiteLDev",
            "description": "EconomyCore for LiteLoaderBDS",
            "homepage": "https://github.com/LiteLDev/LLMoney",
            "license": "GPL-3.0-only",
            "name": "LLMoney",
            "repository": "github.com/LiteLDev/LLMoney",
            "tooth": "github.com/Tooth-Hub/LLMoney"
        },
        "minecraftlevelexporter": {
            "author": "Futrime",
            "description": "Export and convert Minecraft level to NovelCraft level.",
            "homepage": "https://github.com/NovelCraft/MinecraftLevelExporter",
            "license": "LGPL-3.0-only",
            "name": "Minecraft Level Exporter",
            "repository": "github.com/NovelCraft/MinecraftLevelExporter",
            "tooth": "github.com/Tooth-Hub/MinecraftLevelExporter"
        },
        "pflp": {
            "author": "LazuliKao",
            "description": "brand new essentials plugin kit",
            "homepage": "https://github.com/LazuliKao/PixelFaramitaLuminousPolymerizationRes",
            "license": "",
            "name": "PixelFaramitaLuminousPolymerization",
            "repository": "github.com/LazuliKao/PixelFaramitaLuminousPolymerizationRes",
            "tooth": "github.com/Tooth-Hub/PFLP"
        },
        "pnx": {
            "author": "PowerNukkitX",
            "description": "Make Nukkit Great Again!",
            "homepage": "https://github.com/PowerNukkitX/PowerNukkitX",
            "license": "GPL-3.0",
            "name": "PowerNukkitX",
            "repository": "github.com/PowerNukkitX/PowerNukkitX",
            "tooth": "github.com/Tooth-Hub/PowerNukkitX"
        },
        "preloader": {
            "author": "LiteLDev",
            "description": "Library preloader for LiteLoaderBDS",
            "homepage": "https://github.com/LiteLDev/PreLoader",
            "license": "LGPL-3.0-only",
            "name": "PreLoader for LiteLoaderBDS",
            "repository": "github.com/LiteLDev/PreLoader",
            "tooth": "github.com/Tooth-Hub/PreLoader"
        },
        "redstonelimiter": {
            "author": "ShrBox",
            "description": "Limit redstone frequency on LiteLoaderBDS server",
            "homepage": "https://github.com/ShrBox/RedstoneLimiter",
            "license": "MIT",
            "name": "RedstoneLimiter",
            "repository": "github.com/ShrBox/RedstoneLimiter",
            "tooth": "github.com/Tooth-Hub/RedstoneLimiter"
        }
    }
}
     */
}
