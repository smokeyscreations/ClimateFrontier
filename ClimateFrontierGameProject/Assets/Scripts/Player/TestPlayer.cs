using UnityEngine;

namespace Scenes.BehaviorTreeTest
{
    /// <summary>
    /// TestPlayer is a simple implementation of BasePlayer for testing purposes.
    /// Attach this script to a dummy GameObject (e.g., a Capsule) to simulate a player.
    /// </summary>
    public class TestPlayer : BasePlayer
    {
        protected override void Awake()
        {
            base.Awake();
            // Additional initialization if necessary
        }

        public override void InitializePlayer()
        {
            base.InitializePlayer();
            // Additional initialization for testing if necessary
        }

        protected override void Update()
        {
            base.Update();
            // Additional update logic for testing if necessary
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            // Additional fixed update logic for testing if necessary
        }

        // You can override other methods from BasePlayer if you need custom behavior for testing
    }
}