using Rocket.API;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System.Collections;
using Steamworks;
using UnityEngine;

namespace InfiniteStamina
{
    public class Main : RocketPlugin
    {
        protected override void Load()
        {
            StartCoroutine(GiveInfiniteStamina());
        }

        protected override void Unload()
        {
            StopAllCoroutines();
        }

        private IEnumerator GiveInfiniteStamina()
        {
            while (true)
            {
                foreach (SteamPlayer steamPlayer in Provider.clients)
                {
                    if (steamPlayer?.player == null) continue;

                    steamPlayer.player.life.serverModifyStamina(255);
                }

                yield return new WaitForSeconds(0.1f); // update stamina 10 times per second
            }
        }
    }
}
