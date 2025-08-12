namespace Perspective
{
    public interface IInteractable
    {
        public string InteractionPrompt { get; }
        public bool IsInteractable { get; }
        public void Interact();
    }
}
