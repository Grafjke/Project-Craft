using UnityEngine;



namespace ProjectCraft._Server
{
    public interface IInteractable
    {
        void Interact(ServerPlayerEntity player);

        float InteractionRadius { get; }

        UnityEngine.Vector3 Position { get; }
    }
}
