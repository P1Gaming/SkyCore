/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID PLAY_CAMPFIRE = 4000411161U;
        static const AkUniqueID PLAY_CAMPFIRE_EXTINGUISH = 2264789132U;
        static const AkUniqueID PLAY_ENV_BLOCK_SOUND = 2922412317U;
        static const AkUniqueID PLAY_ENV_DRONE_ALLSOUNDS = 2451341124U;
        static const AkUniqueID PLAY_ISLANDHEART_LEVELUP = 3487040007U;
        static const AkUniqueID PLAY_JELLIES_PET = 403792184U;
        static const AkUniqueID PLAY_JELLIES_ROAM_GRASS = 1406322581U;
        static const AkUniqueID PLAY_JELLIES_ROAM_STONE = 37469952U;
        static const AkUniqueID PLAY_JELLIES_ROAM_WOOD = 2015355878U;
        static const AkUniqueID PLAY_JELLY_EATING = 2405070987U;
        static const AkUniqueID PLAY_JELLYDUE_PICKUP = 3822484859U;
        static const AkUniqueID PLAY_MUSIC__LOADINGSCREEN_01 = 2124889703U;
        static const AkUniqueID PLAY_PAUSE_MENU_MUSIC = 1423477630U;
        static const AkUniqueID PLAY_PICKUP_ISLANDBLOCKS = 1113491808U;
        static const AkUniqueID PLAY_PICKUPITEM_BERRIES = 1376999814U;
        static const AkUniqueID PLAY_PLAYER_FOOTSTEPS = 98439365U;
        static const AkUniqueID PLAY_TITLE_SCREEN_MUSIC = 375134059U;
        static const AkUniqueID PLAY_UI__CRAFTINGMENU_CLOSINGCRAFTINGSYSTEM_01 = 1158385986U;
        static const AkUniqueID PLAY_UI__CRAFTINGMENU_CRAFT_01 = 3079436876U;
        static const AkUniqueID PLAY_UI__CRAFTINGMENU_CRAFTCANTDONE_01 = 1488088378U;
        static const AkUniqueID PLAY_UI__CRAFTINGMENU_LEFTCLICKICON_02 = 2406796967U;
        static const AkUniqueID PLAY_UI__CRAFTINGMENU_LISTOFITEMS_01 = 1848836357U;
        static const AkUniqueID PLAY_UI__CRAFTINGMENU_OPENINGUPCRAFTINGSYSTEM_01 = 2962648104U;
        static const AkUniqueID PLAY_UI__CRAFTINGMENU_RIGHTCLICKICON_02 = 2584199242U;
        static const AkUniqueID PLAY_UI__CRAFTINGMENU_SORTING_01 = 3170528434U;
        static const AkUniqueID PLAY_UI__CRAFTINGMENU_UPGRADE_01 = 2013194162U;
        static const AkUniqueID PLAY_UI_PAUSE_CLOSEMENU = 1572933919U;
        static const AkUniqueID PLAY_UI_PAUSE_INMENUSOUNDS = 2540695190U;
        static const AkUniqueID PLAY_UI_PAUSE_OPENMENU = 659570891U;
        static const AkUniqueID PLAY_UI_PAUSE_TABSOUNDS = 4190538351U;
        static const AkUniqueID PLAY_USEITEM_BLOCK = 2978452144U;
        static const AkUniqueID STOP_CAMPFIRE = 3878502195U;
        static const AkUniqueID STOP_TITLE_SCREEN_MUSIC = 4201027921U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace FIRESTATE
        {
            static const AkUniqueID GROUP = 1043523140U;

            namespace STATE
            {
                static const AkUniqueID FIREOFF = 205898616U;
                static const AkUniqueID FIREON = 307829538U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace FIRESTATE

        namespace GAMESTATE
        {
            static const AkUniqueID GROUP = 4091656514U;

            namespace STATE
            {
                static const AkUniqueID GAME_ON = 2219001485U;
                static const AkUniqueID GAME_PAUSE = 2772308904U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace GAMESTATE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace ITEMTYPE
        {
            static const AkUniqueID GROUP = 4247838896U;

            namespace SWITCH
            {
                static const AkUniqueID BLOCKBROKEN = 3465160839U;
                static const AkUniqueID BLOCKGRASS = 2065309022U;
                static const AkUniqueID BLOCKSTONE = 2903927127U;
                static const AkUniqueID BLOCKWOOD = 3915253415U;
            } // namespace SWITCH
        } // namespace ITEMTYPE

        namespace MATERIAL
        {
            static const AkUniqueID GROUP = 3865314626U;

            namespace SWITCH
            {
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID GRASS = 4248645337U;
                static const AkUniqueID SAND = 803837735U;
                static const AkUniqueID STONE = 1216965916U;
                static const AkUniqueID WOOD = 2058049674U;
            } // namespace SWITCH
        } // namespace MATERIAL

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID GAME_MUSIC_VOLUME = 1850957680U;
        static const AkUniqueID MASTER_VOLUME = 4179668880U;
        static const AkUniqueID SFX_VOLUME = 1564184899U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID GENERAL = 133642231U;
        static const AkUniqueID MUSIC = 3991942870U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID ENV = 529726550U;
        static const AkUniqueID GAME_MUSIC = 258110631U;
        static const AkUniqueID JELLIES = 2607341427U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID PAUSE_MENU_MUSIC = 2095980153U;
        static const AkUniqueID PLAYER = 1069431850U;
        static const AkUniqueID SFX = 393239870U;
        static const AkUniqueID UI = 1551306167U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
