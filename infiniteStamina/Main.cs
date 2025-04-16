using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace InfiniteForwardSprint
{
    public class Main : RocketPlugin
    {
        private Dictionary<CSteamID, Vector3> lastPositions = new Dictionary<CSteamID, Vector3>();

        protected override void Load()
        {
            StartCoroutine(MonitorPlayers());
        }

        protected override void Unload()
        {
            StopAllCoroutines();
            lastPositions.Clear();
        }

        private IEnumerator MonitorPlayers()
        {
            while (true)
            {
                foreach (SteamPlayer steamPlayer in Provider.clients)
                {
                    if (steamPlayer?.player == null) continue;

                    Player player = steamPlayer.player;
                    CSteamID id = steamPlayer.player.channel.owner.playerID.steamID;
                    Vector3 currentPosition = player.transform.position;

                    // save last position
                    if (!lastPositions.ContainsKey(id))
                    {
                        lastPositions[id] = currentPosition;
                        continue;
                    }

                    // check movement direction
                    Vector3 lastPosition = lastPositions[id];
                    Vector3 delta = currentPosition - lastPosition;
                    Vector3 movementDir = delta.normalized;

                    lastPositions[id] = currentPosition;

                    // keep stamina at max
                    player.life.serverModifyStamina(255);

                    float forwardDot = Vector3.Dot(player.transform.forward, movementDir);
                    float rightDot = Vector3.Dot(player.transform.right, movementDir);

                    bool isMoving = delta.magnitude > 0.01f;
                    bool isForward = forwardDot > 0.7f;
                    bool isSideways = Mathf.Abs(rightDot) > 0.4f;
                    bool isBackward = forwardDot < -0.1f;

                    if (isMoving)
                    {
                        if (isForward && !isSideways)
                        {
                            // allow sprinting
                            player.movement.sendPluginSpeedMultiplier(1.5f);
                        }
                        else
                        {
                            // force walk speed
                            player.movement.sendPluginSpeedMultiplier(1f);
                        }
                    }
                    else
                    {
                        // not moving
                        player.movement.sendPluginSpeedMultiplier(1f);
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
